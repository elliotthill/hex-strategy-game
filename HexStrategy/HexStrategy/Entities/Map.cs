using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexStrategy
{
	public class Map
	{
		int width, height;
		List<Hex> hexList = new List<Hex> ();

        #region sublists
        List<Hex> visibleHex = new List<Hex>();
        List<Hex> plainHex = new List<Hex>();
        List<Hex> desertHex = new List<Hex>();
        List<Hex> waterHex = new List<Hex>();
        List<Hex> shallowWaterHex = new List<Hex>();
        List<Hex> mountainHex = new List<Hex>();
        List<Hex> forestHex = new List<Hex>();
        List<Hex> dryPlainHex = new List<Hex>();
        List<Hex> coldPlainHex = new List<Hex>();
        List<Hex> snowHex = new List<Hex>();
        List<Hex> iceHex = new List<Hex>();
        List<Hex> savannaHex = new List<Hex>();
        List<Hex> rainforestHex = new List<Hex>();
        List<Hex> steppeHex = new List<Hex>();

        List<Hex> castleHex = new List<Hex>();

        #endregion

        //FBX hex model is imperfect
		float spacingX = 2.94f;
		float spacingZ = 0.87f;
		float oddRowOffset = 1.49f;

		public Hex selectedHex;
		public List<Hex> highlightedHex = new List<Hex>();

		Vector3[,] heightData;

		public Map ()
		{
			LoadHeightData();
			HeightDataToMap();

            //Bug, doesnt work if you put dis in hex constructor :$
            foreach (Hex hex in hexList)
            {
                hex.world = Matrix.CreateTranslation(hex.position);
            }

		}

		//Partially stolen from here: http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series1/Terrain_from_file.php
		//Use google earth screenshots (increase contrast 20% + brightness 10%)
		private void LoadHeightData()
		{
			this.width = Textures.heightMap.Width;
			this.height = Textures.heightMap.Height;

			//Load the color data into one dim array
			Color[] heightMapColors = new Color[this.width * this.height];
			Textures.heightMap.GetData(heightMapColors);

			//Load R component into 2D array
			heightData = new Vector3[this.width, this.height];
			for (int x = 0; x < this.width; x++)
				for (int y = 0; y < this.height; y++)
					heightData[x, y] = new Vector3(heightMapColors[x + y * this.width].R,
													heightMapColors[x + y * this.width].G,
													heightMapColors[x + y * this.width].B);
		}

		private void HeightDataToMap()
		{
			for(int i = 0; i < width; i++)
			{
				for (int z = 0; z < height; z++)
				{
					Vector3 loc = new Vector3 (i * spacingX, 0, z * spacingZ);
					Hex hex = new Hex(loc, heightData[i,z], ((float)z/(float)height));

					//Odd row offset
					if (z % 2 != 0) {
						hex.position += new Vector3 (oddRowOffset, 0, 0);
						hex.odd = true;
					}

					hexList.Add(hex);
				}
			}
		}

		public void Update(GameTime gameTime)
		{
			UpdateMouse (gameTime);
		}

		private void UpdateMouse(GameTime gameTime)
		{
			//Did the user click on the map?
			if (Core.leftClickLastFrame == true) {

				Vector3 nearSource = new Vector3 ((float)Core.mouseState.X, (float)Core.mouseState.Y, 0f);
				Vector3 farSource = new Vector3 ((float)Core.mouseState.X, (float)Core.mouseState.Y, 1f);
				Matrix world = Matrix.CreateTranslation (Vector3.Zero);


				Vector3 nearPoint = Core.graphicsDevice.Viewport.Unproject (nearSource, 
																			Core.camera.projection, 
																			Core.camera.view, world);

				Vector3 farPoint = Core.graphicsDevice.Viewport.Unproject (farSource, 
																			Core.camera.projection, 
																			Core.camera.view, world);

				Vector3 direction = farPoint - nearPoint;
				direction.Normalize ();

				Ray pickRay = new Ray (nearPoint, direction);


					foreach (Hex hex in hexList) {

						if (Core.camera.GetHexCullState(hex) == CullState.Culled)
							continue;


						float? result = new BoundingSphere (new Vector3(hex.position.X, 
																		hex.position.Y + 0.5f,
																		hex.position.Z), 1f).Intersects (pickRay);

						if (result.HasValue)
						{
							//User clicked this hex
							this.selectedHex = hex;
							
							break;
						}


					}
					
			}
		}

		public  void Draw3D()
		{
            /* Get list of visible hexes and clear sublists */
            visibleHex.Clear();

            foreach (Hex hex in hexList)
            {
                if (Core.camera.GetHexCullState(hex) != CullState.Culled)
                    visibleHex.Add(hex);
            }

            plainHex.Clear();
            desertHex.Clear();
            dryPlainHex.Clear();
            waterHex.Clear();
            shallowWaterHex.Clear();
            mountainHex.Clear();
            forestHex.Clear();
            coldPlainHex.Clear();
            snowHex.Clear();
            iceHex.Clear();
            savannaHex.Clear();
            rainforestHex.Clear();
            steppeHex.Clear();
            castleHex.Clear();

            /* Sort each visible hex into appropriate sublist */
            foreach (Hex hex in visibleHex)
            {
                if (hex.hexData.terrainType == TerrainType.Plains)
                    plainHex.Add(hex);
                else if (hex.hexData.terrainType == TerrainType.Desert)
                    desertHex.Add(hex);
                else if (hex.hexData.terrainType == TerrainType.DryPlains)
                    dryPlainHex.Add(hex);
                else if (hex.hexData.terrainType == TerrainType.Water)
                    waterHex.Add(hex);
                else if (hex.hexData.terrainType == TerrainType.Mountain)
                    mountainHex.Add(hex);
                else if (hex.hexData.terrainType == TerrainType.ShallowWater)
                    shallowWaterHex.Add(hex);
                else if (hex.hexData.terrainType == TerrainType.Forest)
                    forestHex.Add(hex);
                else if (hex.hexData.terrainType == TerrainType.ColdPlains)
                    coldPlainHex.Add(hex);
                else if (hex.hexData.terrainType == TerrainType.Snow)
                    snowHex.Add(hex);
                else if (hex.hexData.terrainType == TerrainType.Ice)
                    iceHex.Add(hex);
                else if (hex.hexData.terrainType == TerrainType.Savanna)
                    savannaHex.Add(hex);
                else if (hex.hexData.terrainType == TerrainType.Rainforest)
                    rainforestHex.Add(hex);
                else if (hex.hexData.terrainType == TerrainType.Steppe)
                    steppeHex.Add(hex);

                if (hex.hexData.buildingType == BuildingType.Castle)
                    castleHex.Add(hex);
                
            }

            Model hexModel = Meshes.hexTopInstanced;

            /* Batch draw each sublist */
            Render.DrawInstances(plainHex, hexModel, Textures.green);
            Render.DrawInstances(desertHex, hexModel, Textures.yellow);
            Render.DrawInstances(dryPlainHex, hexModel, Textures.lightGreen);
            Render.DrawInstances(waterHex, hexModel, Textures.blue);
            Render.DrawInstances(shallowWaterHex, hexModel, Textures.lightBlue);
            Render.DrawInstances(mountainHex, hexModel, Textures.tree);
            Render.DrawInstances(forestHex, hexModel, Textures.tree);
            Render.DrawInstances(coldPlainHex, hexModel, Textures.snow);
            Render.DrawInstances(iceHex, hexModel, Textures.white);
            Render.DrawInstances(snowHex, hexModel, Textures.white);
            Render.DrawInstances(savannaHex, hexModel, Textures.lightGreen);
            Render.DrawInstances(rainforestHex, hexModel, Textures.tree);
            Render.DrawInstances(steppeHex, hexModel, Textures.DarkBrown);

            /* Draw mountains, trees and castles */
            Render.DrawInstances(mountainHex, Meshes.mountain, Textures.DarkBrown);
            Render.DrawInstances(forestHex, Meshes.tree, Textures.green);
            Render.DrawInstances(rainforestHex, Meshes.tree, Textures.DarkGreen);

            Render.setWorld(Matrix.CreateScale(0.00065f) * Matrix.CreateTranslation(new Vector3(0f, 0.65f, 0.3f)));
            Render.DrawInstances(castleHex, Meshes.castle, Textures.DarkBrown);
            Render.setWorld(Matrix.Identity);
		}

		public void Draw2D(SpriteBatch sb)
		{

			foreach (Hex hex in hexList)
			{
				hex.DrawLabels(sb);
			}
		}

		public List<Hex> getSurroundingHexes(Hex hex)
		{
			//Get index of hex, and make new list to store surrounding hex
			int index = hexList.IndexOf(hex);
			List<Hex> surroundingHexes = new List<Hex>();

			if (hex.odd) {

				//If theres a tile above
				if (hexList.Count () > index + height + 11 && index + height + 1 > 0)
					surroundingHexes.Add (hexList [index + height + 1]);

				if (hexList.Count () > index + height - 1 && index + height - 1 > 0)
					surroundingHexes.Add (hexList [index + height - 1]);

				if (hexList.Count () > index - 2 && index - 2 > 0)
					surroundingHexes.Add (hexList [index - 2]);

				if (hexList.Count () > index + 2 && index + 2 > 0)
					surroundingHexes.Add (hexList [index + 2]);

				if (hexList.Count () > index + 1 && index + 1 > 0)
					surroundingHexes.Add (hexList [index + 1]);

				if (hexList.Count () > index - 1 && index - 1 > 0)
					surroundingHexes.Add (hexList [index - 1]);

			} else {

				if (hexList.Count () > index - height + 1 && index - height + 1 > 0)
					surroundingHexes.Add (hexList [index - height + 1]);

				if (hexList.Count () > index - height - 1 && index - height - 1 > 0)
					surroundingHexes.Add (hexList [index - height - 1]);

				if (hexList.Count () > index - 2 && index - 2 > 0)
					surroundingHexes.Add (hexList [index - 2]);

				if (hexList.Count () > index + 2 && index + 2 > 0)
					surroundingHexes.Add (hexList [index + 2]);

				if (hexList.Count () > index + 1 && index + 1 > 0)
					surroundingHexes.Add (hexList [index + 1]);

				if (hexList.Count () > index - 1 && index - 1 > 0)
					surroundingHexes.Add (hexList [index - 1]);
			}

			return surroundingHexes;


		}


		/*private Boolean AnySurroundingHexOfType (Hex hex, HexType hexType)
		{
			int index = hexList.IndexOf(hex);

			if (hex.odd) {

				//If theres a tile above
				if (hexList.Count () > index + height + 11 && index + height + 1 > 0)
					if (hexList [index + height + 1].type == hexType)
						return true;

				if (hexList.Count () > index + height - 1 && index + height - 1 > 0)
					if (hexList [index + height - 1].type == hexType)
						return true;

				if (hexList.Count () > index - 2 && index - 2 > 0)
					if (hexList [index - 2].type == hexType)
						return true;

				if (hexList.Count () > index + 2 && index + 2 > 0)
					if (hexList [index + 2].type == hexType)
						return true;

				if (hexList.Count () > index + 1 && index + 1 > 0)
					if (hexList [index + 1].type == hexType)
						return true;

				if (hexList.Count () > index - 1 && index - 1 > 0)
					if (hexList [index - 1].type == hexType)
						return true;

			} else {

				if (hexList.Count () > index - height + 1 && index - height + 1 > 0)
					if (hexList [index - height + 1].type == hexType)
						return true;

				if (hexList.Count () > index - height - 1 && index - height - 1 > 0)
					if (hexList [index - height - 1].type == hexType)
						return true;

				if (hexList.Count () > index - 2 && index - 2 > 0)
					if (hexList [index - 2].type == hexType)
						return true;

				if (hexList.Count () > index + 2 && index + 2 > 0)
					if (hexList [index + 2].type == hexType)
						return true;

				if (hexList.Count () > index + 1 && index + 1 > 0)
					if (hexList [index + 1].type == hexType)
						return true;

				if (hexList.Count () > index - 1 && index - 1 > 0)
					if (hexList [index - 1].type == hexType)
						return true;
			}

			return false;
		}*/
	}
}

