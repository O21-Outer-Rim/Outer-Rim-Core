using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OuterRimCore
{
    public class PlaceWorker_ShowExtractableResources : PlaceWorker_ShowDeepResources
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            bool canPlace = false;
            var cell = IntVec3.Invalid;
            ThingDef checkingResource = checkingDef.GetModExtension<DefModExt_ExtractableResource>().thing;
            if (map.deepResourceGrid.ThingDefAt(loc) is ThingDef thingDef && thingDef == checkingResource)
            {
                canPlace = true;
                cell = loc;
            }
            if (cell != IntVec3.Invalid)
            {
                var good = new List<IntVec3>();
                var treated = new HashSet<IntVec3>();
                var toCheck = new Queue<IntVec3>();

                toCheck.Enqueue(cell);
                treated.Add(cell);

                while (toCheck.Count > 0)
                {
                    var temp = toCheck.Dequeue();
                    good.Add(temp);

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
                GenDraw.DrawFieldEdges(good, Color.white);
            }
            if (!canPlace)
            {
                return new AcceptanceReport("OuterRim.CantPlaceHere".Translate());
            }
            return canPlace;
        }

        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            base.DrawGhost(def, center, rot, ghostCol, thing);
            if (thing != null && thing.TryGetComp<Comp_ResourceExtractor>() is Comp_ResourceExtractor c)
            {
                GenDraw.DrawFieldEdges(c.lumpCells, Color.white);
            }
        }
    }
}
