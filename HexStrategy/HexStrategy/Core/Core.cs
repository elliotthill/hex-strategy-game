using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HexStrategy
{
	public static class Core
	{

		public static Camera camera;

		public static MouseState mouseState, oldMouseState;
		public static KeyboardState keyboardState, oldKeyboardState;

		public static float tick;
		public static float mapSpeed = 35f;

		public static int screenX = 1280;
		public static int screenY = 720;
		public static float contrast = 1.6f;

		public static Vector4 ambientLight = new Vector4(1f, 1f, 1f, 1f);
		public static Vector3 sunDirection = new Vector3(0.2f, -1f, 0.2f);
		public static Vector4 sunDiffuse = new Vector4(1f,0.75f,0.65f, 1f);
		public static Vector3 selectedAmbientLight = new Vector3(1f,1f,1f);
		public static float ambientIntensity = 0.8f;
		public static float diffuseIntensity = 2f;


		public static Boolean leftClickLastFrame = false;
		public static Boolean rightClickLastFrame = false;

		public static GraphicsDevice graphicsDevice;

		public static Map map;


		public static DTShader dtShader;

		public static Random random = new Random();

		public static List<Faction> factions = new List<Faction> ();
		public static Faction userFaction;

		public static void BeginUpdate(GameTime gameTime)
		{
			mouseState = Mouse.GetState();
			keyboardState = Keyboard.GetState();

			tick = (float)gameTime.ElapsedGameTime.TotalSeconds;

			UpdateMouse();


		}

		private static void UpdateMouse()
		{
			if (mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
				leftClickLastFrame = true;
			else
				leftClickLastFrame = false;

			if (mouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton == ButtonState.Released)
				rightClickLastFrame = true;
			else
				rightClickLastFrame = false;
		}

		public static void FinishUpdate(GameTime gameTime)
		{
			oldMouseState = mouseState;
			oldKeyboardState = keyboardState;

		}

		public static float RandomFloat()
		{

			return (float)random.NextDouble ();
		}

		public static Color RandomColor()
		{
			return new Color (random.Next (255), random.Next (255), random.Next (255));
		}

		public static Vector3 RandomColorAsVector()
		{
			return new Vector3 ((float)random.NextDouble(), (float)random.NextDouble(),(float)random.NextDouble());
		}

		public static String RandomFactionName()
		{
			int rnd = random.Next(10);

			switch (rnd) {

			case 1:
				return "Mongols";
				break;

			case 2:
				return "Vandals";
				break;

			case 3:
				return "Aztecs";
				break;

			case 4:
				return "Mayans";
				break;

			case 5:
				return "Turks";
				break;

			case 6:
				return "Romans";
				break;

			case 7:
				return "Greeks";
				break;

			case 8:
				return "Goths";

				break;
			case 9:
				return "Gauls";
				break;

			case 10:
				return "Natives";
				break;
			}

			return "Error";
		}

	}
}

