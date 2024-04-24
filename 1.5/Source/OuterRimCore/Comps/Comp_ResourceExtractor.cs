using PipeSystem;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace OuterRimCore
{
    [StaticConstructorOnStartup]
    public class Comp_ResourceExtractor : ThingComp
    {
        public CompProperties_ResourceExtractor Props => (CompProperties_ResourceExtractor)props;

        public CompResourceStorage resource;
        public CompPowerTrader powerComp;

        public Sustainer sustainer;
        public int nextProduceTick = -1;
        public bool noCapacity = false;
        public List<IntVec3> adjCells;

        public List<IntVec3> lumpCells;

        public ThingDef checkingResource => parent.def.GetModExtension<DefModExt_ExtractableResource>().thing;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            resource = parent.GetComp<CompResourceStorage>();
            powerComp = parent.GetComp<CompPowerTrader>();
            adjCells = GenAdj.CellsAdjacent8Way(parent).ToList();

            if (lumpCells.NullOrEmpty())
            {
                lumpCells = new List<IntVec3>();
                var treated = new HashSet<IntVec3>();
                var toCheck = new Queue<IntVec3>();

                var cell = parent.Position;
                var map = parent.Map;

                toCheck.Enqueue(cell);
                treated.Add(cell);

                while (toCheck.Count > 0)
                {
                    var temp = toCheck.Dequeue();
                    lumpCells.Add(temp);

                    var neighbours = GenAdjFast.AdjacentCellsCardinal(temp);
                    for (int i = 0; i < neighbours.Count; i++)
                    {
                        var n = neighbours[i];
                        if (!treated.Contains(n) && map.deepResourceGrid.ThingDefAt(n) is ThingDef r && r == checkingResource)
                        {
                            treated.Add(n);
                            toCheck.Enqueue(n);
                        }
                    }
                }

                lumpCells.SortByDescending(c => c.DistanceTo(cell));
            }

            LongEventHandler.ExecuteWhenFinished(delegate
            {
                StartSustainer();
            });
        }

        public override void PostDeSpawn(Map map)
        {
            nextProduceTick = -1;
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref nextProduceTick, "nextProduceTick", 0);
            Scribe_Values.Look(ref noCapacity, "noCapacity", false);

            Scribe_Collections.Look(ref lumpCells, "lumpCells", LookMode.Value);
        }

        public override void CompTick()
        {
            base.CompTick();
            if (parent.Spawned && lumpCells.Count > 0)
            {
                var ticksGame = Find.TickManager.TicksGame;
                if (nextProduceTick == -1)
                {
                    nextProduceTick = ticksGame + Props.ticksPerPortion;
                }
                else if (ticksGame >= nextProduceTick)
                {
                    TryProducePortion();
                    nextProduceTick = ticksGame + Props.ticksPerPortion;
                }

                sustainer?.Maintain();
            }
            else
            {
                EndSustainer();
            }
        }

        public override string CompInspectStringExtra()
        {
            if (parent.Spawned)
            {
                if (noCapacity)
                {
                    return "OuterRim.NoCapacity".Translate();
                }
                if (lumpCells.Count == 0)
                {
                    return "DeepDrillNoResources".Translate();
                }
            }
            return null;
        }

        public void TryProducePortion()
        {
            // No power -> Return
            if (powerComp != null && !powerComp.PowerOn)
            {
                EndSustainer();
                return;
            }
            // Get resource
            bool nextResource = GetNextResource(out ThingDef resDef, out int count, out IntVec3 cell);
            // Resource isn't deepchem -> Return
            if (resDef == null || resDef != checkingResource)
                return;

            var map = parent.Map;
            // Resource comp is here
            if (resource != null)
            {
                var net = resource.PipeNet;
                if (net.connectors.Count > 1)
                {
                    noCapacity = net.AvailableCapacity < 1f;

                    if (!noCapacity)
                    {
                        net.DistributeAmongStorage(1f, out float stored);
                        parent.Map.deepResourceGrid.SetAt(cell, resDef, count - 1);
                        StartSustainer();
                    }
                    else
                    {
                        EndSustainer();
                    }
                    return;
                }
            }
            // If there is no resource comp
            // Deplete resource by one
            if (nextResource)
            {
                StartSustainer();
                parent.Map.deepResourceGrid.SetAt(cell, resDef, count - 1);
                // Spawn items
                for (int i = 0; i < adjCells.Count; i++)
                {
                    // Find an output cell
                    var c = adjCells[i];
                    if (c.Walkable(map))
                    {
                        // Try find thing of the same def
                        var t = c.GetFirstThing(map, resDef);
                        if (t != null)
                        {
                            if ((t.stackCount + 1) > t.def.stackLimit)
                                continue;
                            // We found some, modifying stack size
                            t.stackCount += 1;
                        }
                        else
                        {
                            // We didn't find any, creating thing
                            t = ThingMaker.MakeThing(resDef);
                            t.stackCount = 1;
                            GenPlace.TryPlaceThing(t, c, map, ThingPlaceMode.Near);
                        }
                        break;
                    }
                }
            }
        }

        public bool GetNextResource(out ThingDef resDef, out int countPresent, out IntVec3 cell)
        {
            var map = parent.Map;
            lumpCells.RemoveAll(c => map.deepResourceGrid.ThingDefAt(c) == null);

            if (lumpCells.Count > 0)
            {
                var c = lumpCells[0];

                if (map.deepResourceGrid.ThingDefAt(c) is ThingDef r)
                {
                    resDef = r;
                    countPresent = map.deepResourceGrid.CountAt(c);
                    cell = c;
                    return true;
                }
                else
                {
                    resDef = null;
                    countPresent = 0;
                    cell = c;
                    lumpCells.RemoveAt(0);
                    return false;
                }
            }
            resDef = null;
            countPresent = 0;
            cell = IntVec3.Invalid;
            return false;
        }

        public void StartSustainer()
        {
            if (!Props.ambientSound.NullOrUndefined() && sustainer == null)
            {
                SoundInfo info = SoundInfo.InMap(parent);
                sustainer = Props.ambientSound.TrySpawnSustainer(info);
            }
        }

        public void EndSustainer()
        {
            if (sustainer != null)
            {
                sustainer.End();
                sustainer = null;
            }
        }
    }
}
