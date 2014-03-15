using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexStrategy
{
	public class Scenery
	{
	
		public void Draw()
		{
			// Copy any parent transforms.
			Matrix[] transforms = new Matrix[Meshes.plane.Bones.Count];
			Meshes.plane.CopyAbsoluteBoneTransformsTo(transforms);

			foreach (ModelMesh mesh in Meshes.plane.Meshes)
			{

				foreach (BasicEffect effect in mesh.Effects)
				{
					effect.EnableDefaultLighting();
					effect.AmbientLightColor = new Vector3(1.4f,1.4f,1.4f);
					effect.DirectionalLight0.Enabled = true;
					effect.DirectionalLight0.Direction = Core.sunDirection;

					effect.DirectionalLight0.DiffuseColor = new Vector3(Core.sunDiffuse.X, Core.sunDiffuse.Y, Core.sunDiffuse.Z);
					effect.DirectionalLight1.Enabled = false;
					effect.DirectionalLight2.Enabled = false;
					effect.World = Matrix.CreateScale(500f)
						* Matrix.CreateTranslation(new Vector3(100f,0f,100f));
					effect.View = Core.camera.view;
					effect.Projection = Core.camera.projection;
					effect.Texture = Textures.blue;
					effect.TextureEnabled = true;
				}
				// Draw the mesh, using the effects set above.
				mesh.Draw();
			}
		}
	}
}

