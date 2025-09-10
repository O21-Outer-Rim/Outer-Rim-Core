using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace OuterRimCore
{
	public class WorkGiver_HaulToBactaPod : WorkGiver_HaulToBiosculpterPod
	{

		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForDef(OuterRimCoreDefOf.OuterRim_BactaPod);
			}
		}

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			if (t.IsForbidden(pawn) || !pawn.CanReserve(t, 1, -1, null, forced))
			{
				return false;
			}
			if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
			{
				return false;
			}
			Comp_BactaPod compBactaPod = t.TryGetComp<Comp_BactaPod>();
			if (compBactaPod == null || !compBactaPod.Powered || compBactaPod.cycleState != CycleState.Inactive)
			{
				return false;
			}
			if (t.IsBurning())
			{
				return false;
			}
			return true;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Comp_BactaPod compBactaPod = t.TryGetComp<Comp_BactaPod>();
			if (compBactaPod == null)
			{
				return null;
			}
			Job job = BactaUtil.HaulToBactaPodJob(pawn, t);
			return job;
		}
	}
}
