using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace HexStrategy
{
	public static class Shaders
	{
		public static Effect dtShader, instanceShader;

		public static void LoadContent(ContentManager content)
		{

			dtShader = content.Load<Effect>("Shaders/Basic");
            instanceShader = content.Load<Effect>("Shaders/InstancedHex");
		}
	}
}

