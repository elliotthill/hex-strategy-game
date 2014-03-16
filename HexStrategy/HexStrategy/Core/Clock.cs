using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HexStrategy
{

    public static class Clock
    {
        public static float seconds = 10f;

        public static int year = 1588;
        public static float timeCompression = 5f;

        public static void Update(GameTime gameTime)
        {
            seconds -= (float)gameTime.ElapsedGameTime.TotalSeconds * timeCompression;

            if (seconds < 0)
            {
                seconds = 10f;
                UpdateDaily();    
            }
        }

        private static void UpdateDaily()
        {
            foreach (Faction faction in Core.factions)
                faction.UpdateDaily();
        }


        
    }
}
