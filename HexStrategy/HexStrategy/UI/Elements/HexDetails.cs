using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexStrategy
{
	public class HexDetails
	{
		protected Boolean IsVisible = true;
		public Rectangle bounds = new Rectangle(Core.screenX-204,Core.screenY-300,200,300);
		public List<Rectangle> buttonBounds = new List<Rectangle>();

		//public Dictionary<Vector2> textBounds = new Dictionary<Vector2>();

		public Dictionary<String, Vector2> textBounds = new Dictionary<String, Vector2>();

		public HexDetails()
		{
			//buttonBounds.Add (new Rectangle(bounds.X,bounds.Y,100,100));


			textBounds.Add ("terrain", new Vector2 (bounds.X + UserInterface.textMargin, bounds.Y + (UserInterface.textSpacing)));
			textBounds.Add ("building", new Vector2 (bounds.X + UserInterface.textMargin, bounds.Y + (UserInterface.textSpacing * 2)));
			textBounds.Add ("population", new Vector2 (bounds.X + UserInterface.textMargin, bounds.Y + (UserInterface.textSpacing * 3)));
            textBounds.Add("income", new Vector2(bounds.X + UserInterface.textMargin, bounds.Y + (UserInterface.textSpacing * 4)));
            textBounds.Add ("owner", new Vector2(bounds.X + UserInterface.textMargin, bounds.Y + (UserInterface.textSpacing * 5)));
            textBounds.Add("wealth", new Vector2(bounds.X + UserInterface.textMargin, bounds.Y + (UserInterface.textSpacing * 6)));
            textBounds.Add("food", new Vector2(bounds.X + UserInterface.textMargin, bounds.Y + (UserInterface.textSpacing * 7)));
		}

		public void Update(GameTime gameTime)
		{
            if (Core.map.selectedHex != null)
                IsVisible = true;
            else
                IsVisible = false;

            if (!IsVisible)
                return;

            /*
             * Handle mouse hovering and clicking
             */


		}

		public void Draw(SpriteBatch sb)
		{
            if (!IsVisible)
                return;

			sb.Draw (Textures.DarkBrown, bounds, UserInterface.grey);
            Border.Draw(bounds, sb);

			foreach (Rectangle button in buttonBounds) {

				sb.Draw (Textures.white, button, Color.White);
			}

			if (Core.map.selectedHex != null) {

				sb.DrawString(Fonts.medium, "Terrain: " + Core.map.selectedHex.hexData.terrainType.ToString(),textBounds["terrain"],Color.White);
				sb.DrawString(Fonts.medium, "Building: " + Core.map.selectedHex.hexData.buildingType.ToString(),textBounds["building"],Color.White);
				sb.DrawString(Fonts.medium, "Population: " + Core.map.selectedHex.hexData.population.ToString(),textBounds["population"],Color.White);
                sb.DrawString(Fonts.medium, "Income: " + Core.Format(Core.map.selectedHex.hexData.taxRevenue), textBounds["income"], Color.White);
                sb.DrawString(Fonts.medium, "GDPc: " + Core.map.selectedHex.hexData.GDPc, textBounds["wealth"], Color.White);
                sb.DrawString(Fonts.medium, "Food: " + Core.map.selectedHex.hexData.agriculturalOutput, textBounds["food"], Color.White);

                if (Core.map.selectedHex.getOwner() != null)
                sb.DrawString(Fonts.medium, "Owner: " + Core.map.selectedHex.getOwner().name, textBounds["owner"], Color.White);
			}
		}


	}
}

