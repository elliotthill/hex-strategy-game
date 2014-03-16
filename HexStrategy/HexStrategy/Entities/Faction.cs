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
		public Vector3 color;

        public float treasury = 1f;

		private List<Hex> hexList = new List<Hex> ();
		public List<Army> armyList = new List<Army> ();

        private List<Hex> addHex = new List<Hex>();
        private AIController aiController;

		public Faction (String name, Vector3 color, Hex hex, Boolean AI)
		{
			this.name = name;
			this.color = color;

			armyList.Add (new Army (hex, this));

            if (AI)
                aiController = new AIController(this);
		}

		public List<Hex> hexes()
		{
			return hexList;
		}

		public void AnnexHex(Hex hex)
		{
			//Check if already owns this
			if (hexList.Contains (hex))
				return;

			if (hex.owner != null)
			    hex.owner.CedeHex (hex);

			hexList.Add (hex);
			hex.owner = this;

		}

		public void CedeHex(Hex hex)
		{
			//Check if doesn't own this
			if (!hexList.Contains (hex))
				return;


			this.hexList.Remove (hex);
			hex.owner = null;
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

				// Copy any parent transforms.
                Matrix[] transforms = new Matrix[Meshes.hexTop.Bones.Count];
                Meshes.hexTop.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in Meshes.hexTop.Meshes)
				{

					foreach (BasicEffect effect in mesh.Effects)
					{
						effect.EnableDefaultLighting();
						effect.AmbientLightColor = color;
						effect.DirectionalLight0.Enabled = true;
						effect.DirectionalLight0.Direction = Core.sunDirection;
						effect.DirectionalLight0.DiffuseColor = color;
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

        public void Update(GameTime gameTime)
        {
            foreach (Army army in armyList)
                army.Update(gameTime);
        }

        public void UpdateDaily()
        {
            CollectTaxes();

            if (this != Core.userFaction)
            {
                aiController.UpdateDaily();
            }


            /*
            //Annex tiles around capital

            */
        }

        private void CollectTaxes()
        {
            foreach (Hex hex in hexList)
            {
                treasury += hex.hexData.wealth;
            }
        }

	}
}

