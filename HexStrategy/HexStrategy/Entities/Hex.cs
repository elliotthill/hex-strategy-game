using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace HexStrategy
{

	public enum CullState {
		Unkown, Culled, Close, Far
	}

    public class Hex
	{
		public Vector3 position;

        public Matrix world = Matrix.Identity;

		public HexData hexData;

		public Boolean odd = false;

        private List<Hex> surroundingHexes;
		public Faction owner;

		public Hex(Vector3 position, Vector3 clr, float longtitude)
		{
			this.position = position;

			this.hexData = new HexData (longtitude, clr);

			if (hexData.terrainType == TerrainType.Water || hexData.terrainType == TerrainType.ShallowWater) {
				this.position = new Vector3 (this.position.X, this.position.Y - 0.3f, this.position.Z);

			}

			if (hexData.buildingType == BuildingType.Castle)
			{
				Faction faction = new Faction (Core.RandomFactionName (), Core.RandomColorAsVector (),this, true);
				faction.AnnexHex (this);

				Core.factions.Add(faction);
			}

		}

		public void DrawLabels(SpriteBatch sb)
		{
			if (Core.camera.GetHexCullState (this) == CullState.Close)
			    this.hexData.DrawLabels(sb, position);

		}

        public void SetSurroundingHexes(List<Hex> hexes)
        {
            this.surroundingHexes = hexes;
        }

        /// <summary>
        /// Use Map.GetSurroundingHexes() instead, this will return null if no cached list
        /// </summary>
        /// <returns></returns>
        public List<Hex> GetSurroundingHexes()
        {
            return this.surroundingHexes;
        }
	}
}

