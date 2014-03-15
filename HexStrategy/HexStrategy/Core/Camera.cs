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

		public Vector3 position = new Vector3(0,60,0);
		public Matrix view, projection, rotation;

		
		public float closeDistance = 2200f;
        public float farDistance = 12000f;

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
			projection = Matrix.CreatePerspectiveFieldOfView (MathHelper.ToRadians (51f), aspectRatio, 2f, 2000f);

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
			if (Core.keyboardState.IsKeyDown (Keys.W)) {
				position += forward * Core.tick * Core.mapSpeed;
			} else if (Core.keyboardState.IsKeyDown (Keys.S)) {
				position -= forward * Core.tick * Core.mapSpeed;
			} 
			if (Core.keyboardState.IsKeyDown (Keys.A)) {
				position -= right * Core.tick * Core.mapSpeed; 
			} else if (Core.keyboardState.IsKeyDown (Keys.D)) {
				position += right * Core.tick * Core.mapSpeed;
			} 

			/*
			 * QE to rotate
			 */
			if (Core.keyboardState.IsKeyDown (Keys.Q)) {
				rotationY += 1f * Core.tick;
			} else if (Core.keyboardState.IsKeyDown (Keys.E)) {
				rotationY -= 1f * Core.tick;
			}

			/*
			 * Scroll wheel
			 */
			float scrollWheelDelta = (float)(Core.oldMouseState.ScrollWheelValue - Core.mouseState.ScrollWheelValue);

			position = new Vector3(position.X, position.Y + (scrollWheelDelta/100), position.Z);

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
	}
}

