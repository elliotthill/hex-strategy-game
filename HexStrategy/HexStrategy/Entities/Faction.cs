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

        public float treasury = 1f;

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
        }

        public void DayTick()
        {
            CollectTaxes();

            if (this != Core.userFaction && aiController != null)
            {
                aiController.DayTick();
            }

        }

        /*
         * Only gamey stuff 
         */

        private void CollectTaxes()
        {
            foreach (Hex hex in hexList)
            {
                treasury += (hex.hexData.population * hex.hexData.wealth * GetTaxRate() * GetTaxCollectionEfficiency()) / 365f;
            }
        }

        private float GetTaxCollectionEfficiency()
        {
            return 0.1f;
        }

        private float GetTaxRate()
        {
            return 0.4f;
        }
	}
}

