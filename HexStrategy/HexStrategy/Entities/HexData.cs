using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexStrategy
{
	public enum BuildingType {

		None, Castle
	}

	public enum TerrainType{

		Water,
		ShallowWater,

		Ice,

		Rainforest,
		Savanna,

		Desert,
		Steppe,

		Forest,
		Mountain,
		Plains,

		DryPlains,
		ColdPlains,

		Snow,

	}

	public class HexData
	{
		public BuildingType buildingType;
		public TerrainType terrainType;

		public int population = 0;
		public float wealth = 1;
		public float longtitude;
		public float alpha = 1f;
        public Color color;

		public HexData(float longtitude, Vector3 x)
		{
			this.longtitude = longtitude;

			float lightness = (x.X + x.Y + x.Z) / 3f;
			this.alpha = lightness / 255f; 

			//1 would indicate no overall green at all, 3 would be very green
			float greenness = (x.Y) / ((x.X + x.Z) / 2f);
			float blueness = (x.Z) / ((x.Y + x.Z) / 2f);
			float yellowness = ((x.X + x.Y) / 2f) / x.Z;

			if (blueness > 1.2f) {
				this.terrainType = TerrainType.Water;
				this.alpha = (this.alpha / 2) + 0.35f;

			} else if (greenness > 1.2f || blueness < 1.1f) {

				if (this.alpha > 0.8f && greenness < 1.1f && yellowness < 1.1f) {
					this.terrainType = TerrainType.Snow;
					this.alpha -= 0.6f;
				}
				else if (yellowness > 1.3f && this.alpha > 0.6f) {
					this.terrainType = TerrainType.Desert;

				} else if (lightness > 70)
					this.terrainType = TerrainType.Plains;
				else if (lightness > 60) {
					this.terrainType = TerrainType.Forest;
					this.alpha += (Core.RandomFloat()/14f) + 0.05f;
				}
				else {
					this.terrainType = TerrainType.Mountain;
					this.alpha += (Core.RandomFloat () / 12f) + 0.05f;

				}

			} 
			else {
				this.terrainType = TerrainType.ShallowWater;
				this.alpha = (this.alpha / 2) + 0.35f;
			}


			if (longtitude < 0.1f || longtitude > 0.9f)
			{
				if (this.terrainType == TerrainType.Plains)
					this.terrainType = TerrainType.ColdPlains;

			} else if (longtitude > 0.1f && longtitude < 0.2f)
			{
				if (this.terrainType == TerrainType.Plains && Core.RandomFloat() > (longtitude - 0.1f)*5f)
					this.terrainType = TerrainType.ColdPlains;

			} else if(longtitude > 0.8f && longtitude < 0.9f)
			{
				if (this.terrainType == TerrainType.Plains && Core.RandomFloat() < (longtitude -0.8f)*10f)
					this.terrainType = TerrainType.ColdPlains;


			}else if ((longtitude > 0.2f && longtitude < 0.3f) ||  (longtitude > 0.7f && longtitude < 0.8f))
			{


			} else if ((longtitude > 0.3f && longtitude < 0.4f) || (longtitude > 0.6f && longtitude < 0.7f))
			{
				//Tropical
				if (this.terrainType == TerrainType.Plains && Core.RandomFloat() < 0.7f)
					this.terrainType = TerrainType.DryPlains;


			} else if ((longtitude > 0.4f && longtitude < 0.5f) ||  (longtitude > 0.5f && longtitude < 0.6f))
			{
				//Desert
				if (this.terrainType == TerrainType.Forest)
					this.terrainType = TerrainType.Rainforest;

				if (this.terrainType == TerrainType.Mountain && Core.RandomFloat() < 0.4f)
					this.terrainType = TerrainType.Steppe;

				if (this.terrainType == TerrainType.Plains && Core.RandomFloat() < 0.7f)
					this.terrainType = TerrainType.DryPlains;



			}


			if (this.terrainType == TerrainType.Plains && Core.RandomFloat () < 0.003f)
				this.buildingType = BuildingType.Castle;

            color = new Color(this.alpha, this.alpha, this.alpha);
		}

		public void DrawLabels(SpriteBatch sb, Vector3 position)
		{
			if (buildingType == BuildingType.Castle) {
				Vector3 location = Core.graphicsDevice.Viewport.Project(new Vector3(position.X, position.Y +2, position.Z), Core.camera.projection, Core.camera.view, Matrix.Identity);
				float distance = Vector3.DistanceSquared (position, Core.camera.position);

				//Choose best font size from distance
				SpriteFont font = Fonts.large;
				float scale = 1f;

				int length = "Castle".Length * 6;


				if (distance < 100f)
				sb.DrawString (font, "Castle", new Vector2 ((int)location.X - length, (int)location.Y), UserInterface.fontColor,0f,Vector2.Zero, scale, SpriteEffects.None, 1f);
			}
		}
	}
}

