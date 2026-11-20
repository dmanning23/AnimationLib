using DrawListBuddy;
using GameTimer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using RenderBuddy;
using ResolutionBuddy;
using System;
using FilenameBuddy;

namespace AnimationLib.Example
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Properties

        GraphicsDeviceManager graphics;

        Renderer Renderer { get; set; }
        DrawList Drawlist { get; set; }
        GameClock Clock;

        public AnimationContainer Animations { get; private set; }

        Vector2 _center;

        #endregion //Properties

        #region Methods

        public Game1()
        {
            Content.RootDirectory = "Content";
            Drawlist = new DrawList();
            Clock = new GameClock();

            graphics = new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef
            };

            Animations = new AnimationContainer();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            var resolution = new ResolutionComponent(this, graphics, new Point(1280, 720), new Point(1280, 720), false, true, false);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Renderer = new Renderer(this, Content);
            Renderer.AmbientColor = new Color(.2f, .2f, .2f);
            Renderer.ClearLights();
            Renderer.AddDirectionalLight(new Vector3(0f, 1f, .1f), new Color(.2f, 0f, .3f));
            Renderer.AddDirectionalLight(new Vector3(-.5f, -1f, -.1f), new Color(1f, .7f, 0f, 0.75f));
            Renderer.AddDirectionalLight(new Vector3(.5f, -1f, .6f), new Color(1f, 1f, .75f));

            Renderer.Camera.IgnoreWorldBoundary = true;

            Renderer.LoadContent(GraphicsDevice);

            try
            {
                LoadModel(@"MegaMan\MegaManModel.xml",
                    @"MegaMan\MegaManAnimations.xml",
                    Content);
            }
            catch (Exception ex)
            {
                throw new Exception("Darn! Couldn't load the thing.", ex);
            }

            //start the time
            Clock.Start();
            Animations.SetAnimation("Walking", EPlayback.Loop);

            _center = Resolution.TitleSafeArea.Center.ToVector2();

            Renderer.Camera.BeginScene(true);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            base.Update(gameTime);

            Clock.Update(gameTime);

            Renderer.Camera.Update(Clock);
            Renderer.Update(gameTime);

            Animations.Update(Clock, _center, false, 0f, false);
            Animations.UpdateRagdoll();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.DarkGray);

            Renderer.Camera.AddPoint(_center);

            Renderer.Camera.BeginScene(false);

            //set up the drawlist
            Drawlist.Set(0.0f, Color.White, 1f);
            Drawlist.Flush();

            //draw the character
            Animations.Render(Drawlist);
            Renderer.SpriteBatchBegin(BlendState.AlphaBlend, Renderer.Camera.TranslationMatrix * Resolution.TransformationMatrix(), SpriteSortMode.Immediate);

            Drawlist.Render(Renderer);
            Renderer.SpriteBatchEnd();

            base.Draw(gameTime);
        }

        private void LoadModel(string modelFilename, string animationFilename, ContentManager content = null)
        {
            var modelFile = new Filename();
            var animationFile = new Filename();

            modelFile.SetRelFilename(modelFilename);
            animationFile.SetRelFilename(animationFilename);

            Animations.ReadSkeletonXml(modelFile, Renderer, content);
            Animations.ReadAnimationXml(animationFile, content);
        }

        #endregion //Methods
    }
}