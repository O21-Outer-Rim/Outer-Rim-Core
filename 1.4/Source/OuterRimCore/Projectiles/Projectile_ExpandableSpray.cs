using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using VFECore;

namespace OuterRimCore
{
    public class Projectile_ExpandableSpray : ExpandableProjectile
	{
		public override void DoDamage(IntVec3 pos)
		{
			base.DoDamage(pos);
			try
			{
				if (!(pos != launcher.Position) || launcher.Map == null || !pos.InBounds(launcher.Map))
				{
					return;
				}
				List<Thing> list = launcher.Map.thingGrid.ThingsListAt(pos);
				for (int num = list.Count - 1; num >= 0; num--)
				{
					if (IsDamagable(list[num]))
					{
						customImpact = true;
						Impact(list[num]);
						customImpact = false;
					}
				}
			}
			catch
			{
			}
		}

		public override bool IsDamagable(Thing t)
		{
			return base.IsDamagable(t) && t.def != ThingDefOf.Fire;
		}
	}
}
