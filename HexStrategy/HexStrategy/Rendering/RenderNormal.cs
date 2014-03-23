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
    public static class RenderNormal
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
        static EffectParameter specularColorParameter;
        static EffectParameter lightDirectionParameter;

        static EffectParameter colorMapTextureParameter;
        static EffectParameter normalMapTextureParameter;
        static EffectParameter eyePositionParameter;
        static EffectParameter opacityParameter;

        //Global world matrix
        static Matrix world = Matrix.Identity;

        public static void Initialize()
        {

            effect = Shaders.instanceNormalShader;
            //Bind params
            viewParameter = effect.Parameters["View"];
            projectionParameter = effect.Parameters["Projection"];

            ambientColorParameter = effect.Parameters["AmbientColor"];

            diffuseColorParameter = effect.Parameters["DiffuseColor"];
            diffuseIntensityParameter = effect.Parameters["DiffuseIntensity"];
            specularColorParameter = effect.Parameters["SpecularColor"];
            lightDirectionParameter = effect.Parameters["DiffuseDirection"];

            opacityParameter = effect.Parameters["opacity"];
            colorMapTextureParameter = effect.Parameters["ColorMap"];
            normalMapTextureParameter = effect.Parameters["NormalMap"];
            eyePositionParameter = effect.Parameters["EyePosition"];
        }

        public static void setWorld(Matrix newWorld)
        {
            world = newWorld;
        }

        public static void DrawInstances(List<Hex> hexes, Model model, Texture2D texture, Texture2D normal, float opacity = -1f, Boolean useFactionColor = false)
        {

            if (hexes.Count() < 1)
                return;

            InstanceDataVertex[] data = new InstanceDataVertex[hexes.Count()];

            for (int i = 0; i < hexes.Count(); i++)
            {

                if (hexes[i] == Core.map.selectedHex)
                {

                    data[i] = new InstanceDataVertex(world * hexes[i].GetWorld(), Color.LightGray);
                }
                else
                {
                    if (useFactionColor == true)
                    {
                        if (hexes[i].GetIsBorder())
                            data[i] = new InstanceDataVertex(world * hexes[i].GetWorld(), hexes[i].getOwner().borderColor);
                        else
                            data[i] = new InstanceDataVertex(world * hexes[i].GetWorld(), hexes[i].getOwner().color);
                    }
                    else
                        data[i] = new InstanceDataVertex(world * hexes[i].GetWorld(), hexes[i].hexData.color);
                }
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
                    specularColorParameter.SetValue(new Vector4(0.5f, 0.5f, 0.5f, 1f));
                    eyePositionParameter.SetValue(Core.camera.position);
                    colorMapTextureParameter.SetValue(texture);
                    normalMapTextureParameter.SetValue(normal);
                    opacityParameter.SetValue(opacity);

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
