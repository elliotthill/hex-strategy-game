using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexStrategy
{

	public enum BuildingType {

		None, Town, Fortified, Church, Port, Market
	}

	public enum TerrainType{
        None,

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
		public BuildingType buildingType = BuildingType.None;
		public TerrainType terrainType;


        public String name;

        //Anything above 2500 is considered a city       
		public float population = 0f;

        //Local GDP/c or average wage = Price of trade goods produced / population
        public float GDPc = 0f;

        //Tax revenue = Population * GDP/c * Tax Rate * Tax collection efficiency
        public float taxRevenue = 0f;

        //Population growth = 1/cost of food
        public float popGrowth = 1f;

        //Agricultural output = (local pop * %of pop employed in agriculture) * terrain modifier * agricultural 
        public float agriculturalOutput = 0f;

        //Trade items output = local pop * local pop % not employed in agriculture * manufacturing efficiency
        public float tradeItemsOutput = 0f;

		public float longtitude;
		public float alpha = 1f;
        public Color color;


        //Serial constructor
        public HexData()
        {
        }

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
				this.alpha = (this.alpha / 1.3f) + 0.35f;

			} else if (greenness > 1.2f || blueness < 1.1f) {

				if (this.alpha > 0.8f && greenness < 1.1f && yellowness < 1.1f) {
					this.terrainType = TerrainType.Snow;
					this.alpha -= 0.2f;
				}
				else if (yellowness > 1.2f && this.alpha > 0.6f) {
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
				this.alpha = (this.alpha / 1.3f) + 0.35f;
			}

            
            color = new Color((this.alpha / 2) + 0.1f, (this.alpha / 2) + 0.1f,( this.alpha / 2) + 0.1f);

            SetPopulation();
		}

        private void SetPopulation()
        {
            switch (this.terrainType)
            {
                case TerrainType.Plains:
                    population = 300f;
                    break;

                case TerrainType.DryPlains:
                    population = 250f;
                    break;

                case TerrainType.ColdPlains:
                    population = 280f;
                    break;

                case TerrainType.Forest:
                    population = 280f;
                    break;

                case TerrainType.Mountain:
                    population = 140f;
                    break;
            }
        }

        public void EconomicTick(float foodCost, float taxRate, float taxEff, float agriculturalEff
            , float manufacturingEff)
        {
            if (population < 1f)
                return;

            GDPc = (tradeItemsOutput) / population;
            taxRevenue = (population * GDPc * taxRate * taxEff)/365f;

            
            


            if (buildingType != BuildingType.None)
            {
                //Its a town, no food output, all in trade items
                agriculturalOutput = 0f;
                tradeItemsOutput = population * 1000f * GetManufacturingModifier() * manufacturingEff;

                //Cheap food? MOve to city, 
                popGrowth = (1f - foodCost) / 365f;
            }
            else
            {
                agriculturalOutput = ((population)) * 2f * GetAgricultureModifier() * agriculturalEff;
                tradeItemsOutput = 0f;

                //Expensive food? move to rural (with penalty)
                popGrowth = ((foodCost - 1f) * 0.5f) / 365f;
            }

            population += population * popGrowth;

            if (population > 2500 && buildingType == BuildingType.None)
                buildingType = BuildingType.Town;
            else if (population < 2000 && buildingType != BuildingType.None)
                buildingType = BuildingType.None;
                

            
        }

        //TODO instead of penalsing amount of desert, we should trace route from capital to see if hex is cutt off from capital by desert
        public float GetInfastructureModifier()
        {
            switch (terrainType)
            {
                case TerrainType.Plains:
                    return 1f;
                case TerrainType.ColdPlains:
                    return 0.9f;
                case TerrainType.DryPlains:
                    return 0.9f;
                case TerrainType.Forest:
                    return 0.95f;
                case TerrainType.Mountain:
                    return 0.9f;
                case TerrainType.Desert:
                    return 0.6f;

            }

            return 0.75f;
        }

        public float GetAgricultureModifier()
        {
            switch (terrainType)
            {
                case TerrainType.Plains:
                    return 1f;
                case TerrainType.ColdPlains:
                    return 0.6f;
                case TerrainType.DryPlains:
                    return 0.6f;
                case TerrainType.Forest:
                    return 0.6f;

            }

            return 0.1f;
        }

        public float GetManufacturingModifier()
        {
            switch (buildingType)
            {
                case BuildingType.Market:
                    return 1.2f;
                case BuildingType.Fortified:
                    return 1.1f;
                case BuildingType.Port:
                    return 1.1f;
                case BuildingType.Town:
                    return 1.3f;

            }

            return 1f;
        }

        /// <summary>
        /// Draws the town name and details
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="position"></param>
		public void DrawLabels(SpriteBatch sb, Vector3 position)
		{
			if (buildingType != BuildingType.None && this.name != null) {
				Vector3 location = Core.graphicsDevice.Viewport.Project(new Vector3(position.X, position.Y, position.Z), Core.camera.projection, Core.camera.view, Matrix.Identity);
				float distance = Vector3.DistanceSquared (position, Core.camera.position);
                
                SpriteFont font = Fonts.medium;

				Vector2 length = font.MeasureString(this.name);
                

                if (distance > 8000f)
                    return;

                //Draw name background
                sb.Draw(Textures.white, new Rectangle(((int)location.X-(int)length.X/2)-2,
                                                        (int)location.Y-1,
                                                        (int)length.X + 4,
                                                        (int)length.Y + 2), UserInterface.transparentBlack);


                sb.DrawString (font, this.name, new Vector2 ((int)(location.X - (int)length.X/2), (int)location.Y), UserInterface.fontColor);
			}
		}

        /// <summary>
        /// Used when calculating movement speed over terrain
        /// </summary>
        /// <returns></returns>
        public float GetMovementModifier()
        {
            float modifier = 1f;

            if (terrainType == TerrainType.Ice)
                modifier = 0.3f;
            if (terrainType == TerrainType.Snow)
                modifier = 0.1f;
            if (terrainType == TerrainType.Desert)
                modifier = 0.3f;
            if (terrainType == TerrainType.Mountain)
                modifier = 0.3f;

            return modifier;
        }

        /// <summary>
        /// Used in pathfinding
        /// </summary>
        /// <returns></returns>
        public float GetMovementCost()
        {
            float cost = 1f;

            if (terrainType == TerrainType.Ice)
                cost += 0.3f;
            if (terrainType == TerrainType.Snow)
                cost += 0.1f;
            if (terrainType == TerrainType.Desert)
                cost += 0.3f;

            return cost;
        }

	}
}

