using DrawListBuddy;
using FilenameBuddy;
using GameTimer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RenderBuddy;
using ResolutionBuddy;

namespace AnimationLib.Example
{
    /// <summary>
    /// The smallest possible game that loads an AnimationLib character and plays one of its animations.
    ///
    /// The flow every frame is:
    ///   Update: advance the clock -> pose the skeleton for the current animation time -> run ragdoll physics
    ///   Draw:   the skeleton writes its sprites into a DrawList -> the DrawList is drawn through the Renderer
    /// </summary>
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;

        /// <summary>
        /// Drives every AnimationLib character. Pass the same clock to all of them so they stay in sync,
        /// and pause/slow it down to pause/slow down the whole game.
        /// </summary>
        private readonly GameClock _clock = new GameClock();

        /// <summary>
        /// The character: a skeleton (bones + images) and the set of animations that move it.
        /// </summary>
        private readonly AnimationContainer _character = new AnimationContainer();

        /// <summary>
        /// A reusable list of sprites. The character renders itself into this each frame,
        /// then the list is drawn all at once in the right layer order.
        /// </summary>
        private readonly DrawList _drawList = new DrawList();

        /// <summary>
        /// Wraps the SpriteBatch, the lighting shader, and the camera.
        /// </summary>
        private Renderer _renderer;

        /// <summary>
        /// Where the character stands, in virtual-resolution (game) coordinates.
        /// </summary>
        private Vector2 _characterPosition;

        public Game1()
        {
            Content.RootDirectory = "Content";

            // HiDef is needed for the AnimationBuddyShader lighting effect.
            _graphics = new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef
            };

            // Render in a fixed 1280x720 coordinate space no matter how big the window is.
            // The component registers itself with the game, so we don't need to keep a reference to it.
            new ResolutionComponent(this, _graphics,
                virtualResolution: new Point(1280, 720),
                screenResolution: new Point(1280, 720),
                fullscreen: false,
                letterbox: true,
                useDeviceResolution: false);
        }

        protected override void LoadContent()
        {
            // --- Renderer ---
            // Lights must be set up *before* Renderer.LoadContent, which pushes them into the shader.
            // You can skip the lighting entirely and the Renderer will use one default light.
            _renderer = new Renderer(this, Content);
            _renderer.AmbientColor = new Color(.2f, .2f, .2f);
            _renderer.ClearLights();
            _renderer.AddDirectionalLight(new Vector3(0f, 1f, .1f), new Color(.2f, 0f, .3f));
            _renderer.AddDirectionalLight(new Vector3(-.5f, -1f, -.1f), new Color(1f, .7f, 0f, 0.75f));
            _renderer.AddDirectionalLight(new Vector3(.5f, -1f, .6f), new Color(1f, 1f, .75f));
            _renderer.LoadContent(GraphicsDevice);

            // --- Character ---
            // Paths are relative to the Content folder. Both XML files are built by the content pipeline
            // (see AnimationTest.mgcb), which is why the ContentManager is passed in.
            // The skeleton has to be loaded first, because the animations refer to its bones.
            _character.ReadSkeletonXml(new Filename("MegaMan/MegaManModel.xml"), _renderer, Content);
            _character.ReadAnimationXml(new Filename("MegaMan/MegaManAnimations.xml"), Content);

            // Animation names come from the animation file. Loop it forever.
            _character.SetAnimation("Walking", EPlayback.Loop);

            _characterPosition = Resolution.TitleSafeArea.Center.ToVector2();

            // --- Camera ---
            // The camera pans and zooms to keep every point added to it on screen.
            // Turn off the world boundary so it doesn't clamp to the Renderer's default world size,
            // then snap it straight to the character instead of easing in from the origin.
            _renderer.Camera.IgnoreWorldBoundary = true;
            _renderer.Camera.AddPoint(_characterPosition);
            _renderer.Camera.BeginScene(true);

            _clock.Start();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            _clock.Update(gameTime);

            // Camera.Update clears last frame's points, so add this frame's points after it.
            _renderer.Camera.Update(_clock);
            _renderer.Camera.AddPoint(_characterPosition);
            _renderer.Update(gameTime);

            // Pose the skeleton for the current point in the animation...
            _character.Update(_clock, _characterPosition, isFlipped: false, rotation: 0f, ignoreRagdoll: false);

            // ...then let any ragdoll bones (hair, cloth, etc.) swing on top of that pose.
            // Call this after Update every frame.
            _character.UpdateRagdoll();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            // Ease the camera toward the points added during Update.
            _renderer.Camera.BeginScene(false);

            // Empty out last frame's sprites, then have the character add this frame's.
            _drawList.Flush();
            _character.Render(_drawList);

            // Camera matrix first, then the resolution matrix to scale game coordinates to the window.
            _renderer.SpriteBatchBegin(BlendState.AlphaBlend,
                _renderer.Camera.TranslationMatrix * Resolution.TransformationMatrix());
            _drawList.Render(_renderer);
            _renderer.SpriteBatchEnd();

            base.Draw(gameTime);
        }
    }
}
