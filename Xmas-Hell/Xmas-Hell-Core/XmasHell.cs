using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;
using XmasHell.Screens;
using XmasHell.Shaders;
using XmasHell.Sprites;
using Xmas_Hell_Core.Controls;

#if ANDROID
using Xmas_Hell_Android;
#endif

namespace XmasHell
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class XmasHell : Game
    {
        public GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;
        public SpriteBatchManager SpriteBatchManager;
        public ViewportAdapter ViewportAdapter;
        public Camera Camera;
        public GameManager GameManager;

        public bool Pause;

        // Screens
        public MainMenuScreen MainMenuScreen;
        public GameScreen GameScreen;

#if ANDROID
        private XmasHellActivity _activity;
#endif

        private Sprite _backgroundSprite;

        // Performance
        private FramesPerSecondCounterComponent _fpsCounter;
        private Stopwatch _stopWatch;
        private TimeSpan _updateTime;
        private TimeSpan _drawTime;

#if ANDROID
        public XmasHell(XmasHellActivity activity)
#else
        public XmasHell()
#endif
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

#if ANDROID
            Graphics.SupportedOrientations = DisplayOrientation.Portrait;
            _activity = activity;

            // Used for bloom effect
            Graphics.PreferredDepthStencilFormat = DepthFormat.Depth16;
#else
            Graphics.IsFullScreen = false;
            Window.AllowUserResizing = true;
            IsMouseVisible = true;
            //Graphics.PreferredBackBufferWidth = GameConfig.VirtualResolution.X;
            //Graphics.PreferredBackBufferHeight = GameConfig.VirtualResolution.Y;

            Graphics.PreferredBackBufferWidth = 480;
            Graphics.PreferredBackBufferHeight = 853;

            // Unlock FPS
            //IsFixedTimeStep = false;
            //Graphics.SynchronizeWithVerticalRetrace = false;
#endif

            GameManager = new GameManager(this);
            SpriteBatchManager = new SpriteBatchManager(this);
        }

        protected override void Initialize()
        {
            ViewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y);

            Camera = new Camera(this, ViewportAdapter);

            _stopWatch = new Stopwatch();

            SpriteBatchManager.Initialize();

            base.Initialize();

            Pause = false;

            GameManager.Initialize();

            // Screens
            ScreenComponent screenComponent;
            Components.Add(screenComponent = new ScreenComponent(this));

            MainMenuScreen = new MainMenuScreen(this);
            GameScreen = new GameScreen(this);

            screenComponent.Register(MainMenuScreen);
            screenComponent.Register(GameScreen);

            // Input manager
            Components.Add(new InputManager(this));

            // FPS counter
            Components.Add(_fpsCounter = new FramesPerSecondCounterComponent(this));
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

#if ANDROID
            Assets.SetActivity(_activity);
#endif

            Assets.Load(Content, GraphicsDevice);

            SpriteBatchManager.LoadContent();

            _backgroundSprite = new Sprite(
                new TextureRegion2D(
                    Assets.GetTexture2D("Graphics/Pictures/background"),
                    0, 0, GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y
                )
            )
            {
                Origin = Vector2.Zero
            };

            SpriteBatchManager.BackgroundSprites.Add(_backgroundSprite);
        }

        protected override void UnloadContent()
        {
            SpriteBatchManager.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            _stopWatch.Reset();
            _stopWatch.Start();

            if (InputManager.KeyPressed(Keys.P))
                Pause = !Pause;

            if (GameConfig.EnableBloom)
            {
                // Switch to the next bloom settings preset?
                if (InputManager.KeyPressed(Keys.C))
                {
                    SpriteBatchManager.BloomSettingsIndex = (SpriteBatchManager.BloomSettingsIndex + 1) %
                                                            BloomSettings.PresetSettings.Length;
                    SpriteBatchManager.Bloom.Settings =
                        BloomSettings.PresetSettings[SpriteBatchManager.BloomSettingsIndex];
                }
                // Cycle through the intermediate buffer debug display modes?
                if (InputManager.KeyPressed(Keys.X))
                {
                    SpriteBatchManager.Bloom.ShowBuffer++;
                    if (SpriteBatchManager.Bloom.ShowBuffer > Bloom.IntermediateBuffer.FinalResult)
                        SpriteBatchManager.Bloom.ShowBuffer = 0;
                }
            }

            base.Update(gameTime);

            if (Pause)
                return;

            SpriteBatchManager.Update();
            GameManager.Update(gameTime);
            Camera.Update(gameTime);

            _updateTime = _stopWatch.Elapsed;
        }

        protected override void Draw(GameTime gameTime)
        {
            _stopWatch.Reset();
            _stopWatch.Start();

            GraphicsDevice.Clear(Color.CornflowerBlue);

            _drawTime = _stopWatch.Elapsed;

            SpriteBatchManager.Draw();

            base.Draw(gameTime);

            if (GameConfig.DisplayCollisionBoxes)
                GameManager.CollisionWorld.Draw();

            if (GameConfig.ShowDebugInfo)
            {
                SpriteBatch.Begin(
                    samplerState: SamplerState.PointClamp,
                    blendState: BlendState.AlphaBlend,
                    transformMatrix: ViewportAdapter.GetScaleMatrix()
                );

                SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"), $"FPS: {_fpsCounter.FramesPerSecond:0}",
                    Vector2.Zero, Color.White);
                SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"),
                    $"Player's bullets: {GameManager.GetPlayerBullets().Count:0}", new Vector2(0, 20), Color.White);
                SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"),
                    $"Boss' bullets: {GameManager.GetBossBullets().Count:0}", new Vector2(0, 40), Color.White);

                SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"),
                    "Active particles: " + GameManager.ParticleManager.ActiveParticlesCount(), new Vector2(0, 60),
                    Color.White);

                SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"),
                    $"Update time: {_updateTime.TotalMilliseconds} ms", new Vector2(0, 80), Color.White
                );
                SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"),
                    $"Draw time: { _drawTime.TotalMilliseconds } ms", new Vector2(0, 100), Color.White
                );

                if (GameConfig.EnableBloom)
                {
                    SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"),
                        "C = settings (" + SpriteBatchManager.Bloom.Settings.Name + ")", new Vector2(0, 120),
                        Color.White);
                    SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"),
                        "X = show buffer (" + SpriteBatchManager.Bloom.ShowBuffer + ")", new Vector2(0, 140),
                        Color.White);
                }

                SpriteBatch.End();
            }
        }
    }
}
