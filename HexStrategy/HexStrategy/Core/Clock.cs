using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HexStrategy
{
    public enum ClockState
    {
        Running, Paused
    }
    public static class Clock
    {
        public static float seconds = 5f;
        public static ClockState clockState = ClockState.Running;

        public static float timeCompression = 2f;

        public static DateTime dateTime = new DateTime(1200, 1, 1);
        public static int lastDayMonth = dateTime.Month;
        public static int lastDayYear = dateTime.Year;

        public static void Update(GameTime gameTime)
        {
            
            seconds -= (float)gameTime.ElapsedGameTime.TotalSeconds * timeCompression;

            if (seconds < 0)
            {
                seconds = 5f;
                DayTick();    
            }
        }

        private static void DayTick()
        {
            if (clockState != ClockState.Running)
                return;

            foreach (Faction faction in Core.factions)
                faction.DayTick();

            dateTime = dateTime.AddDays(1);

            if (dateTime.Month != lastDayMonth)
            {
                //Advanced a month
                foreach (Faction faction in Core.factions)
                    faction.MonthTick();
            }
            if (dateTime.Year != lastDayYear)
            {
                //Advanced a year
                foreach (Faction faction in Core.factions)
                    faction.YearTick();

            }

            lastDayMonth = dateTime.Month;
            lastDayYear = dateTime.Year;
        }

        public static void Pause()
        {
            timeCompression = 0f;
            clockState = ClockState.Paused;
        }

        public static void Unpause()
        {
            timeCompression = 0.5f;
            clockState = ClockState.Running;
        }
        
    }
}
