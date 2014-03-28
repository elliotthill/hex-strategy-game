using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HexStrategy
{
    public enum DiplomacyType
    {
        None, Allied, War
    }

    public class Diplomacy
    {
        //The target of the relationship
        private Faction target;
        public int targetId;

        //The owning side of the relationship
        private Faction source;
        public int sourceId;

        public DiplomacyType diplomacyType = DiplomacyType.None;

        public Diplomacy()
        {
        }

        public Diplomacy(Faction source, Faction target)
        {
            this.target = target;
            this.source = source;

            this.targetId = Core.factions.IndexOf(target);
            this.sourceId = Core.factions.IndexOf(source);
        }


        //Called from faction.reconstruct
        public void Reconstruct()
        {
            this.target = Core.factions[targetId];
            this.source = Core.factions[sourceId];
        }

        public Faction getSourceFaction()
        {
            return source;
        }

        public Faction getTargetFaction()
        {
            return target;
        }

    }
}
