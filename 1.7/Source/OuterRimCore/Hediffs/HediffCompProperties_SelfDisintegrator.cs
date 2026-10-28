using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OuterRimCore
{
    public class HediffCompProperties_SelfDisintegrator : HediffCompProperties
    {
        public HediffCompProperties_SelfDisintegrator()
        {
            compClass = typeof(HediffComp_SelfDisintegrator);
        }

        public FleckDef fleck;

        public ThingDef mote;

        public int moteCount = 3;

        public FloatRange moteOffsetRange = new FloatRange(0.2f, 0.4f);

        public ThingDef filth;

        public int filthCount = 4;

        public HediffDef injuryCreatedOnDeath;

        public IntRange injuryCount;

        public SoundDef sound;

        public List<ThingDef> whitelist = new List<ThingDef>();

        public List<ThingDef> blacklist = new List<ThingDef>();
    }
}
