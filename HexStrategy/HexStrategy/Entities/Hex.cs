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
        private Vector2 position2D;

        private Matrix world = Matrix.Identity;
        private BoundingSphere bsphere;
		public HexData hexData;
        private Boolean isBorder = false;
        public int index;

		public Boolean odd = false;

        private List<Hex> surroundingHexes;
		private Faction owner;

        private CullState cullState = CullState.Unkown;

        //Serial constructor
        public Hex()
        {
            
            position2D = new Vector2(position.X, position.Z);

        }

        public void Reconstruct()
        {
            world = Matrix.CreateTranslation(position);
            position2D = new Vector2(position.X, position.Z);
        }

		public Hex(Vector3 position, Vector3 clr, float longtitude)
		{
			this.position = position;

			this.hexData = new HexData (longtitude, clr);

			if (hexData.terrainType == TerrainType.Water || hexData.terrainType == TerrainType.ShallowWater) {
				this.position = new Vector3 (this.position.X, this.position.Y - 0.09f, this.position.Z);

			}

            bsphere = new BoundingSphere(position, 1f);
            position2D = new Vector2(position.X, position.Z);
		}

        public Matrix GetWorld()
        {
            return world;
        }

        public void SetWorld(Matrix world)
        {
            this.world = world;
        }

        public Boolean GetIsBorder()
        {
            return isBorder;
        }

        public void SetIsBorder(Boolean isborder)
        {
            this.isBorder = isborder;
        }

        public CullState getCullState()
        {
            return this.cullState;
        }

        public void SetCullState(CullState cullstate)
        {
            this.cullState = cullstate;
        }
        public Vector2 getPosition2D()
        {
            return position2D;
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

