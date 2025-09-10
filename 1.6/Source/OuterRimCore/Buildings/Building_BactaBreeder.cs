using PipeSystem;
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
    public class Building_BactaBreeder : Building
    {
        public const int tickCost = 30000;
        public const float output = 5f;

        public static readonly Material BarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(1f, 1f, 1f));
        public static readonly Material BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0f, 0f, 0f, 0f));

        public CompPowerTrader powerComp;

        public CompResourceTrader resourceComp;

        public int curTick = -1;

        public bool Powered => powerComp == null || powerComp.PowerOn;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            powerComp = this.TryGetComp<CompPowerTrader>();
            resourceComp = this.TryGetComp<CompResourceTrader>();

            curTick = tickCost;
        }

        public override void Tick()
        {
            base.Tick();
            if (resourceComp != null && Powered)
            {
                curTick++;
                if (curTick >= tickCost)
                {
                    resourceComp.PipeNet.DistributeAmongStorage(output, out var _);
                    curTick = 0;
                }
            }
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);
            GenDraw.FillableBarRequest r = new GenDraw.FillableBarRequest()
            {
                center = drawLoc + new Vector3(0f, 1f, 0f),
                size = new Vector2(1.5f, 0.25f),
                fillPercent = curTick / tickCost,
                filledMat = BarFilledMat,
                unfilledMat = BarUnfilledMat,
                margin = 0.5f,
                rotation = Rot4.East
            };
            GenDraw.DrawFillableBar(r);
        }
    }
}
