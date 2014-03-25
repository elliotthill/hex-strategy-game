using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HexStrategy
{
    class AIController
    {

        private Faction faction;


        public AIController(Faction faction)
        {
            this.faction = faction;
        }

        public void Update()
        {

        }

        public void DayTick()
        {
            AnnexEmpty();
        }

        public void MonthTick()
        {
        }

        public void YearTick()
        {
        }

        private List<Army> GetIdleArmies()
        {
            List<Army> idle = new List<Army>();

            foreach (Army army in this.faction.armyList)
            {
                if (army.IsSieging() == false)
                    idle.Add(army);
            }
            return idle;
        }

        private Army GetIdleArmy()
        {
            foreach (Army army in this.faction.armyList)
            {
                if (army.IsSieging() == false)
                    return army;
            }

            return null;
        }


        private void AnnexEmpty()
        {
            //First check whether our army is already sieging
            if (this.GetIdleArmy() != null && this.GetIdleArmy().IsSieging())
                return;

            //Iterate over border neighbours until we find one
            foreach (Hex border in this.faction.GetBorders())
            {
                foreach (Hex neighbour in Core.map.FindNeighbours(border))
                {
                    if (neighbour.getOwner() == null && neighbour.IsNotWater() && this.GetIdleArmy() != null)
                    {
                        this.GetIdleArmy().Move(neighbour);
                        return;
                    }
                }
            }
        }

    }
}
