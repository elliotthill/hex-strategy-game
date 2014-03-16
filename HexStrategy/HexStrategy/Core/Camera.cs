using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace HexStrategy
{

	public class Camera
	{
		private float aspectRatio, rotationY = 0.0f;
		private Vector3 up, right, forward, lookAt;

		public Vector3 position = new Vector3(0,30,0);
		public Matrix view, projection, rotation;

		public float closeDistance = 2200f;
        public float farDistance = 24000f;

		private Vector3 lookNormal;

		public Camera(float aspect)
		{
			aspectRatio = aspect;

		}

		public void Update(GameTime gameTime)
		{

			lookAt = new Vector3(0,0,30);

			rotation = Matrix.CreateRotationY(rotationY);
			lookAt = Vector3.Transform (lookAt, rotation) + position;
			lookAt = new Vector3 (lookAt.X, 0, lookAt.Z);

			view = Matrix.CreateLookAt (position, lookAt, Vector3.Up);
			projection = Matrix.CreatePerspectiveFieldOfView (MathHelper.ToRadians (55f), aspectRatio, 2f, 2000f);

			//Look normal for culling
			lookNormal = position - lookAt;
			lookNormal.Normalize();

			UpdateMouse(gameTime);

		}

		private void UpdateMouse(GameTime gameTime)
		{
			up =  Vector3.Transform (new Vector3(0, 1, 0),rotation);
			right =  Vector3.Transform (new Vector3(-1, 0, 0),rotation);
			forward = Vector3.Transform (new Vector3(0, 0, 1),rotation);

			/*
			 * WASD to move
			 */
			if (Core.keyboardState.IsKeyDown (Keys.NumPad8)) {
				position += forward * Core.tick * Core.mapSpeed;
			} else if (Core.keyboardState.IsKeyDown (Keys.NumPad2)) {
				position -= forward * Core.tick * Core.mapSpeed;
			} 
			if (Core.keyboardState.IsKeyDown (Keys.NumPad4)) {
				position -= right * Core.tick * Core.mapSpeed; 
			} else if (Core.keyboardState.IsKeyDown (Keys.NumPad6)) {
				position += right * Core.tick * Core.mapSpeed;
			} 

			/*
			 * QE to rotate
			 */
			if (Core.keyboardState.IsKeyDown (Keys.NumPad9)) {
				rotationY += 1f * Core.tick;
			} else if (Core.keyboardState.IsKeyDown (Keys.NumPad7)) {
				rotationY -= 1f * Core.tick;
			}

			/*
			 * Scroll wheel
			 */
			float scrollWheelDelta = (float)(Core.oldMouseState.ScrollWheelValue - Core.mouseState.ScrollWheelValue);

			position = new Vector3(position.X, position.Y + (scrollWheelDelta/100)*Core.scrollSpeed, position.Z);

		}

		private void UpdateKeyboard(GameTime gameTime)
		{

		}

		public void Focus(Hex hex)
		{
			position = new Vector3 (hex.position.X, position.Y, hex.position.Z);
		}

		//Neccessary premature optimization
		public CullState GetHexCullState(Hex hex)
		{

            float Distance = Vector3.DistanceSquared(this.lookAt, hex.position);
			//Otherwise recalculate
			if ( Distance > farDistance) {
				return CullState.Culled;

			} else {

				//Check if hex is behind camera
				Vector3 hexNormal = hex.position - this.position;
				hexNormal.Normalize();

				float theta = Vector3.Dot (hexNormal, this.lookNormal);
				if (theta > (-0.62f))
					return CullState.Culled;
			}

            //Check close or far
            if (Distance > closeDistance)
                return CullState.Far;

			return CullState.Close;
		}

		public Boolean Visible(Vector3 pos)
		{
			//Otherwise recalculate
			if (Vector3.DistanceSquared (this.lookAt, pos) > 2200f) {
				return false;

			} else {

				//Check if hex is behind camera
				Vector3 hexNormal = pos - this.position;
				hexNormal.Normalize();

				float theta = Vector3.Dot (hexNormal, this.lookNormal);
				if (theta > (-0.62f))
					return false;
			}

			return true;
		}

        public Ray PickRay()
        {
            
            Vector3 nearSource = new Vector3((float)Core.mouseState.X, (float)Core.mouseState.Y, 0f);
            Vector3 farSource = new Vector3((float)Core.mouseState.X, (float)Core.mouseState.Y, 1f);
            Matrix world = Matrix.CreateTranslation(Vector3.Zero);


            Vector3 nearPoint = Core.graphicsDevice.Viewport.Unproject(nearSource,
                                                                        this.projection,
                                                                        this.view, world);

            Vector3 farPoint = Core.graphicsDevice.Viewport.Unproject(farSource,
                                                                        this.projection,
                                                                        this.view, world);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            Ray pickRay = new Ray(nearPoint, direction);
            return pickRay;
        }


	}
}

