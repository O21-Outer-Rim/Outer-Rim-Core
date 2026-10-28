using PipeSystem;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace OuterRimCore
{
    [StaticConstructorOnStartup]
    public class Comp_BactaPod : ThingComp, IThingHolder, ISuspendableThingHolder, IThingHolderWithDrawnPawn
    {
        public static readonly Material BackgroundMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.37f, 0.63f, 0.70f));

        public static readonly Material BarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(1f, 1f, 1f));
        public static readonly Material BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0f, 0f, 0f, 0f));

        public CompProperties_BactaPod Props => (CompProperties_BactaPod)props;

        public CompPowerTrader powerComp;
        public CompRefuelable fuelComp;
        public CompResourceTrader resourceComp;

        public ThingOwner innerContainer;

        public int currTick = -1;
        public bool cycleStarted = false;
        public CycleState cycleState;

        public bool IsContentsSuspended => true;

        public float HeldPawnDrawPos_Y => parent.DrawPos.y - 3f / 74f;

        public float HeldPawnBodyAngle => parent.Rotation.Opposite.AsAngle;

        public PawnPosture HeldPawnPosture => PawnPosture.LayingOnGroundFaceUp;

        public bool Empty => innerContainer.NullOrEmpty();

        public Pawn Occupant
        {
            get
            {
                if (Empty)
                {
                    return null;
                }
                return innerContainer[0] as Pawn;
            }
        }

        public bool Powered => (powerComp == null || powerComp.PowerOn) && (fuelComp == null || fuelComp.HasFuel);

        public bool HasBacta => resourceComp.PipeNet.CurrentStored() > BactaConsumption;

        public float BactaConsumption => Props.bactaPerCycle / Props.ticksPerCycle;

        public bool EnoughBactaForCycle => resourceComp.PipeNet.CurrentStored() > Props.bactaPerCycle;

        public int TicksRemainingTillHealed => (Props.ticksPerCycle * CyclesTillHealed(Occupant)) - currTick;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
            Scribe_Values.Look(ref currTick, "currTick", -1);
            Scribe_Values.Look(ref cycleStarted, "cycleStarted");
            Scribe_Values.Look(ref cycleState, "cycleState");
        }

        public Comp_BactaPod()
        {
            innerContainer = new ThingOwner<Thing>(this);
        }

        public override void CompTick()
        {
            base.CompTick();
            if (Occupant != null)
            {
                if (HasWoundsToHeal(Occupant))
                {
                    if (!Powered)
                    {
                        // No power means stasis field is disabled and pawn should tick.
                        if (cycleStarted)
                        {
                            cycleState = CycleState.Unpowered;
                        }
                        return;
                    }
                    if (!HasBacta)
                    {
                        // No bacta is fine, it will just pause the process.
                        cycleState = CycleState.AwaitingBacta;
                        return;
                    }
                    RunOrInitiateCycle();
                }
                else
                {
                    EjectContents();
                    ResetCycle();
                }
            }
        }

        public void RunOrInitiateCycle()
        {
            if (!cycleStarted)
            {
                // Attempt Healing Cycle
                if (EnoughBactaForCycle)
                {
                    InitiateCycle();
                }
                else
                {
                    cycleState = CycleState.AwaitingBacta;
                    return;
                }
            }
            RunCycle();
        }

        public void RunCycle()
        {
            if (cycleStarted)
            {
                currTick++;
                resourceComp.PipeNet.DrawAmongStorage(BactaConsumption, resourceComp.PipeNet.storages);
                if (currTick >= Props.ticksPerCycle)
                {
                    CompleteCycle();
                    ResetCycle();
                }
            }
        }

        public void CompleteCycle()
        {
            List<Hediff> hediffs = GetHealableHediffs(Occupant).ToList();
            for (int i = 0; i < hediffs.Count; i++)
            {
                Hediff h = hediffs[i];
                h.Heal(Props.healingPerCycle);
                if (h.ShouldRemove)
                {
                    Occupant.health.RemoveHediff(h);
                }
                if (h.TendableNow())
                {
                    h.Tended(2f, 2f);
                }
            }
        }

        public void ResetCycle()
        {
            cycleStarted = false;
            currTick = -1;
            cycleState = CycleState.Inactive;
        }

        public void InitiateCycle()
        {
            currTick = 0;
            cycleStarted = true;
            cycleState = CycleState.Working;
        }

        public bool HasWoundsToHeal(Pawn pawn)
        {
            if (!GetHealableHediffs(pawn).EnumerableNullOrEmpty())
            {
                return true;
            }
            return false;
        }

        public IEnumerable<Hediff> GetHealableHediffs(Pawn pawn)
        {
            if (pawn != null && !pawn.health.hediffSet.hediffs.NullOrEmpty())
            {
                foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
                {
                    if (hediff.def.hediffClass == typeof(Hediff_Injury) && hediff.TendableNow())
                    {
                        yield return hediff;
                    }
                }
            }
            yield break;
        }

        public int CyclesTillHealed(Pawn pawn)
        {
            if (!GetHealableHediffs(pawn).EnumerableNullOrEmpty())
            {
                float worstSeverity = 0f;
                foreach (Hediff hediff in GetHealableHediffs(pawn))
                {
                    if (hediff.Severity > worstSeverity)
                    {
                        worstSeverity = hediff.Severity;
                    }
                }
                return Mathf.CeilToInt(worstSeverity / Props.healingPerCycle);
            }
            return 0;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (!base.CompGetGizmosExtra().EnumerableNullOrEmpty())
            {
                foreach (Gizmo gizmo in base.CompGetGizmosExtra())
                {
                    yield return gizmo;
                }
            }
            if (parent.Faction == Faction.OfPlayer && innerContainer.Count > 0 && parent.def.building.isPlayerEjectable)
            {
                Command_Action command_Action = new Command_Action();
                command_Action.action = EjectContents;
                command_Action.defaultLabel = "OuterRim.BactaPodEjectLabel".Translate();
                command_Action.defaultDesc = "OuterRim.BactaPodEjectDesc".Translate();
                if (innerContainer.Count == 0)
                {
                    command_Action.Disable("CommandPodEjectFailEmpty".Translate());
                }
                command_Action.hotKey = KeyBindingDefOf.Misc8;
                command_Action.icon = ContentFinder<Texture2D>.Get("UI/Commands/PodEject");
                yield return command_Action;
            }
        }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            if (selPawn.IsQuestLodger())
            {
                yield return new FloatMenuOption("CannotEnter".Translate() + ": " + "CryptosleepCasketGuestsNotAllowed".Translate().CapitalizeFirst(), null);
                yield break;
            }
            string failReason = CannotUseNowReason() ?? CannotUseNowPawnReason(selPawn);
            string text = failReason;
            if (text != null)
            {
                yield return new FloatMenuOption(text, null);
                yield break;
            }
            string label = "OuterRim.EnterBactaPod".Translate().CapitalizeFirst();
            Action action = delegate
            {
                Job job = JobMaker.MakeJob(OuterRimCoreDefOf.OuterRim_EnterBactaPod, parent);
                selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            };
            yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action), selPawn, parent);
        }

        public string CannotUseNowReason()
        {
            if (!Powered)
            {
                return "NoPower".Translate().CapitalizeFirst();
            }
            if (!EnoughBactaForCycle)
            {
                return "OuterRim.NotEnoughBactaForCycle".Translate().CapitalizeFirst();
            }
            if (Occupant != null)
            {
                return "OuterRim.AlreadyOccupied".Translate().CapitalizeFirst();
            }
            return null;
        }

        public string CannotUseNowPawnReason(Pawn p)
        {
            if (!p.RaceProps.IsFlesh)
            {
                return "OuterRim.PawnIsNotOrganic".Translate().CapitalizeFirst();
            }
            if (!HasWoundsToHeal(p))
            {
                return "OuterRim.PawnHasNoWoundsToHeal".Translate().CapitalizeFirst();
            }
            if (!p.CanReach(parent, PathEndMode.InteractionCell, Danger.Deadly))
            {
                return "NoPath".Translate().CapitalizeFirst();
            }
            return null;
        }

        public override string CompInspectStringExtra()
        {
            string inspectString = base.CompInspectStringExtra();
            string text = "";
            text += "OuterRim.CurrentState".Translate() + " " + GetStateString();
            if (Occupant != null)
            {
                text += "\n" + "OuterRim.WoundsRemaining".Translate(GetHealableHediffs(Occupant).Count(), CyclesTillHealed(Occupant));
                text += "\n" + "OuterRim.BactaTotalNeeded".Translate((Props.bactaPerCycle * CyclesTillHealed(Occupant)).ToString());
            }
            text += "\n" + "OuterRim.BactaUsedPerCycle".Translate(Props.bactaPerCycle.ToString(), resourceComp.Resource.unit);
            return inspectString + text;
        }

        public string GetStateString()
        {
            switch (cycleState)
            {
                case CycleState.Inactive: return "OuterRim.State_Inactive".Translate();
                case CycleState.Unpowered: return "OuterRim.State_Unpowered".Translate();
                case CycleState.AwaitingBacta: return "OuterRim.State_AwaitingInput".Translate();
                case CycleState.Working: return "OuterRim.State_Working".Translate(TicksRemainingTillHealed.ToStringTicksToPeriod());
                case CycleState.Complete: return "OuterRim.State_Finished".Translate();
                default: return "OuterRim.State_Invalid".Translate();
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();
            DrawPawn();
            DrawProgressBar();
        }

        public void DrawProgressBar()
        {

        }

        public void DrawPawn()
        {
            Rot4 rotation = parent.Rotation;
            Vector3 s = new Vector3(parent.def.graphicData.drawSize.x * 0.7f, 1f, parent.def.graphicData.drawSize.y * 0.7f);
            Vector3 drawPos = parent.DrawPos;
            drawPos.y -= 0.08108108f;
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(drawPos, rotation.AsQuat, s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, BackgroundMat, 0);
            if (Occupant != null)
            {
                Pawn occupant = Occupant;
                Vector3 drawLoc = parent.DrawPos;
                Rot4 rotation2 = parent.Rotation;
                if (rotation2 == Rot4.North || rotation2 == Rot4.South)
                {
                    drawLoc.x -= 0.5f;
                    drawLoc.z += 0.2f;
                }
                if (rotation2 == Rot4.East || rotation2 == Rot4.West)
                {
                    drawLoc.z += 0.2f;
                }
                occupant.Drawer.renderer.RenderPawnAt(drawLoc, null, neverAimWeapon: true);
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            powerComp = parent.TryGetComp<CompPowerTrader>();
            fuelComp = parent.TryGetComp<CompRefuelable>();
            resourceComp = parent.TryGetComp<CompResourceTrader>();
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return innerContainer;
        }

        public void EjectContents()
        {
            innerContainer.TryDropAll(parent.InteractionCell, parent.Map, ThingPlaceMode.Near);
        }

        public bool Accepts(Thing thing)
        {
            return innerContainer.CanAcceptAnyOf(thing);
        }

        public bool TryAcceptThing(Thing thing, bool allowSpecialEffects = true)
        {
            if (!Accepts(thing))
            {
                return false;
            }
            bool flag = false;
            if (thing.holdingOwner != null)
            {
                thing.holdingOwner.TryTransferToContainer(thing, innerContainer, thing.stackCount);
                flag = true;
            }
            else
            {
                flag = innerContainer.TryAdd(thing);
            }
            if (flag)
            {
                if (allowSpecialEffects)
                {
                    SoundDefOf.CryptosleepCasket_Accept.PlayOneShot(new TargetInfo(parent.Position, parent.Map));
                }
                return true;
            }
            return false;
        }

        public static Building FindBactaPodFor(Pawn p, Pawn traveler, bool ignoreOtherReservations = false)
        {
            foreach (ThingDef item in DefDatabase<ThingDef>.AllDefs.Where((ThingDef def) => IsBactaPod(def)))
            {
                Building building = (Building)GenClosest.ClosestThingReachable(p.Position, p.Map, ThingRequest.ForDef(item), PathEndMode.InteractionCell, TraverseParms.For(traveler), 9999f, (Thing x) => ((Building)x).TryGetComp<Comp_BactaPod>()?.Occupant == null && traveler.CanReserve(x, 1, -1, null, ignoreOtherReservations));
                if (building != null)
                {
                    return building;
                }
            }
            return null;
        }

        public static bool IsBactaPod(ThingDef def)
        {
            return def == OuterRimCoreDefOf.OuterRim_BactaPod;
        }
    }
}
