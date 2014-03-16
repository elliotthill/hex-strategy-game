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
			small = content.Load<SpriteFont>("Fonts/bitmapSmall");
            medium = content.Load<SpriteFont>("Fonts/bitmapMedium");
            large = content.Load<SpriteFont>("Fonts/bitmapBig");
            ultra = content.Load<SpriteFont>("Fonts/bitmapGiant");

            large.Spacing = 1.3f;
            medium.Spacing = 1.3f;
		}

	}
}

