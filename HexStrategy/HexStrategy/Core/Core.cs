using System;
using System.Collections.Generic;
using System.Text;
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

        public static float scrollSpeed = 5f;

		public static int screenX = 1440;
		public static int screenY = 900;
		public static float contrast = 1.6f;

		public static Vector4 ambientLight = new Vector4(1f, 1f, 1f, 1f);
        public static Vector3 ambientLight3 = new Vector3(1f, 1f, 1f);

		public static Vector3 sunDirection = new Vector3(0.2f, -1f, 0.2f);
		public static Vector4 sunDiffuse = new Vector4(1f,0.75f,0.65f, 1f);
        public static Vector3 sunDiffuse3 = new Vector3(1f, 0.75f, 0.65f);

		public static float ambientIntensity = 0.6f;
		public static float diffuseIntensity = 1.4f;


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

        public static String RandomTownName()
        {
            int rnd = random.Next(10);

            switch (rnd)
            {

                case 1:
                    return "Constantinople";
                    break;

                case 2:
                    return "Rome";
                    break;

                case 3:
                    return "Cairo";
                    break;

                case 4:
                    return "Beijing";
                    break;

                case 5:
                    return "London";
                    break;

                case 6:
                    return "Paris";
                    break;

                case 7:
                    return "Tokyo";
                    break;

                case 8:
                    return "Madrid";

                    break;
                case 9:
                    return "Moscow";
                    break;

                case 10:
                    return "Lisbon";
                    break;
            }

            return "Error";
        }

        public static string ToRoman(int number)
        {
            if (-9999 >= number || number >= 9999)
            {
                throw new ArgumentOutOfRangeException("Number too large to convert to Roman numeral");
            }

            if (number == 0)
            {
                return "NUL";
            }

            StringBuilder sb = new StringBuilder(10);

            if (number < 0)
            {
                sb.Append('-');
                number *= -1;
            }

            string[,] table = new string[,] { 
        { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" }, 
        { "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" }, 
        { "", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM" },
        { "", "M", "MM", "MMM", "M(V)", "(V)", "(V)M", 
                                          "(V)MM", "(V)MMM", "M(X)" } 
    };

            for (int i = 1000, j = 3; i > 0; i /= 10, j--)
            {
                int digit = number / i;
                sb.Append(table[j, digit]);
                number -= digit * i;
            }

            return sb.ToString();
        }

        public static class Maths
        {
            public static bool IsVector2InsideRect(Vector2 vec, Rectangle rect)
            {
                if (!(vec.X > (rect.X)))
                    return false;

                if (!(vec.X < (rect.X + rect.Width)))
                    return false;

                if (!(vec.Y > (rect.Y)))
                    return false;

                if (!(vec.Y < (rect.Y + rect.Height)))
                    return false;

                return true;
            }

            public static Matrix GetRotation(Vector3 source, Vector3 dest, Vector3 up)
            {
                float dot = Vector3.Dot(source, dest);

                if (Math.Abs(dot - (-1.0f)) < 0.000001f)
                {
                    // vector a and b point exactly in the opposite direction, 
                    // so it is a 180 degrees turn around the up-
                    return Matrix.Identity;
                }
                if (Math.Abs(dot - (1.0f)) < 0.000001f)
                {
                    // vector a and b point exactly in the same direction
                    // so we return the identity quaternion
                    return Matrix.Identity;
                }

                float rotAngle = (float)Math.Acos(dot);
                Vector3 rotAxis = Vector3.Cross(source, dest);
                rotAxis = Vector3.Normalize(rotAxis);
                return Matrix.CreateFromAxisAngle(rotAxis, rotAngle);
            }

            public static double AngleBetween(Vector2 v1, Vector2 v2)
            {
                //Calculate the distance from the square to the mouse's X and Y position
                float XDistance = v1.X - v2.X;
                float YDistance = v1.Y - v2.Y;

                //Calculate the required rotation by doing a two-variable arc-tan
                return (float)Math.Atan2(YDistance, XDistance);
            }

            public static float GetRotation(Vector3 NegativeDirection)
            {
                float Rotation = (float)Math.Atan2(NegativeDirection.X, NegativeDirection.Z);
                return Rotation;
            }

            public static float GetRotationFromVec3(Vector3 src, Vector3 dest)
            {
                Vector3 normal = dest - src;
                normal.Normalize();

                return GetRotation(normal);
            }
            public static float UnsignedAngleBetweenTwoV3(Vector3 first, Vector3 second)
            {
                first.Normalize(); second.Normalize();
                double Angle = (float)Math.Acos(Vector3.Dot(first, second));
                return (float)Angle;
            }
            public static String DisplayDecimalAsPercentage(float decmial)
            {
                return Math.Round(decmial * 100, 0) + "%";
            }
            public static String DisplayDecimalAsPercentageIncrease(float decmial, int digits)
            {
                String thing = "";
                if (decmial > 0)
                    thing += "+";

                thing += Math.Round(decmial * 100, digits) + "%";

                return thing;
            }
            public static String DisplayDecimalAsFloatingPercentage(float decmial, int digits)
            {
                return Math.Round(decmial * 100, digits) + "%";
            }
        }


	}
}

