using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;
using XmasHell.Performance;
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
        public DebugScreen DebugScreen;
        public MainMenuScreen MainMenuScreen;
        public GameScreen GameScreen;

#if ANDROID
        private XmasHellActivity _activity;
#endif

        // Performance
        public PerformanceManager PerformanceManager;

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

            Graphics.IsFullScreen = true;
            Graphics.ApplyChanges();
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
            PerformanceManager = new PerformanceManager(this);
        }

        protected override void Initialize()
        {
            ViewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y);

            Camera = new Camera(this, ViewportAdapter);

            SpriteBatchManager.Initialize();

            base.Initialize();

            Pause = false;

            GameManager.Initialize();

            // Screens
            ScreenComponent screenComponent;
            Components.Add(screenComponent = new ScreenComponent(this));

            if (GameConfig.DebugScreen)
            {
                DebugScreen = new DebugScreen(this);
                screenComponent.Register(DebugScreen);

                DebugScreen.Show();
            }
            else
            {
                MainMenuScreen = new MainMenuScreen(this);
                GameScreen = new GameScreen(this);

                screenComponent.Register(MainMenuScreen);
                screenComponent.Register(GameScreen);
            }

            // Input manager
            Components.Add(new InputManager(this));

            PerformanceManager.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

#if ANDROID
            Assets.SetActivity(_activity);
#endif

            Assets.Load(Content, GraphicsDevice);

            SpriteBatchManager.LoadContent();

            SpriteBatchManager.GradientBackground = new GradientBackground(this);
        }

        protected override void UnloadContent()
        {
            SpriteBatchManager.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            PerformanceManager.StartStopwatch(PerformanceStopwatchType.GlobalUpdate);

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

            Camera.Update(gameTime);

            if (!GameManager.EndGame())
                SpriteBatchManager.Update(gameTime);

            GameManager.Update(gameTime);

            PerformanceManager.StartStopwatch(PerformanceStopwatchType.PerformanceManagerUpdate);
            PerformanceManager.StopStopwatch(PerformanceStopwatchType.GlobalUpdate);
            PerformanceManager.StopStopwatch(PerformanceStopwatchType.PerformanceManagerUpdate);

            PerformanceManager.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            PerformanceManager.StartStopwatch(PerformanceStopwatchType.GlobalDraw);

            PerformanceManager.StartStopwatch(PerformanceStopwatchType.ClearColorDraw);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            PerformanceManager.StopStopwatch(PerformanceStopwatchType.ClearColorDraw);

            PerformanceManager.StartStopwatch(PerformanceStopwatchType.SpriteBatchManagerDraw);
            SpriteBatchManager.Draw();
            PerformanceManager.StopStopwatch(PerformanceStopwatchType.SpriteBatchManagerDraw);

            base.Draw(gameTime);

            if (GameConfig.DisplayCollisionBoxes)
                GameManager.CollisionWorld.Draw();

            PerformanceManager.StartStopwatch(PerformanceStopwatchType.PerformanceManagerDraw);
            PerformanceManager.Draw(gameTime);
            PerformanceManager.StopStopwatch(PerformanceStopwatchType.PerformanceManagerDraw);

            PerformanceManager.StopStopwatch(PerformanceStopwatchType.GlobalDraw);
        }
    }
}
