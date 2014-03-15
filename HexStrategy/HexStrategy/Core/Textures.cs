using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace HexStrategy
{
	public static class Textures
	{
		public static Texture2D blue, green, lightBlue, lightGreen,halfRed, yellow, snow,tree, white, DarkGreen, DarkBrown, heightMap;

		public static void LoadContent(ContentManager content)
		{
			blue = content.Load<Texture2D> ("Textures/blue");
			green = content.Load<Texture2D> ("Textures/green");
			DarkGreen = content.Load<Texture2D> ("Textures/dark_green");
			DarkBrown = content.Load<Texture2D> ("Textures/dark_brown");
			lightBlue = content.Load<Texture2D> ("Textures/light_blue");
			lightGreen = content.Load<Texture2D> ("Textures/light_green");
			yellow = content.Load<Texture2D> ("Textures/yellow");
			snow = content.Load<Texture2D> ("Textures/snow");
			white = content.Load<Texture2D> ("Textures/white");
			tree = content.Load<Texture2D> ("Textures/tree");
			halfRed = content.Load<Texture2D> ("Textures/half_red");

			heightMap = content.Load<Texture2D> ("Textures/height_earth");
		}

	}
}

