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
    public class Hediff_Training : HediffWithComps
    {
        public int winCount = 0;

        public int previousWinCount = 0;

        public override bool Visible => true;

        public DefModExt_TrainingCurve modExt => def.GetModExtension<DefModExt_TrainingCurve>();

        public override void PostMake()
        {
            base.PostMake();

            winCount = Mathf.RoundToInt(modExt.curvePoints.EvaluateInverted(Severity));
        }

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);

            winCount = Mathf.RoundToInt(modExt.curvePoints.EvaluateInverted(Severity));
        }

        public override void Tick()
        {
            base.Tick();

            if (pawn.Faction != null && pawn.Faction.IsPlayer && Severity < 1.0f)
            {
                RunRecordCheck();
            }
        }

        public void RunRecordCheck()
        {
            int newCount = pawn.records.GetAsInt(OuterRimCoreDefOf.Kills) + pawn.records.GetAsInt(OuterRimCoreDefOf.PawnsDowned);
            if (newCount > previousWinCount)
            {
                winCount += (newCount - previousWinCount);
                previousWinCount = newCount;
                UpdateSeverity();
            }
        }

        public void UpdateSeverity()
        {
            Severity = modExt.curvePoints.Evaluate(winCount);
        }
    }
}
