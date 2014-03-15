using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace HexStrategy
{
	public class Cube
	{
		float rotation = 0.0f;
		Vector3 position = Vector3.Zero;

		public void Draw()
		{
			// Copy any parent transforms.
			Matrix[] transforms = new Matrix[Meshes.cube.Bones.Count];
			Meshes.cube.CopyAbsoluteBoneTransformsTo(transforms);

			foreach (ModelMesh mesh in Meshes.cube.Meshes)
			{

				foreach (BasicEffect effect in mesh.Effects)
				{
					effect.EnableDefaultLighting();
					effect.World = transforms[mesh.ParentBone.Index] * 
						Matrix.CreateRotationY(rotation)
						* Matrix.CreateTranslation(position);
					effect.View = Core.camera.view;
					effect.Projection = Core.camera.projection;
					effect.Texture = Textures.green;
					effect.TextureEnabled = true;
					effect.PreferPerPixelLighting = true;
				}
				// Draw the mesh, using the effects set above.
				mesh.Draw();
			}
		}

		public void DrawShader()
		{


		}
	}
}

