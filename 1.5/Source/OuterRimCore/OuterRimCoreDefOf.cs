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
    [DefOf]
    public static class OuterRimCoreDefOf
    {
        static OuterRimCoreDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(OuterRimCoreDefOf));
        }

        public static DamageDef 
            OuterRim_Ion;

        public static JobDef 
            OuterRim_FrozenInCarbonite;

        public static ThingDef 
            OuterRim_JetpackJump_Flyer;

        public static SoundDef 
            OuterRim_Imperial_ThreatBig,
            OuterRim_Rebel_ThreatBig;

        public static RecordDef 
            Kills, 
            PawnsDowned;

        public static StatDef 
            MaxHitPoints;
    }
}
