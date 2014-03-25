using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace HexStrategy
{
	public static class Fonts
	{
		public static SpriteFont small,large, ultra, medium;


		public static void LoadContent(ContentManager content)
		{
			small = content.Load<SpriteFont>("Fonts/Georgia10pt");
            medium = content.Load<SpriteFont>("Fonts/Georgia12pt");
            large = content.Load<SpriteFont>("Fonts/Georgia14pt");
            ultra = content.Load<SpriteFont>("Fonts/Georgia16pt");

            large.Spacing = 1.3f;
            medium.Spacing = 1.2f;
            ultra.Spacing = 1.4f;
		}

	}
}

