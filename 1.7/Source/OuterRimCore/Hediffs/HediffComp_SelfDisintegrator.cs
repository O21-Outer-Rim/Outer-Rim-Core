using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OuterRimCore
{
    public class HediffComp_SelfDisintegrator : HediffComp
    {
        public HediffCompProperties_SelfDisintegrator Props => (HediffCompProperties_SelfDisintegrator)props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (Pawn.Downed)
            {
                Notify_PawnDowned();
            }
        }

        public void Notify_PawnDowned()
        {
            if (!Pawn.Spawned)
            {
                return;
            }
            if (OuterRimCoreMod.settings.disintegrationDestroys && !Pawn.Faction.IsPlayer)
            {
                DestroyRandomApparel();
                DestroyRandomEquipment();
            }
            DisintegratePawn();
        }

        public void DestroyRandomEquipment()
        {
            List<ThingWithComps> eqList = Pawn.equipment.AllEquipmentListForReading;
            for (int i = 0; i < eqList.Count; i++)
            {
                if (Props.whitelist.Contains(eqList[i].def))
                {
                    continue;
                }
                else if (Props.blacklist.Contains(eqList[i].def))
                {
                    eqList[i].Destroy();
                }
                else if (Rand.Chance(0.5f))
                {
                    eqList[i].Destroy();
                }
            }
        }

        public void DestroyRandomApparel()
        {
            List<Apparel> apList = Pawn.apparel.WornApparel;
            for (int i = 0; i < apList.Count; i++)
            {
                if (Props.whitelist.Contains(apList[i].def))
                {
                    continue;
                }
                else if (Props.blacklist.Contains(apList[i].def))
                {
                    apList[i].Destroy();
                }
                else if (Rand.Chance(0.5f))
                {
                    apList[i].Destroy();
                }
            }
        }

        public void DisintegratePawn()
        {
            Pawn.DropAndForbidEverything();
            Pawn.apparel.DropAll(Pawn.Position);
            Pawn.equipment.DropAllEquipment(Pawn.Position);
            if (Pawn.Corpse == null)
            {
                Pawn.DeSpawn();
                Pawn.Kill(null, parent);
            }
            else
            {
                Pawn.Corpse.Destroy();
            }
            Pawn.Destroy();
        }

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            base.Notify_PawnDied(dinfo, culprit);
            if (!Pawn.Spawned)
            {
                return;
            }
            if (OuterRimCoreMod.settings.disintegrationDestroys && !Pawn.Faction.IsPlayer)
            {
                DestroyRandomApparel();
                DestroyRandomEquipment();
            }
            DisintegratePawn();
        }

        public override void Notify_PawnKilled()
        {
            if (!Pawn.Spawned)
            {
                return;
            }
            if (OuterRimCoreMod.settings.disintegrationDestroys && !Pawn.Faction.IsPlayer)
            {
                DestroyRandomApparel();
                DestroyRandomEquipment();
            }
            DisintegratePawn();
        }
    }
}
