using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace HexStrategy
{
    public static class Render
    {
        static DynamicVertexBuffer instanceVertexBuffer;

        static Effect effect;
        // Parameters for our shader object
        static EffectParameter projectionParameter;
        static EffectParameter viewParameter;

        static EffectParameter ambientColorParameter;

        // new parameters for diffuse light
        static EffectParameter diffuseIntensityParameter;
        static EffectParameter diffuseColorParameter;
        static EffectParameter lightDirectionParameter;

        static EffectParameter colorMapTextureParameter;

        //Global world matrix
        static Matrix world = Matrix.Identity;

        public static void Initialize()
        {
            effect = Shaders.instanceShader;


            viewParameter = effect.Parameters["View"];
            projectionParameter = effect.Parameters["Projection"];

            ambientColorParameter = effect.Parameters["AmbientColor"];

            diffuseColorParameter = effect.Parameters["DiffuseColor"];
            diffuseIntensityParameter = effect.Parameters["DiffuseIntensity"];
            lightDirectionParameter = effect.Parameters["DiffuseDirection"];


            colorMapTextureParameter = effect.Parameters["ColorMap"];
        }

        public static void setWorld(Matrix newWorld)
        {
            world = newWorld;
        }

        public static void DrawInstances(List<Hex> hexes, Model model, Texture2D texture)
        {

            if (hexes.Count() < 1)
                return;

            InstanceDataVertex[] data = new InstanceDataVertex[hexes.Count()];

            for (int i = 0; i < hexes.Count(); i++)
            {
                if (hexes[i] == Core.map.selectedHex)
                {

                    data[i] = new InstanceDataVertex(world * hexes[i].world, Color.LightGray);
                }
                else
                    data[i] = new InstanceDataVertex(world * hexes[i].world, hexes[i].hexData.color);

                //If using world matrix override (e.g. for castle model)
                

            }

            #region Grow vertex buffer if neccessary
            // If we have more instances than room in our vertex buffer, grow it to the neccessary size.
            if ((instanceVertexBuffer == null) ||
                (data.Length > instanceVertexBuffer.VertexCount))
            {
                if (instanceVertexBuffer != null)
                    instanceVertexBuffer.Dispose();

                instanceVertexBuffer = new DynamicVertexBuffer(Core.graphicsDevice, InstanceDataVertex.VertexDeclaration,
                                                               data.Length, BufferUsage.WriteOnly);
            }
            #endregion

            // Transfer the latest instance transform matrices into the instanceVertexBuffer.
            instanceVertexBuffer.SetData(data, 0, data.Length, SetDataOptions.Discard);
            #region Draw instances
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
                    Core.graphicsDevice.SetVertexBuffers(
                        new VertexBufferBinding(meshPart.VertexBuffer, meshPart.VertexOffset, 0),
                        new VertexBufferBinding(instanceVertexBuffer, 0, 1)
                    );

                    Core.graphicsDevice.Indices = meshPart.IndexBuffer;

                    meshPart.Effect = Shaders.instanceShader;
                    // Set up the instance rendering effect.
                    Effect effect = meshPart.Effect;
                    
                    viewParameter.SetValue(Core.camera.view);
                    projectionParameter.SetValue(Core.camera.projection);

                    ambientColorParameter.SetValue(Core.ambientLight);
                    

                    diffuseColorParameter.SetValue(Core.sunDiffuse);

                    diffuseIntensityParameter.SetValue(Core.diffuseIntensity);
                    lightDirectionParameter.SetValue(Core.sunDirection);

                    colorMapTextureParameter.SetValue(texture);
                    effect.CurrentTechnique = effect.Techniques["HardwareInstanceLow"];

                    // Draw all the instance copies in a single call.
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        Core.graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                               meshPart.NumVertices, meshPart.StartIndex,
                                                               meshPart.PrimitiveCount, data.Length);
                    }
                }

            }



            #endregion

        }
    }
}
