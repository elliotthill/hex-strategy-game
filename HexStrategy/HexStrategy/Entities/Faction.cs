using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexStrategy
{

	public class Faction
	{
		public String name;
		public Vector3 colorVec;
        public Color color, borderColor;

        public float population = 0f;
        public float treasury = 0f;
        public float GDPc = 0f;
        public float foodCost = 1f;
        public float totalFood = 0f;
        public float lastDayTaxRevenue = 0f;
        public float infastructureTaxModifier = 1f;
        public float percentRural = 0.5f;

        public List<Diplomacy> diplomacy = new List<Diplomacy>();

		private List<Hex> hexList = new List<Hex>();
        public List<int> hexListIndicies;

        private List<Hex> borders = new List<Hex>();
        private List<Hex> visible = new List<Hex>();

		public List<Army> armyList = new List<Army>();

        private List<Hex> addHex = new List<Hex>();
        private AIController aiController;
        
        public Faction()
        {
        }

        public void ResetAI()
        {
            aiController = new AIController(this);
        }
        /// <summary>
        /// This is the defacto post-serial constructor, all other game objects at this point have
        /// correct values from file loading. Now we must reconstruct references etc.
        /// </summary>
        public void Reconstruct()
        {
            
            //Reconstruct hex references from indicies
            foreach (int x in hexListIndicies)
            {
                hexList.Add(Core.map.hexList[x]);
                Core.map.hexList[x].setOwner(this);
            }

            foreach (Army army in armyList)
                army.SetOwner(this);

            //Reconstruct hexList from indicies - use proxy obj
            if (Core.userFaction != this)
                aiController = new AIController(this);

            foreach (Diplomacy diplo in diplomacy)
                diplo.Reconstruct();


            CalculateBorders();
        }

        /// <summary>
        /// Deconstruct faction for serialization. All references need to be converted to indicies
        /// in reference to the original list, probably contained in Core or Map.
        /// </summary>
        public void Deconstruct()
        {
            hexListIndicies = new List<int>();
            foreach (Hex hex in hexList)
                hexListIndicies.Add(Core.map.hexList.IndexOf(hex));

        }


		public Faction (String name, Vector3 color, Hex hex, Boolean AI)
		{
			this.name = name;
			this.colorVec = color;

			armyList.Add (new Army (hex, this));
            hexList.Add(hex);
            hex.setOwner(this);
            borders.Add(hex);
            if (AI)
                aiController = new AIController(this);


            this.color = new Color(colorVec.X / 2f, colorVec.Y / 2f, colorVec.Z / 2f);
            this.borderColor = new Color(colorVec.X / 5f, colorVec.Y / 5f, colorVec.Z / 5f);
		}

        //Create idle faction
        public Faction(String name, Vector3 color)
        {
            this.name = name;
            this.colorVec = color;


            this.color = new Color(colorVec.X / 2f, colorVec.Y / 2f, colorVec.Z / 2f);
            this.borderColor = new Color(colorVec.X / 5f, colorVec.Y / 5f, colorVec.Z / 5f);
        }

		public List<Hex> GetOwned()
		{
			return hexList;
		}
        public List<Hex> GetBorders()
        {
            return this.borders;
        }
        public List<Hex> GetVisible()
        {
            visible.Clear();
            foreach (Hex hex in hexList)
            {
                if (hex.getCullState() != CullState.Culled)
                    visible.Add(hex);
            }

            return visible;
        }
        public List<Faction> getEnemies()
        {
            List<Faction> enemies = new List<Faction>();

            foreach (Diplomacy diplo in diplomacy)
            {
                if (diplo.diplomacyType == DiplomacyType.War)
                    enemies.Add(diplo.getTargetFaction());
            }
            return enemies;
        }
        public List<Faction> getAllies()
        {
            List<Faction> allies = new List<Faction>();

            foreach (Diplomacy diplo in diplomacy)
            {
                if (diplo.diplomacyType == DiplomacyType.Allied)
                    allies.Add(diplo.getTargetFaction());
            }
            return allies;
        }
        public Boolean AreWeAtWar()
        {
            if (getEnemies().Count() > 0)
                return true;

            return false;
        }

        public List<Hex> GetBordersVisible()
        {
            visible.Clear();
            foreach (Hex hex in this.GetBorders())
            {
                if (hex.getCullState() != CullState.Culled)
                    visible.Add(hex);
            }

            return visible;
        }

		public void AnnexHex(Hex hex)
		{
			//Check if already owns this
			if (hexList.Contains (hex))
				return;

			if (hex.getOwner() != null)
			    hex.getOwner().CedeHex (hex);

			hexList.Add (hex);
			hex.setOwner(this);
            CalculateBorders();
		}

		public void CedeHex(Hex hex)
		{
			//Check if doesn't own this
			if (!hexList.Contains (hex))
				return;


			this.hexList.Remove (hex);
			hex.setOwner(null);
            CalculateBorders();
		}

        private void CalculateBorders()
        {
            //We are initialising
            if (Core.map == null)
                return;

            borders.Clear();

            foreach (Hex hex in hexList)
            {
                hex.SetIsBorder(false);
                foreach (Hex borderHex in Core.map.FindNeighbours(hex))
                {
                    if (borderHex.getOwner() == null || borderHex.getOwner() != this)
                    {

                        if (!borders.Contains(hex) && borderHex.IsNotWater())
                        {
                            borders.Add(hex);
                            hex.SetIsBorder(true);
                        }
                            
                        
                    }
                }
            }
        }

        public void DrawArmies()
        {
            foreach (Army army in armyList)
            {
                army.Draw3D();
            }
        }

		public void DrawOwnershipFilter()
		{
            
			//Draws ownership filter
			foreach (Hex hex in hexList) {

				if (Core.camera.GetHexCullState (hex) == CullState.Culled)
					continue;

                Vector3 clr = this.colorVec;

                if (borders.Contains(hex))
                    clr = new Vector3(colorVec.X - 155f, colorVec.Y - 155f, colorVec.Z - 155f);

				// Copy any parent transforms.
                Matrix[] transforms = new Matrix[Meshes.hexTop.Bones.Count];
                Meshes.hexTop.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in Meshes.hexTop.Meshes)
				{

					foreach (BasicEffect effect in mesh.Effects)
					{
						effect.EnableDefaultLighting();
						effect.AmbientLightColor = colorVec;
						effect.DirectionalLight0.Enabled = true;
						effect.DirectionalLight0.Direction = Core.sunDirection;
						effect.DirectionalLight0.DiffuseColor = clr;
						effect.DirectionalLight1.Enabled = false;
						effect.DirectionalLight2.Enabled = false;
						effect.World = Matrix.CreateScale(1f)
							* Matrix.CreateTranslation(hex.position + new Vector3(0f,0.1f,0f));
						effect.View = Core.camera.view;
						effect.Projection = Core.camera.projection;
						effect.Alpha = 0.6f;

						//effect.TextureEnabled = true;
					}
					// Draw the mesh, using the effects set above.
					mesh.Draw();
				}
			}
		}

        public void Draw2D(SpriteBatch sb)
        {
            foreach (Army army in armyList)
                army.Draw2D(sb);
        }

        public void Update(GameTime gameTime)
        {
            foreach (Army army in armyList)
                army.Update(gameTime);

            if (this != Core.userFaction && aiController != null)
            {
                aiController.Update();
            }
        }

        public void DayTick()
        {
            EconomicTick();

            if (this != Core.userFaction && aiController != null)
            {
                aiController.DayTick();
            }

        }

        public void MonthTick()
        {
            if (this != Core.userFaction && aiController != null)
            {
                aiController.MonthTick();
            }
        }

        public void YearTick()
        {
            if (this != Core.userFaction && aiController != null)
            {
                aiController.YearTick();
            }
        }

        /*
         * Only gamey stuff 
         */
        private void EconomicTick()
        {


            population = 0f;
            totalFood = 0f;
            GDPc = 0f;
            infastructureTaxModifier = 0f;
            percentRural = 0f;

            //Calculate faction values first
            foreach (Hex hex in hexList)
            {
                
                population += hex.hexData.population;
                totalFood += hex.hexData.agriculturalOutput;
                GDPc += hex.hexData.tradeItemsOutput;
                infastructureTaxModifier += hex.hexData.GetInfastructureModifier();

                if(hex.hexData.buildingType == BuildingType.None)
                percentRural += hex.hexData.population;
            }

            //Per capita
            GDPc = GDPc/ population;

            //Find percentage
            percentRural = percentRural / population;

            //Avg
            infastructureTaxModifier = infastructureTaxModifier / hexList.Count();

            foodCost = (population / totalFood)  + GetTaxRate();
            //CLamp food
            foodCost = (foodCost < 0.5f) ? 0.5f : (foodCost > 2f) ? 2f : foodCost;
            

            lastDayTaxRevenue = 0f;
            
            foreach (Hex hex in hexList)
            {
                hex.hexData.EconomicTick(foodCost, GetTaxRate(), GetTaxEff(), 1f, 1f);
                lastDayTaxRevenue += hex.hexData.taxRevenue * infastructureTaxModifier;
                
            }
            treasury += lastDayTaxRevenue;

        }

        /// <summary>
        /// What percentage of the tax rate ends up in the state coffers?
        /// </summary>
        /// <returns></returns>
        private float GetTaxEff()
        {
            return 0.75f;
        }

        /// <summary>
        /// What percentage of a persons income was subject to tax?
        /// </summary>
        /// <returns></returns>
        private float GetTaxRate()
        {
            return 0.3f;
        }
	}
}

