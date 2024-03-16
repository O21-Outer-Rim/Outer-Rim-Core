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
	public class CompProperties_BactaPod : CompProperties_BiosculpterPod
	{
		public CompProperties_BactaPod()
		{
			this.compClass = typeof(Comp_BactaPod);
		}
	}
}
