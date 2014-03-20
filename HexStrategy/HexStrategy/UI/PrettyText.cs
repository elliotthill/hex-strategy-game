using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace HexStrategy
{
    enum TextAnchor
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }

    class PrettyText
    {
        private string _text;
        private Vector2 _origin;
        private SpriteFont _font;
        private Vector2 _position;
        private Color clr;


        public PrettyText(SpriteFont font, string text, Vector2 position)
            : this(font, text, position, TextAnchor.TopCenter, Color.White)
        {
        }

        public PrettyText(SpriteFont font, string text, Vector2 position, TextAnchor anchor, Color color)
        {
            _font = font;

            _text = text;

            _position = position;
            _origin = Vector2.Zero;
            clr = color;

            Vector2 textSize = font.MeasureString(_text);

            if (anchor == TextAnchor.TopCenter ||
                anchor == TextAnchor.MiddleCenter ||
                anchor == TextAnchor.BottomCenter)
            {
                _origin.X = textSize.X / 2;
            }
            else if (anchor == TextAnchor.TopRight ||
                     anchor == TextAnchor.MiddleRight ||
                     anchor == TextAnchor.BottomRight)
            {
                _origin.X = textSize.X;
            }

            if (anchor == TextAnchor.MiddleLeft ||
                anchor == TextAnchor.MiddleCenter ||
                anchor == TextAnchor.MiddleRight)
            {
                _origin.Y = textSize.Y / 2;
            }
            else if (anchor == TextAnchor.BottomLeft ||
                     anchor == TextAnchor.BottomCenter ||
                     anchor == TextAnchor.BottomRight)
            {
                _origin.Y = textSize.Y;
            }
        }

        // Make sure the caller of this method calls 
        // Begin and End on the SpriteBatch before and 
        // after calling this method, respectively. 
        public void Draw(SpriteBatch batch)
        {

            batch.DrawString(_font, _text, _position, clr, 0f, new Vector2((int)_origin.X, (int)_origin.Y), 1f, SpriteEffects.None, 0f);

        }
        public void DrawWithShadow(SpriteBatch batch)
        {
            batch.DrawString(_font, _text, new Vector2((int)_position.X + 1,(int) _position.Y - 1), UserInterface.shadow, 0f, new Vector2((int)_origin.X, (int)_origin.Y), 1, SpriteEffects.None, 0f);
            batch.DrawString(_font, _text, new Vector2((int)_position.X - 1, (int)_position.Y + 1), UserInterface.shadow, 0f, new Vector2((int)_origin.X, (int)_origin.Y), 1, SpriteEffects.None, 0f);
            batch.DrawString(_font, _text, _position, clr, 0f, new Vector2((int)_origin.X, (int)_origin.Y), 1f, SpriteEffects.None, 0f);

        }
    }
}
