using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using VFECore.Abilities;

namespace OuterRimCore
{
	public class Ability_Jetpack : VFECore.Abilities.Ability
	{
		public override void Cast(params GlobalTargetInfo[] targets)
		{
			base.Cast(targets);
			CastEffects(targets);
			LongEventHandler.QueueLongEvent(delegate
			{
				Map map = pawn.Map;
				AbilityPawnFlyer obj = (AbilityPawnFlyer)PawnFlyer.MakeFlyer(OuterRimCoreDefOf.OuterRim_JetpackJump_Flyer, pawn, targets.First().Cell, null, null);
				obj.ability = this;
				obj.target = targets.First().Cell;
				GenSpawn.Spawn(obj, targets.First().Cell, map);
			}, "jetpackFlightAbility", doAsynchronously: false, null);
		}

		public override bool CanHitTarget(LocalTargetInfo target, bool sightCheck)
		{
			return base.CanHitTarget(target, sightCheck: false);
		}

		public override bool CanHitTarget(LocalTargetInfo target)
		{
			if (base.CanHitTarget(target))
			{
				return target.Cell.Walkable(base.Caster.Map);
			}
			return false;
		}
	}
}
