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
	public class Ability_Pull : VFECore.Abilities.Ability
	{
		public override void Cast(params GlobalTargetInfo[] targets)
        {
            foreach (GlobalTargetInfo target in targets)
            {
                IntVec3 destination = this.pawn.Position + ((target.Cell - this.pawn.Position).ToVector3().normalized * 2).ToIntVec3();
                target.Thing.TryGetComp<CompCanBeDormant>()?.WakeUp();
                if (target.Thing is Pawn)
                {
                    AbilityPawnFlyer flyer = (AbilityPawnFlyer)PawnFlyer.MakeFlyer(VFE_DefOf_Abilities.VFEA_AbilityFlyer, target.Thing as Pawn, destination, null, null);
                    flyer.ability = this;
                    flyer.target = destination;
                    GenSpawn.Spawn(flyer, target.Cell, this.pawn.Map);
                }
                else
                {
                    target.Thing.Position = destination;
                }
            }
        }

        public override void CheckCastEffects(GlobalTargetInfo[] targetsInfos, out bool cast, out bool target, out bool hediffApply)
        {
            base.CheckCastEffects(targetsInfos, out cast, out target, out _);
            hediffApply = false;
        }
	}
}
