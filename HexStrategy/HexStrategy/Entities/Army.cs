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

		private Hex hex;

		public Army(Hex hex)
		{
			this.hex = hex;
		}

		public void Move(Hex hex)
		{
			this.hex = hex;
		}

		public void Draw3D()
		{
			if (Core.camera.Visible(this.hex.position))
				Core.dtShader.Draw (hex.position, Meshes.hex, Textures.DarkBrown,1f, 1f, 0f);
		}

		public void Draw2D()
		{
            
		}

	}
}

