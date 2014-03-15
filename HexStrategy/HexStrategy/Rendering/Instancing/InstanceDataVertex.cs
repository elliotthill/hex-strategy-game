using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexStrategy
{
    public struct InstanceDataVertex
    {
        public Matrix World;
        public Color Colour;

        public InstanceDataVertex(Matrix world, Color colour)
        {
            World = world;
            Colour = colour;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
            (
            // World Matrix Data
                 new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 5),
                 new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 6),
                 new VertexElement(sizeof(float) * 8, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 7),
                 new VertexElement(sizeof(float) * 12, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 8),
            //Colour Data
                 new VertexElement(sizeof(float) * 16, VertexElementFormat.Color, VertexElementUsage.Color, 0)

            );
    }
}
