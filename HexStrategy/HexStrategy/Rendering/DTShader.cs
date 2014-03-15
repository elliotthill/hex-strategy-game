using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace HexStrategy
{
	/// <summary>
	/// Basic draw. A wrapper around Shaders/Normal.mgfx. 
	/// Diffuse Spec Tex Normal 
	/// </summary>
	public class DTShader
	{

		Effect effect;
		// Parameters for our shader object
		EffectParameter projectionParameter;
		EffectParameter viewParameter;
		EffectParameter worldParameter;

		EffectParameter ambientIntensityParameter;
		EffectParameter ambientColorParameter;

		// new parameters for diffuse light
		EffectParameter diffuseIntensityParameter;
		EffectParameter diffuseColorParameter;
		EffectParameter lightDirectionParameter;

		EffectParameter colorMapTextureParameter;


		public DTShader()
		{
			effect = Shaders.dtShader;

			worldParameter = effect.Parameters["World"];
			viewParameter = effect.Parameters["View"];
			projectionParameter = effect.Parameters["Projection"];

			ambientColorParameter = effect.Parameters["AmbientColor"];
			ambientIntensityParameter = effect.Parameters["AmbientIntensity"];

			diffuseColorParameter = effect.Parameters["DiffuseColor"];
			diffuseIntensityParameter = effect.Parameters["DiffuseIntensity"];
			lightDirectionParameter = effect.Parameters["DiffuseDirection"];


			colorMapTextureParameter = effect.Parameters["ColorMap"];


		}

		public void DrawVertices()
		{

		}

		public void Draw(Vector3 pos, Model model, Texture texture, float scale, float alpha, float rotation = 0f) {


			if (model == null)
				return;
			Matrix[] transforms = new Matrix[model.Bones.Count];
			model.CopyAbsoluteBoneTransformsTo(transforms);

			foreach (ModelMesh mesh in model.Meshes) {

				foreach (ModelMeshPart meshPart in mesh.MeshParts) {

					worldParameter.SetValue(Matrix.CreateScale(scale)* transforms[mesh.ParentBone.Index] * 
						Matrix.CreateRotationY(rotation) * Matrix.CreateTranslation(pos));
					viewParameter.SetValue(Core.camera.view);
					projectionParameter.SetValue(Core.camera.projection);

					ambientColorParameter.SetValue(Core.ambientLight);
					ambientIntensityParameter.SetValue(Core.ambientIntensity);

					diffuseColorParameter.SetValue(Core.sunDiffuse);

					diffuseIntensityParameter.SetValue(Core.diffuseIntensity * alpha);
					lightDirectionParameter.SetValue(Core.sunDirection);

					colorMapTextureParameter.SetValue(texture);


					//set the vertex source to the mesh's vertex buffer
					//had to remove second argument , meshPart.VertexOffset
					Core.graphicsDevice.SetVertexBuffer(meshPart.VertexBuffer);

					//set the current index buffer to the sample mesh's index buffer
					Core.graphicsDevice.Indices = meshPart.IndexBuffer;

					effect.CurrentTechnique = effect.Techniques["Technique1"];

					for (int i = 0; i < effect.CurrentTechnique.Passes.Count; i++)
					{
						//EffectPass.Apply will update the device to
						//begin using the state information defined in the current pass
						effect.CurrentTechnique.Passes[i].Apply();

						//theMesh contains all of the information required to draw
						//the current mesh
						effect.GraphicsDevice.DrawIndexedPrimitives(
							PrimitiveType.TriangleList, 0, 0,
							meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);


					}

				}

			}


		}
	}
}

	