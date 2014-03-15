using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace HexStrategy
{
	public static class Meshes
	{
		public static Model cube, hex, hexTop, hexTopSide, plane, castle, tree, mountain;

		public static Vector3 castleTransform = new Vector3 (0, 0.3f, -0.2f);

		public static void LoadContent(ContentManager content)
		{
			cube = content.Load<Model> ("Meshes/hex");

			//Hex with all sides, hex with only top, hex with top and sides
            hex = content.Load<Model>("Meshes/hex");
            hexTop = content.Load<Model>("Meshes/hexTop");
            hexTopSide = content.Load<Model>("Meshes/hexTopSide");

            plane = content.Load<Model>("Meshes/plane");
            castle = content.Load<Model>("Meshes/castle");
            tree = content.Load<Model>("Meshes/tree");
            mountain = content.Load<Model>("Meshes/mountain");
		}

		public static Vector3 getModelTransform(String modelName)
		{
			if (modelName == "castle")
				return castleTransform;

			return Vector3.Zero;
		}

	}
}

