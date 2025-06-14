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
	public class CompProperties_ResourceExtractor : CompProperties
	{
		public CompProperties_ResourceExtractor()
		{
			compClass = typeof(Comp_ResourceExtractor);
		}

		public int ticksPerPortion = 60;
		public SoundDef ambientSound;
	}
}
