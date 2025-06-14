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
    [StaticConstructorOnStartup]
    public static class OuterRimCoreStartup
    {
        static OuterRimCoreStartup()
        {
            OuterRimCoreMod.mod.UpdateSoundSettings();
        }
    }
}
