using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexStrategy
{
    /// <summary>
    /// World data provides an easy way to bundle up game state variables
    /// for serialization and deserialization from save files.
    /// </summary>
    [Serializable]
    public class WorldData
    {
        public List<Hex> hexList;
        public List<Faction> faction;
        public Vector3 cameraPosition;
        public Vector3 cameraLookAt;
        public int userFactionIndex;



        /// <summary>
        /// Constructor makes copies of game variables like ships, ready to be serialized.
        /// </summary>
        public WorldData()
        {
            Core.Deconstruct();
            

            this.hexList = Core.map.hexList;
            this.faction = Core.factions;
            this.cameraPosition = Core.camera.position;
            this.cameraLookAt = Core.camera.lookAt;

            this.userFactionIndex = Core.factions.IndexOf(Core.userFaction);

            Logger.AddMessage("Saved Factions " + this.faction.Count);
            Logger.AddMessage("Saved hexes " + this.hexList.Count);
        }
    }
}
