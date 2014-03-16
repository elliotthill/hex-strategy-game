using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HexStrategy
{
	public class Army
	{

		private Hex hex;
        private Hex hexMovingTo;

        private Hex currentWaypoint;
        private List<Hex> waypoints;
        private Vector3 position;

        private Faction owner;
        private float rotation = 0f;

		public Army(Hex hex, Faction owner)
		{
			this.hex = hex;
            this.owner = owner;
            this.position = hex.position;
		}

        public Vector3 getPosition()
        {
            return this.position;
        }
        public Faction GetOwner()
        {
            return this.owner;
        }

		public void Move(Hex hex)
		{
            

            waypoints = Core.map.GetWaypath(this.hex, hex);

            //Path finding algo could not generate a path
            if (waypoints.Count() == 0)
                return;

            //Face direction of new tile
            this.rotation = Core.Maths.GetRotationFromVec3(this.position, hex.position);

            hexMovingTo = hex;
            currentWaypoint = waypoints[1];


		}



        //Need proper waypathing
        private void TweenToNewLocation(GameTime gameTime)
        {
            Vector3 normal = currentWaypoint.position - position;
            normal.Normalize();

            position += normal * (float)gameTime.ElapsedGameTime.TotalSeconds * Clock.timeCompression;

            if (Vector3.Distance(currentWaypoint.position, position) < 0.2f)
            {
                //If we have reached our destination
                if (waypoints.Count() == waypoints.IndexOf(currentWaypoint) + 1)
                {
                    //Were there!
                    this.position = hexMovingTo.position;
                    hex = hexMovingTo;
                    hexMovingTo = null;
                    waypoints.Clear();
                    currentWaypoint = null;
                }
                else
                {
                    //Else keep on moving
                    currentWaypoint = waypoints[waypoints.IndexOf(currentWaypoint) + 1];
                    this.hex = currentWaypoint;
                }

                
                
            }


        }

        public void Update(GameTime gameTime)
        {
            if (hexMovingTo != null)
                TweenToNewLocation(gameTime);
        }


		public void Draw3D()
		{
            if (!Core.camera.Visible(this.position))
                return;

            if (Core.map.selectedArmy != null && Core.map.selectedArmy == this)
                Core.dtShader.Draw(this.position + new Vector3(0f, 0.6f, 0f), Meshes.knight, Textures.knight, 0.5f, 2f, rotation);
            else
		        Core.dtShader.Draw (this.position + new Vector3(0f, 0.6f, 0f), Meshes.knight, Textures.knight,0.5f, 1f, rotation);
		}

		public void Draw2D()
		{
            
		}

	}
}

