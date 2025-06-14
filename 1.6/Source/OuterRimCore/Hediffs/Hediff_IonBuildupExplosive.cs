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
    public class Hediff_IonBuildupExplosive : HediffWithComps
    {
        public override void Tick()
        {
            base.Tick();

            if (pawn.stances.stunner.CanBeStunnedByDamage(DamageDefOf.EMP) || pawn.stances.stunner.CanBeStunnedByDamage(OuterRimCoreDefOf.OuterRim_Ion))
            {
                if(Severity >= 1f)
                {
                    ExplodeNow();
                }
            }
            else
            {
                pawn.health.hediffSet.hediffs.Remove(this);
            }
        }

        public void ExplodeNow()
        {
            GenExplosion.DoExplosion(pawn.Position, pawn.Map, pawn.BodySize * 1.5f, DamageDefOf.EMP, pawn);
            pawn.Kill(null);
            pawn.health.hediffSet.hediffs.Remove(this);
        }
    }
}
