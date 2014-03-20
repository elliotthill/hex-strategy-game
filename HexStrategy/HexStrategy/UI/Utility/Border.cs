using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace HexStrategy
{
    public static class Border
    {

        public static void Draw(Rectangle bounds, SpriteBatch sb)
        {
            //Left edge to bottom
            sb.Draw(Textures.DarkBrown, new Rectangle(bounds.X - 1, bounds.Y - 1, 1, bounds.Height + 2), UserInterface.transparentBlack);
            sb.Draw(Textures.white, new Rectangle(bounds.X - 2, bounds.Y - 2, 1, bounds.Height + 4), UserInterface.grey);

            //Left edge to right
            sb.Draw(Textures.DarkBrown, new Rectangle(bounds.X - 1, bounds.Y - 1, bounds.Width + 2, 1), UserInterface.transparentBlack);
            sb.Draw(Textures.white, new Rectangle(bounds.X - 2, bounds.Y - 2, bounds.Width + 4, 1), UserInterface.grey);

            //Bottom left edge to right
            sb.Draw(Textures.DarkBrown, new Rectangle(bounds.X - 1, bounds.Y + bounds.Height, bounds.Width + 2, 1), UserInterface.transparentBlack);
            sb.Draw(Textures.white, new Rectangle(bounds.X - 2, bounds.Y + bounds.Height + 1, bounds.Width + 4, 1), UserInterface.grey);

            //Bottom right edge to top
            sb.Draw(Textures.DarkBrown, new Rectangle(bounds.X + bounds.Width, bounds.Y - 1, 1, bounds.Height + 2), UserInterface.transparentBlack);
            sb.Draw(Textures.white, new Rectangle(bounds.X + bounds.Width + 1, bounds.Y - 2, 1, bounds.Height + 4), UserInterface.grey);





        }
        public static void DrawWhite(Rectangle bounds, SpriteBatch sb)
        {
            //Left edge to bottom

            sb.Draw(Textures.white, new Rectangle(bounds.X - 2, bounds.Y - 2, 1, bounds.Height + 4), UserInterface.transparentWhite);

            //Left edge to right

            sb.Draw(Textures.white, new Rectangle(bounds.X - 2, bounds.Y - 2, bounds.Width + 4, 1), UserInterface.transparentWhite);

            //Bottom left edge to right

            sb.Draw(Textures.white, new Rectangle(bounds.X - 2, bounds.Y + bounds.Height + 1, bounds.Width + 4, 1), UserInterface.transparentWhite);

            //Bottom right edge to top

            sb.Draw(Textures.white, new Rectangle(bounds.X + bounds.Width + 1, bounds.Y - 2, 1, bounds.Height + 4), UserInterface.transparentWhite);




        }
    }
}
