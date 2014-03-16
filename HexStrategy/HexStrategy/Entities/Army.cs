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
            //Face direction of new tile
            this.rotation = Core.Maths.GetRotationFromVec3(this.position, hex.position);


            hexMovingTo = hex;
		}

        //Need proper waypathing
        private void TweenToNewLocation(GameTime gameTime)
        {
            Vector3 normal = hexMovingTo.position - position;
            normal.Normalize();

            position += normal * (float)gameTime.ElapsedGameTime.TotalSeconds * Clock.timeCompression;

            if (Vector3.Distance(hexMovingTo.position, this.position) < 1f)
            {
                //Finish travelling
                this.hex = this.hexMovingTo;
                this.position = this.hex.position;
                this.hexMovingTo = null;
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

