using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HexStrategy
{

    public static class Clock
    {
        public static float seconds = 6f;

        public static int year = 1588;
        public static int days = 0;
        public static float timeCompression = 0.5f;

        public static void Update(GameTime gameTime)
        {
            seconds -= (float)gameTime.ElapsedGameTime.TotalSeconds * timeCompression;

            if (seconds < 0)
            {
                seconds = 6f;
                DayTick();    
            }
        }

        private static void DayTick()
        {
            foreach (Faction faction in Core.factions)
                faction.DayTick();

            days += 1;
        }


        
    }
}
