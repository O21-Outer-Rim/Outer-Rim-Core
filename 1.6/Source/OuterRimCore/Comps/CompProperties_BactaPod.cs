using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OuterRimCore
{
    public class CompProperties_BactaPod : CompProperties
    {
        public CompProperties_BactaPod()
        {
            compClass = typeof(Comp_BactaPod);
        }

        public SoundDef enterSound;
        public SoundDef exitSound;
        public SoundDef runningSound;
        public EffecterDef effecter;

        public float bactaPerCycle = 0.1f;
        public int ticksPerCycle = 120;
        public float healingPerCycle = 0.1f;
    }
}
