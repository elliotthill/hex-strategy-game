
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
            Logger.AddMessage("Game constructed");
        }

        protected override void Initialize()
        {
            Core.camera = new Camera(graphics.GraphicsDevice.Viewport.AspectRatio);
            Core.graphicsDevice = graphics.GraphicsDevice;
            Core.graphicsDevice.RasterizerState = RasterizerState.CullNone;
            base.Initialize();
            Logger.AddMessage("Initialization complete");
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

            Core.factions.Add(new Faction("Castille", new Vector3(255f/255f,230f/255f,0)));
            Core.factions.Add(new Faction("Navarre", new Vector3(2550f, 108f/255f, 0)));
            Core.factions.Add(new Faction("Aragon", new Vector3(255f / 255f, 0, 66f / 255f)));
            Core.factions.Add(new Faction("Leon", new Vector3(90f/255f, 255f/255f, 0f)));

            Core.userFaction = Core.factions[2];
            



            scenery = new Scenery();

            DTShader dtShader = new DTShader();
            Core.dtShader = dtShader;
            Render.Initialize();
            RenderNormal.Initialize();
            UserInterface.Load();
            Logger.AddMessage("Loading complete");
        }

        protected override void Update(GameTime gameTime)
        {
            Core.BeginUpdate(gameTime);
            Core.camera.Update(gameTime);
            Clock.Update(gameTime);
            UserInterface.Update(gameTime);
            Core.map.Update(gameTime);

            foreach (Faction faction in Core.factions)
                faction.Update(gameTime);

            
            Core.FinishUpdate(gameTime);
            base.Update(gameTime);

        }


        protected override void Draw(GameTime gameTime)
        {
            bloom.BeginDraw();
            graphics.GraphicsDevice.Clear(new Color(20, 100, 255));

            /* 3D Draw */
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            Core.map.Draw3D();
            foreach (Faction faction in Core.factions)
                faction.DrawArmies();

            /* Component draw e.g. bloom */
            base.Draw(gameTime);

            /* 2D Draw */
            spriteBatch.Begin();
            Core.map.Draw2D(spriteBatch);

            foreach (Faction faction in Core.factions)
                faction.Draw2D(spriteBatch);

            UserInterface.Draw(spriteBatch);

            //spriteBatch.DrawString(Fonts.small, (1 / gameTime.ElapsedGameTime.TotalSeconds).ToString(), new Vector2(2, 2), Color.White);
            spriteBatch.End();


        }


    }
}
