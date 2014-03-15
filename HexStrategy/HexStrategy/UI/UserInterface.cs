using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexStrategy
{
	public static class UserInterface
	{

		public static HexDetails hexDetails;

		public static Color grey = new Color(100,100,100,100);
		public static Color fontColor = new Color(255,255,255,200);

		public static int textSpacing = 15;
		public static int textMargin = 10;

		public static void LoadElements()
		{
			hexDetails = new HexDetails ();
		}

		public static void Update(GameTime gameTime)
		{
			hexDetails.Update (gameTime);

		}

		public static void Draw(SpriteBatch sb)
		{
			hexDetails.Draw (sb);
		}
	}
}

