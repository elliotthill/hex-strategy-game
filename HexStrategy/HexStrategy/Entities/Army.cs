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

        public int hexIndex;
		private Hex hex;
        private Hex hexMovingTo;

        private Hex currentWaypoint;
        private List<Hex> waypoints;
        public Vector3 position;

        private Faction owner;
        private float rotation = 0f;
        private float captureProgress = -1f;

        //Serial constructor
        public Army()
        {

        }

        public void Deconstruct()
        {
            this.hexIndex = Core.map.hexList.IndexOf(this.hex);

        }

        public void Reconstruct()
        {

            /* Setup this.hex reference */
            this.hex = Core.map.hexList[this.hexIndex];
            this.StartCapture();
        }
        
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
        public void SetOwner(Faction faction)
        {
            this.owner = faction;
        }

		public Boolean Move(Hex hex)
		{

            InterruptCapture();
            waypoints = Core.map.FindPath(this.hex, hex);

            //Path finding algo could not generate a path
            if (waypoints.Count() == 0)
                return false;

            hexMovingTo = hex;
            currentWaypoint = waypoints[1];

            return true;
		}

        private void CaptureProgress(GameTime gameTime)
        {

            if (captureProgress != -1f)
            {
                captureProgress += (float)gameTime.ElapsedGameTime.TotalSeconds * 100f;

                if (captureProgress > 100f)
                {
                    this.owner.AnnexHex(hex);
                    this.captureProgress = -1f;
                }
            }



        }

        private void StartCapture()
        {
            this.captureProgress = 0f;
        }

        private void InterruptCapture()
        {
            this.captureProgress = -1f;

        }
        public Boolean IsSieging()
        {
            if (this.captureProgress != -1f)
                return true;

            return false;
        }

        private void TweenToNewLocation(GameTime gameTime)
        {
            //Face direction of new tile
            this.rotation = Core.Maths.GetRotationFromVec3(this.position, currentWaypoint.position);

            Vector3 normal = currentWaypoint.position - position;
            normal.Normalize();

            position += normal * (float)gameTime.ElapsedGameTime.TotalSeconds * Clock.timeCompression * currentWaypoint.hexData.GetMovementModifier();

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

                    //Check if we should be sieging
                    if (hex.getOwner() == null || hex.getOwner() != this.owner) 
                    {
                        StartCapture();
                    }
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
            {
                //We are moving
                TweenToNewLocation(gameTime);
            }
            else
            {
                //Were stood still
                CaptureProgress(gameTime);
            }
            
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

		public void Draw2D(SpriteBatch sb)
		{
            if (this.hex.cullState == CullState.Culled)
                return;


            //Unproject army TODO: we need to cache this as we will inevitably use elsewhere
            Vector3 location = Core.graphicsDevice.Viewport.Project(new Vector3(position.X, position.Y+4, position.Z), Core.camera.projection, Core.camera.view, Matrix.Identity);
            if (captureProgress != -1f)
            {
                
                String summary = "Capture: " + Math.Round(this.captureProgress,0) + "%";
                PrettyText captureSummary = new PrettyText(Fonts.medium, summary, new Vector2(location.X, location.Y));
                captureSummary.DrawWithShadow(sb);
            }
		}

	}
}

