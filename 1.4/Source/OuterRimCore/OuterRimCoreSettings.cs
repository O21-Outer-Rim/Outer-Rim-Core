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
    public class OuterRimCoreSettings : ModSettings
    {
        public bool verboseLogging = false;

        public OuterRimSoundSetting_BigThreat soundPack_bigThreat = OuterRimSoundSetting_BigThreat.Vanilla;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref soundPack_bigThreat, "soundPack", OuterRimSoundSetting_BigThreat.Vanilla);
        }

        public bool IsValidSetting(string input)
        {
            if (GetType().GetFields().Where(p => p.FieldType == typeof(bool)).Any(i => i.Name == input))
            {
                return true;
            }

            return false;
        }

        public IEnumerable<string> GetEnabledSettings
        {
            get
            {
                return GetType().GetFields().Where(p => p.FieldType == typeof(bool) && (bool)p.GetValue(this)).Select(p => p.Name);
            }
        }
    }
}
