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
        public Vector2 position2D;

        public Matrix world = Matrix.Identity;
        public BoundingSphere bsphere;
		public HexData hexData;
        public Boolean isBorder = false;
        public int index;

		public Boolean odd = false;

        private List<Hex> surroundingHexes;
		private Faction owner;

        public CullState cullState = CullState.Unkown;

        //Serial constructor
        public Hex()
        {
        }

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
				

				Core.factions.Add(faction);
			}
            bsphere = new BoundingSphere(position, 1f);
            position2D = new Vector2(position.X, position.Z);
		}


		public void DrawLabels(SpriteBatch sb)
		{
			    this.hexData.DrawLabels(sb, position);

		}

        public Faction getOwner()
        {
            return this.owner;
        }

        public void setOwner(Faction faction)
        {
            this.owner = faction;
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

        public Boolean IsNotWater()
        {
            if (this.hexData.terrainType == TerrainType.Water ||
                this.hexData.terrainType == TerrainType.ShallowWater)
                return false;

            return true;
        }
	}
}

