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
				this.alpha = (this.alpha / 2) + 0.2f;

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
					this.alpha += Core.RandomFloat()/10f;
				}
				else {
					this.terrainType = TerrainType.Mountain;
					this.alpha += Core.RandomFloat () / 8f;

				}

			} 
			else {
				this.terrainType = TerrainType.ShallowWater;
				this.alpha = (this.alpha / 2) + 0.25f;
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

		}

		public void Draw(Vector3 position)
		{
			
			DrawScenery (position);
			DrawBuildings (position);

		}

		private void DrawScenery(Vector3 position)
		{
			Texture texture;
			Model model;

			//Get texture
			switch (this.terrainType) {


			case TerrainType.Mountain:
				texture = Textures.DarkBrown;
				model = Meshes.mountain;
				Core.dtShader.Draw(position + new Vector3(0f,-0.4f, 0f), model,texture , 1.2f + this.alpha, this.alpha, this.alpha*300f);
				break;


			case TerrainType.Forest:
				texture = Textures.green;
				model = Meshes.tree;
				Core.dtShader.Draw(position+ new Vector3(0f, 0.0f, 0f), model,texture , 0.85f, this.alpha);

				break;

			case TerrainType.Rainforest:
				texture = Textures.green;
				model = Meshes.tree;
				Core.dtShader.Draw(position+ new Vector3(0f, 0f, 0f), model,texture , 0.85f, this.alpha);
				break;



			}


		}

		private void DrawTerrain(Vector3 position)
		{
			Texture texture = Textures.lightGreen;
			Model model = Meshes.hexTop;

			//Get texture
			switch (this.terrainType) {

				case TerrainType.Plains:
				texture = Textures.green;
					break;

				case TerrainType.DryPlains:
					texture = Textures.lightGreen;
					break;

				case TerrainType.Savanna:
					texture = Textures.lightGreen;
					break;

				case TerrainType.Desert:
					texture = Textures.yellow;
					break;

				case TerrainType.Water:
					
					texture = Textures.blue;
					break;

				case TerrainType.ShallowWater:
					texture = Textures.lightBlue;
					//model = Meshes.hexTopSide;	
					break;

				case TerrainType.Mountain:
					texture = Textures.tree;
				//model = Meshes.hexTopSide;
					break;

				case TerrainType.Snow:
					texture = Textures.white;
					break;

				case TerrainType.ColdPlains:
					texture = Textures.snow;
					break;

				case TerrainType.Forest:
					texture = Textures.tree;
					break;

				case TerrainType.Rainforest:
					texture = Textures.tree;
					break;



			}

			if (Core.map.selectedHex != null && Core.map.selectedHex.hexData == this)
				Core.dtShader.Draw(position, model,texture , 1f, (this.alpha*Core.contrast+0.2f));
			else
				Core.dtShader.Draw(position, model,texture , 1f, (this.alpha*Core.contrast));
		}

		private void DrawBuildings(Vector3 position)
		{
			if (buildingType == BuildingType.Castle)
				Core.dtShader.Draw (Meshes.getModelTransform("castle") + position, Meshes.castle, Textures.DarkBrown ,0.55f,this.alpha);
		}

		public void DrawLabels(SpriteBatch sb, Vector3 position)
		{
			if (buildingType == BuildingType.Castle) {
				Vector3 location = Core.graphicsDevice.Viewport.Project(new Vector3(position.X, position.Y +2, position.Z), Core.camera.projection, Core.camera.view, Matrix.Identity);
				float distance = Vector3.DistanceSquared (position, Core.camera.position);

				//Choose best font size from distance
				SpriteFont font = Fonts.large;
				float scale = 1f;

				if (distance > 1500f)
					scale = 0.5f;

				int length = "Castle".Length * 6;


				//Font scaling algo (150f/distance)+0.2f
				sb.DrawString (font, "Castle", new Vector2 ((int)location.X - length, (int)location.Y), UserInterface.fontColor,0f,Vector2.Zero, scale, SpriteEffects.None, 1f);
			}
		}
	}
}

