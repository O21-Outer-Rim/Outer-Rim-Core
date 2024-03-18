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
    public class Hediff_IonBuildup : HediffWithComps
    {
        public override void Tick()
        {
            base.Tick();

            if (pawn.stances.stunner.AffectedByEMP)
            {
                if (Severity >= 1f)
                {
                    KillNow();
                }
            }
            else
            {
                pawn.health.hediffSet.hediffs.Remove(this);
            }
        }

        public void KillNow()
        {
            pawn.Kill(null);
            pawn.health.hediffSet.hediffs.Remove(this);
        }
    }
}
