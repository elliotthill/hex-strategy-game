using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexStrategy
{
	public class Army
	{

		public Vector3 position;
		private Hex hex;

		public Army(Hex hex)
		{
			this.position = hex.position;
			this.hex = hex;
		}

		public void Move(Hex hex)
		{
			this.hex = hex;
			this.position = hex.position;
		}

		public void Draw3D()
		{
			if (Core.camera.Visible(this.position))
				Core.dtShader.Draw (this.position, Meshes.cube, Textures.DarkBrown, 0.75f, 1f, 0f);
		}

		public void Draw2D()
		{

		}

	}
}

