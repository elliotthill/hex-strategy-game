using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace HexStrategy
{

    public static class Logger
    {
        public static List<String> Log = new List<string>();

        public static void Draw(SpriteBatch sb)
        {
            int i = 0;
            foreach (String str in Log)
            {
                sb.DrawString(Fonts.small, str, new Vector2(0, 100 + (i * 20)), Color.White);
                i++;
            }

        }


        public static void AddMessage(String Message)
        {
            Log.Add(Message);
            if (Log.Count() > 25)
            {
                Log.RemoveAt(0);
                Log.TrimExcess();
            }


        }

    }
}
