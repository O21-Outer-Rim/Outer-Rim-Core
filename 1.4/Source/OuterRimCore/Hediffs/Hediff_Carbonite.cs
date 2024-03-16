using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using TabulaRasa;

namespace OuterRimCore
{
    public class Hediff_Carbonite : Hediff_VisualOverlay
    {
        public Material material;

        public Material Material
        {
            get
            {
                if (material == null) { material = MaterialPool.MatFrom("OuterRim/Overlays/CarboniteFreezing", ShaderDatabase.Transparent); material.color = Color.grey; }
                return material;
            }
        }
        public override string OverlayPath => "OuterRim/Overlays/CarboniteFreezing";

        public override Shader OverlayShader => ShaderDatabase.Transparent;

        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            IntVec3 facing = pawn.Rotation.FacingCell;
            int tickCount = this.TryGetComp<HediffComp_Disappears>().ticksToDisappear;
            Job job = JobMaker.MakeJob(OuterRimCoreDefOf.OuterRim_FrozenInCarbonite);
            job.overrideFacing = pawn.Rotation;
            job.expiryInterval = tickCount;
            pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            pawn.pather.StopDead();
            pawn.stances.SetStance(new Stance_Frozen(tickCount, facing, null));
        }

        public override void Draw()
        {
            Vector3 drawPos = pawn.DrawPos;
            drawPos.y = AltitudeLayer.MoteOverhead.AltitudeFor();
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(drawPos, Quaternion.identity, new Vector3(1.5f, 1f, 1.5f));
            Graphics.DrawMesh(MeshPool.plane10, matrix, Material, 0, null, 0, MatPropertyBlock);
        }
    }
}
