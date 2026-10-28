using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace OuterRimCore
{
    public class FloatMenuOptionProvider_CarryToBactaPod : FloatMenuOptionProvider
    {
        public override bool Drafted => true;

        public override bool Undrafted => true;

        public override bool Multiselect => false;

        public override bool RequiresManipulation => true;

        public override FloatMenuOption GetSingleOptionFor(Pawn clickedPawn, FloatMenuContext context)
        {
            if (!clickedPawn.Downed) { return null; }
            if (!context.FirstSelectedPawn.CanReserveAndReach(clickedPawn, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, true)) { return null; }
            Building bactaPod = Comp_BactaPod.FindBactaPodFor(clickedPawn, context.FirstSelectedPawn, false);
            if (bactaPod == null) 
            {
                Log.Message("Bacta Pod Not Found");
                return null; 
            }
            TaggedString taggedString = "OuterRim.CarryPawnToBacta".Translate(clickedPawn.LabelCap, clickedPawn);
            Action action = delegate ()
            {
                Job job = JobMaker.MakeJob(OuterRimCoreDefOf.OuterRim_CarryToBactaPod, clickedPawn, bactaPod);
                job.count = 1;
                context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, new JobTag?(JobTag.Misc), false);
            };
            return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(taggedString, action, MenuOptionPriority.Default, null, clickedPawn, 0f, null, null, true, 0), context.FirstSelectedPawn, clickedPawn, "ReservedBy", null);
        }
    }
}
