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
	public class Comp_BactaPod : CompBiosculpterPod
	{
		public override void PostDraw()
		{
			//base.PostDraw();
			Rot4 rotation = parent.Rotation;
			Vector3 s = new Vector3(parent.def.graphicData.drawSize.x * 0.7f, 1f, parent.def.graphicData.drawSize.y * 0.7f);
			Vector3 drawPos = parent.DrawPos;
			drawPos.y -= 0.08108108f;
			Matrix4x4 matrix = default(Matrix4x4);
			matrix.SetTRS(drawPos, rotation.AsQuat, s);
			Graphics.DrawMesh(MeshPool.plane10, matrix, BackgroundMat, 0);
			if (State == BiosculpterPodState.Occupied)
			{
				Pawn occupant = Occupant;
				Vector3 drawLoc = parent.DrawPos + FloatingOffset();
				Rot4 rot = parent.Rotation;
				//if (rot == Rot4.East || rot == Rot4.West)
				//{
				//	drawLoc.z += 0.2f;
				//}
				if (rot == Rot4.South)
				{
					drawLoc.x -= 0.5f;
					drawLoc.z += 0.2f;
				}
				if (rot == Rot4.North)
				{
					drawLoc.x += 0.5f;
					drawLoc.z += 0.2f;
				}
				if (rot == Rot4.East)
				{
					drawLoc.z -= 0.3f;
				}
				if (rot == Rot4.West)
				{
					drawLoc.z += 0.7f;
				}
				occupant.Drawer.renderer.RenderPawnAt(drawLoc, null, true);
			}
		}
	}
}
