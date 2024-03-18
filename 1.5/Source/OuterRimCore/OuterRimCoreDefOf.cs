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

        public static JobDef OuterRim_FrozenInCarbonite;
        public static ThingDef OuterRim_JetpackJump_Flyer;

        /// <summary>
        /// Imperial Sound Effects
        /// </summary>
        // Raid Alert
        public static SoundDef OuterRim_Imperial_ThreatBig;

        /// <summary>
        /// Rebel Sound Effects
        /// </summary>
        // Raid Alert
        public static SoundDef OuterRim_Rebel_ThreatBig;

        /// <summary>
        /// Vanilla DefOfs needed that do not exist.
        /// </summary>
        public static RecordDef Kills;
        public static RecordDef PawnsDowned;
        public static StatDef MaxHitPoints;
    }
}
