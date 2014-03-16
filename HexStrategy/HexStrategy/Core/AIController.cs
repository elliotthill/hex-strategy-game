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

        public void UpdateDaily()
        {




            //If hexes are less than 4 keep expanding territory
            if (this.faction.hexes().Count() > 4)
                return;

            List<Hex> addHex = new List<Hex>();

            foreach (Hex hex in faction.hexes())
            {

                List<Hex> surroundingHexes = Core.map.getSurroundingHexes(hex);

                foreach (Hex surroundingHex in surroundingHexes)
                {

                    addHex.Add(surroundingHex);

                }

            }

            foreach (Hex hex in addHex)
            {
                faction.AnnexHex(hex);

            }

        }

    }
}
