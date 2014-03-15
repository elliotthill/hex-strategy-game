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
        public Color color = Color.White;

		public HexData hexData;

		public Boolean odd = false;


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
				Faction faction = new Faction (Core.RandomFactionName (), Core.RandomColorAsVector (),this);
				faction.AnnexHex (this);

				Core.factions.Add(faction);
			}

           

            
		}


		//Temp method to assign tile type to red pixel component
		/*private HexType colorToHexType(Vector3 x)
		{

			float lightness = (x.X + x.Y + x.Z) / 3;
			this.alpha = lightness / 255; 

			//1 would indicate no overall green at all, 3 would be very green
			float greenness = (x.Y) / ((x.X + x.Z) / 2);
			float blueness = (x.Z) / ((x.Y + x.Z) / 2);
			float yellowness = ((x.X + x.Y)/2 ) / x.Z;

			if (blueness > 1.2f)
				return HexType.Water;
			else if (greenness > 1.2f) {

				if (lightness > 60)
					return HexType.Land;
				else if (lightness > 50)
					return HexType.Forest;
				else
					return HexType.Mountain;
			} else if (yellowness > 1.2f)
				return HexType.Sand;
			else if (this.alpha > 0.55f)
				return HexType.Mountain;

			return HexType.ShallowWater;

		}*/

		public void Draw()
		{


            if (Core.camera.GetHexCullState(this) == CullState.Close)
			this.hexData.Draw (this.position);

		}

		public void DrawLabels(SpriteBatch sb)
		{
			if (Core.camera.GetHexCullState (this) == CullState.Close)
			this.hexData.DrawLabels(sb, position);

		}
	}
}

