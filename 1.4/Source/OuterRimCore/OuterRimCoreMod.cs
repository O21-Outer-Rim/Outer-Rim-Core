using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OuterRimCore
{
    public class OuterRimCoreMod : Mod
    {
        public static OuterRimCoreMod mod;
        public static OuterRimCoreSettings settings;

        public Vector2 optionsScrollPosition;
        public float optionsViewRectHeight;

        internal static string VersionDir => Path.Combine(ModLister.GetActiveModWithIdentifier("Neronix17.OuterRim.Core").RootDir.FullName, "Version.txt");
        public static string CurrentVersion { get; private set; }

        public OuterRimCoreMod(ModContentPack content) : base(content)
        {
            mod = this;
            settings = GetSettings<OuterRimCoreSettings>();

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            CurrentVersion = $"{version.Major}.{version.Minor}.{version.Build}";

            LogUtil.LogMessage($"{CurrentVersion} ::");

            if (Prefs.DevMode)
            {
                File.WriteAllText(VersionDir, CurrentVersion);
            }

            Harmony OuterRimHarmony = new Harmony("Neronix17.OuterRim.Core");
            OuterRimHarmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override string SettingsCategory() => "Outer Rim";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            bool flag = optionsViewRectHeight > inRect.height;
            Rect viewRect = new Rect(inRect.x, inRect.y, inRect.width - (flag ? 26f : 0f), optionsViewRectHeight);
            Widgets.BeginScrollView(inRect, ref optionsScrollPosition, viewRect);
            Listing_Standard listing = new Listing_Standard();
            Rect rect = new Rect(viewRect.x, viewRect.y, viewRect.width, 999999f);
            listing.Begin(rect);
            // ============================ CONTENTS ================================
            DoOptionsCategoryContents(listing);
            // ======================================================================
            optionsViewRectHeight = listing.CurHeight;
            listing.End();
            Widgets.EndScrollView();
        }

        public void DoOptionsCategoryContents(Listing_Standard listing)
        {
            listing.Note("Some settings may require a game restart to take effect.", GameFont.Tiny);
            listing.GapLine();
            listing.ValueLabeled("Big Threat Letter", "Changes the sound effect played during a large threat, like a raid.", ref settings.soundPack_bigThreat);

            UpdateSoundSettings();
        }

        public SoundDef original_threatBig;

        public void UpdateSoundSettings()
        {
            // ====== Big Threat Sound Replacer =======
            if (original_threatBig == null)
            {
                original_threatBig = LetterDefOf.ThreatBig.arriveSound;
            }

            switch (settings.soundPack_bigThreat)
            {
                case OuterRimSoundSetting_BigThreat.Vanilla:
                    LetterDefOf.ThreatBig.arriveSound = original_threatBig;
                    break;
                case OuterRimSoundSetting_BigThreat.Rebel:
                    LetterDefOf.ThreatBig.arriveSound = OuterRimCoreDefOf.OuterRim_Rebel_ThreatBig;
                    break;
                case OuterRimSoundSetting_BigThreat.Imperial:
                    LetterDefOf.ThreatBig.arriveSound = OuterRimCoreDefOf.OuterRim_Imperial_ThreatBig;
                    break;
                default:
                    LetterDefOf.ThreatBig.arriveSound = original_threatBig;
                    break;
            }
        }
    }
}
