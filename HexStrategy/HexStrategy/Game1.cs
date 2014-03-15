
#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

#endregion

namespace HexStrategy
{

    public class Game1 : Game
    {

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Scenery scenery;
        BloomComponent bloom;

        public Game1()
        {

            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";

            graphics.IsFullScreen = false;
            graphics.PreferMultiSampling = true;

            graphics.PreferredBackBufferHeight = Core.screenY;
            graphics.PreferredBackBufferWidth = Core.screenX;

            graphics.ApplyChanges();
            this.IsMouseVisible = true;

            bloom = new BloomComponent(this);
            Components.Add(bloom);

        }

        protected override void Initialize()
        {
            Core.camera = new Camera(graphics.GraphicsDevice.Viewport.AspectRatio);
            Core.graphicsDevice = graphics.GraphicsDevice;
            Core.graphicsDevice.RasterizerState = RasterizerState.CullNone;
            base.Initialize();
        }


        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            Meshes.LoadContent(Content);
            Textures.LoadContent(Content);
            Shaders.LoadContent(Content);
            Fonts.LoadContent(Content);

            Map map = new Map();
            Core.map = map;

            Core.userFaction = Core.factions[35];
            Core.camera.Focus(Core.userFaction.hexes()[0]);

            scenery = new Scenery();

            DTShader dtShader = new DTShader();
            Core.dtShader = dtShader;
            Render.Initialize();

            UserInterface.LoadElements();
            //Core.map.TestSetup ();
        }

        protected override void Update(GameTime gameTime)
        {
            Core.BeginUpdate(gameTime);
            Core.map.Update(gameTime);
            Core.camera.Update(gameTime);
            UserInterface.Update(gameTime);
            Core.FinishUpdate(gameTime);
            base.Update(gameTime);

        }


        protected override void Draw(GameTime gameTime)
        {
            bloom.BeginDraw();
            graphics.GraphicsDevice.Clear(new Color(20, 100, 255));

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            Core.map.Draw3D();

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (Faction faction in Core.factions)
                faction.Draw();

            base.Draw(gameTime);



            spriteBatch.Begin();
            Core.map.Draw2D(spriteBatch);

            UserInterface.Draw(spriteBatch);

            spriteBatch.DrawString(Fonts.small, (1 / gameTime.ElapsedGameTime.TotalSeconds).ToString(), new Vector2(2, 2), Color.White);
            spriteBatch.End();


        }


    }
}
