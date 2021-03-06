﻿using System;
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
        public static ArmyDetails armyDetails;
        public static FactionDetails factionDetails;

		public static Color grey = new Color(100,100,100,155);
        public static Color transparentBlack = new Color(0, 0, 0, 100);
        public static Color transparentWhite = new Color(255, 255, 255, 100);
		public static Color fontColor = new Color(255,255,255,200);
        public static Color shadow = new Color(20, 20, 20, 100);
		public static int textSpacing = 15;
		public static int textMargin = 10;
        public static Boolean chatWindowActive = true;
        public static Boolean isMouseOverUI = false;
        public static Boolean isEditingTextField = false;

		public static void Load()
		{
			hexDetails = new HexDetails();
            armyDetails = new ArmyDetails();
            factionDetails = new FactionDetails();
            ChatConsole.LoadContent();
		}

		public static void Update(GameTime gameTime)
		{
            isMouseOverUI = false;

			hexDetails.Update(gameTime);
            armyDetails.Update(gameTime);
            factionDetails.Update(gameTime);
            ChatConsole.Update();
		}

		public static void Draw(SpriteBatch sb)
		{
			hexDetails.Draw(sb);
            armyDetails.Draw(sb);
            factionDetails.Draw(sb);
            //Logger.Draw(sb);
            ChatConsole.Draw(sb);
		}
	}
}

