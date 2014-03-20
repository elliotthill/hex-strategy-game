using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexStrategy
{
    public class ArmyDetails
    {
        protected Boolean IsVisible = true;
        public Rectangle bounds = new Rectangle(0, Core.screenY - 300, 200, 300);
        public List<Rectangle> buttonBounds = new List<Rectangle>();
        public List<String> buttonLabels = new List<String>();
        //public Dictionary<Vector2> textBounds = new Dictionary<Vector2>();

        public Dictionary<String, Vector2> textBounds = new Dictionary<String, Vector2>();

        public ArmyDetails()
        {
            buttonBounds.Add (new Rectangle(bounds.X,bounds.Y,100,20));
            buttonLabels.Add("Annex");

            textBounds.Add("owner", new Vector2(bounds.X + UserInterface.textMargin, bounds.Y + (UserInterface.textSpacing)));

        }

        public void Update(GameTime gameTime)
        {
            if (Core.map.selectedArmy != null)
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

            if (Core.map.selectedArmy != null)
            {
                sb.DrawString(Fonts.medium, "Owner: " + Core.map.selectedArmy.GetOwner().name, textBounds["owner"], Color.White);

            }

            
            
        }


    }
}

