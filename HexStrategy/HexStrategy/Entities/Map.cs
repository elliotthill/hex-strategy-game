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
		public List<Hex> hexList = new List<Hex> ();
        
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
        public Army selectedArmy;

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
            int index = 0;

			for(int i = 0; i < width; i++)
			{
				for (int z = 0; z < height; z++)
				{
                    Vector3 loc = new Vector3(i * spacingX,0, z * spacingZ);
					Hex hex = new Hex(loc, heightData[i,z], ((float)z/(float)height));

					//Odd row offset
					if (z % 2 != 0) {
						hex.position += new Vector3 (oddRowOffset, 0, 0);
						hex.odd = true;
					}
                    hex.index = index;
                    hexList.Insert(index, hex);
                    index++;
					
				}
			}

            heightData = null;
            Textures.heightMap = null;
		}

        public void LoadFromWorldData(List<Hex> hexes)
        {
            this.hexList = hexes;
            this.selectedHex = null;
            this.selectedArmy = null;
           
            
        }
		public void Update(GameTime gameTime)
		{
			UpdateMouse (gameTime);
		}

		private void UpdateMouse(GameTime gameTime)
		{
            UpdateLeftMousePicking(gameTime);
            UpdateRightMousePicking(gameTime);
		}
        private void UpdateLeftMousePicking(GameTime gameTime)
        {
            
            if (Core.leftClickLastFrame == true)
            {

                /* Check user armies */
                foreach (Army army in Core.userFaction.armyList)
                {
                    if (!Core.camera.Visible(army.getPosition()))
                        continue;


                    float? result = new BoundingSphere(new Vector3(army.getPosition().X,
                                                                    army.getPosition().Y + 0.5f,
                                                                    army.getPosition().Z), 1f).Intersects(Core.camera.PickRay());

                    if (result.HasValue)
                    {
                        //User selected this army, also unset selected hex
                        this.selectedArmy = army;
                        this.selectedHex = null;
                        return;
                    }
                }


                /* Check each tile */
                foreach (Hex hex in hexList)
                {

                    if (Core.camera.GetHexCullState(hex) == CullState.Culled)
                        continue;


                    float? result = new BoundingSphere(new Vector3(hex.position.X,
                                                                    hex.position.Y + 0.5f,
                                                                    hex.position.Z), 1f).Intersects(Core.camera.PickRay());
                    if (result.HasValue)
                    {
                        //User clicked this hex
                        this.selectedHex = hex;
                        this.selectedArmy = null;
                        return;
                    }

                }

            }
        }
        private void UpdateRightMousePicking(GameTime gameTime)
        {
            if (Core.rightClickLastFrame == true)
            {
                
                if (this.selectedArmy != null)
                {
                    //User has moved his army

                    //Check for tile army should move to
                    foreach (Hex hex in hexList)
                    {

                        if (Core.camera.GetHexCullState(hex) == CullState.Culled)
                            continue;


                        float? result = new BoundingSphere(new Vector3(hex.position.X,
                                                                        hex.position.Y + 0.5f,
                                                                        hex.position.Z), 1f).Intersects(Core.camera.PickRay());
                        if (result.HasValue)
                        {
                            //User clicked this hex
                            this.selectedArmy.Move(hex);
                            return;
                        }

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
                hex.cullState = Core.camera.GetHexCullState(hex);

                if (hex.cullState != CullState.Culled)
                    visibleHex.Add(hex);
            }
            //IF camera really zoomed out only draw faction filter

            if (Core.camera.position.Y < 100f)
            {
                /* Sort those visible hexes into sublists for batch rendering */
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
                Render.DrawInstances(shallowWaterHex, hexModel, Textures.blue);
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
                Render.DrawInstances(forestHex, Meshes.tree, Textures.green, 0.75f);
                Render.DrawInstances(rainforestHex, Meshes.tree, Textures.DarkGreen);

                Render.setWorld(Matrix.CreateScale(0.00065f) * Matrix.CreateTranslation(new Vector3(0f, 0.65f, 0.3f)));
                Render.DrawInstances(castleHex, Meshes.castle, Textures.darkGrey);
                Render.setWorld(Matrix.Identity);

                /*
                 * Draw faction ownership tiles
                 */
                foreach (Faction faction in Core.factions)
                {
                    Render.DrawInstances(faction.GetVisible(), Meshes.hexTopInstanced, Textures.white, 0.3f, true);
                }
            }
            else
            {
                /*
                 * Draw faction ownership tiles
                 */
                foreach (Faction faction in Core.factions)
                {
                    Render.DrawInstances(faction.GetVisible(), Meshes.hexTopInstanced, Textures.white, 1f, true);
                }
            }


		}

		public void Draw2D(SpriteBatch sb)
		{

			foreach (Hex hex in visibleHex)
			{
				hex.DrawLabels(sb);
			}
		}

        /// <summary>
        /// Crude pathfinding function TODO: use proper A* pathfinding technique :- keeping a sorted priority queue of alternate path segments along the way.
        /// </summary>
        /// <param name="source">The tile the army is standing on</param>
        /// <param name="dest">The tile you want it to move to</param>
        /// <returns></returns>
        public List<Hex> FindPath(Hex source, Hex dest)
        {
            List<Hex> waypath = new List<Hex>();
            waypath.Add(source);

            List<Hex> checkedPaths = new List<Hex>();
            Hex current = source;



            Boolean done = false;

            while (!done)
            {
                //Find closest hex to dest
                float shortestDistance = -1f;
                Hex shortestDistanceHex = null;


                foreach (Hex hex in this.FindNeighbours(current))
                {
                    
                    //Disallow certain terrain
                    if (hex.hexData.terrainType == TerrainType.Water 
                        || hex.hexData.terrainType == TerrainType.ShallowWater)
                        continue;

                    float cost = Vector3.DistanceSquared(hex.position, dest.position);
                    cost *= hex.hexData.GetMovementCost();

                    //Dont try checked paths again
                    if (checkedPaths.Contains(hex) || waypath.Contains(hex))
                        continue;

                    //First one
                    if (shortestDistance == -1f)
                    {
                        shortestDistanceHex = hex;
                        shortestDistance = cost;
                        continue;
                    }

                    //If its shortest distance and were not looping, this also means there are multiple options
                    if (cost < shortestDistance)
                    {

                        shortestDistance = cost;
                        shortestDistanceHex = hex;

                    }
                    
                }

                //Add next waypath
                if (shortestDistanceHex != null)
                {
                        
                    waypath.Add(shortestDistanceHex);
                    current = shortestDistanceHex;
                    checkedPaths.Add(current);

                    
                }
                else
                {
                    if ((waypath.Count() - 1) < 1)
                        return new List<Hex>();

                    //Cant find a route, roll back a waypoint and try a different tile
                    Hex lastTile = waypath[waypath.Count() - 1];
                    current = lastTile;
                    waypath.Remove(lastTile);
                    
                }

                //Short it if its going crazy
                if (checkedPaths.Count() > 500)
                    return new List<Hex>();

                if (shortestDistanceHex == dest)
                    done = true;

            }

            return waypath;
        }

        /// <summary>
        /// Uses caching but is incredibly slow when run first time - UPDATE indexOf is the hot line
        /// </summary>
        /// <param name="hex">The tile you wish to get the neighbours of</param>
        /// <returns></returns>
		public List<Hex> FindNeighbours(Hex hex)
		{
            //Is it cached?
            if (hex.GetSurroundingHexes() != null)
                return hex.GetSurroundingHexes();

			//Get index of hex, and make new list to store surrounding hex
            int index = hex.index;
			List<Hex> surroundingHexes = new List<Hex>();

            int count = hexList.Count();

			if (hex.odd) {

				//If theres a tile above
                if (count > index + height + 11 && index + height + 1 > 0)
					surroundingHexes.Add (hexList [index + height + 1]);

                if (count > index + height - 1 && index + height - 1 > 0)
					surroundingHexes.Add (hexList [index + height - 1]);

                if (count > index - 2 && index - 2 > 0)
					surroundingHexes.Add (hexList [index - 2]);

                if (count > index + 2 && index + 2 > 0)
					surroundingHexes.Add (hexList [index + 2]);

                if (count > index + 1 && index + 1 > 0)
					surroundingHexes.Add (hexList [index + 1]);

                if (count > index - 1 && index - 1 > 0)
					surroundingHexes.Add (hexList [index - 1]);

			} else {

                if (count > index - height + 1 && index - height + 1 > 0)
					surroundingHexes.Add (hexList [index - height + 1]);

                if (count > index - height - 1 && index - height - 1 > 0)
					surroundingHexes.Add (hexList [index - height - 1]);

                if (count > index - 2 && index - 2 > 0)
					surroundingHexes.Add (hexList [index - 2]);

                if (count > index + 2 && index + 2 > 0)
					surroundingHexes.Add (hexList [index + 2]);

				if (count > index + 1 && index + 1 > 0)
					surroundingHexes.Add (hexList [index + 1]);

				if (count > index - 1 && index - 1 > 0)
					surroundingHexes.Add (hexList [index - 1]);
			}

            hex.SetSurroundingHexes(surroundingHexes);
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

