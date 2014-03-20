using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexStrategy
{
    public class FactionDetails
    {
        protected Boolean IsVisible = true;
        public Rectangle bounds = new Rectangle(0, 0, Core.screenX, 30);
        public List<Rectangle> buttonBounds = new List<Rectangle>();
        //public Dictionary<Vector2> textBounds = new Dictionary<Vector2>();

        public Dictionary<String, Vector2> textBounds = new Dictionary<String, Vector2>();

        public FactionDetails()
        {
            //buttonBounds.Add (new Rectangle(bounds.X,bounds.Y,100,100));


            textBounds.Add("name", new Vector2(bounds.X + UserInterface.textMargin, bounds.Y + (UserInterface.textMargin)));
            textBounds.Add("money", new Vector2(bounds.X + (float)300 + UserInterface.textMargin * 2, bounds.Y + (UserInterface.textMargin)));

        }


        public void Update(GameTime gameTime)
        {
            if (Core.userFaction != null)
                IsVisible = true;
            else
                IsVisible = false;

            if (!IsVisible)
                return;
        }

        public void Draw(SpriteBatch sb)
        {
            if (!IsVisible)
                return;

            sb.Draw(Textures.DarkBrown, bounds, UserInterface.grey);

            foreach (Rectangle button in buttonBounds)
            {

                sb.Draw(Textures.white, button, Color.White);
            }

            if (Core.userFaction != null)
            {

                sb.DrawString(Fonts.large, Core.userFaction.name + " armies: " + Core.userFaction.armyList.Count, textBounds["name"], Color.White);
                sb.DrawString(Fonts.large, "$" + Core.userFaction.treasury, textBounds["money"], Color.White);
            }
        }


    }
}

