using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ViewportAdapters;
using XmasHell.Background;
using XmasHell.Performance;
using XmasHell.Screens;
using XmasHell.Screens.Menu;
using XmasHell.Shaders;
using XmasHell.Rendering;
using XmasHell.Controls;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Tweening;
using XmasHell.Audio;
using XmasHell.Entities;
using XmasHell.GUI;
using XmasHell.PlayerData;

#if ANDROID
using XmasHellAndroid;
using Android.Util;
using Android.Content;
using XmasHell.PlayerData.Android;
#elif LINUX
using XmasHell.PlayerData.Desktop;
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
        public ScreenManager ScreenManager;
        public MusicManager MusicManager;
        public GuiManager GuiManager;
        public PlayerData.PlayerData PlayerData;
        public CloudManager CloudManager;

        public bool Pause;
        private bool _computeNextFrame = false;

#if ANDROID
        public XmasHellActivity AndroidActivity;
#endif

        // Performance
        public PerformanceManager PerformanceManager;

        private static XmasHell _instance = null;

        public static XmasHell Instance()
        {
            return _instance;
        }

#if ANDROID
        public XmasHell(XmasHellActivity activity)
#else
        public XmasHell()
#endif
        {
            _instance = this;
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

#if ANDROID
            Graphics.SupportedOrientations = DisplayOrientation.Portrait;
            AndroidActivity = activity;

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
            CloudManager = new CloudManager(this);
        }

        protected override void Initialize()
        {
            ViewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y);

            Camera = new Camera(this, ViewportAdapter);
            MusicManager = new MusicManager();

            SpriteBatchManager.Initialize();

            base.Initialize();

            Pause = false;

            GameManager.Initialize();

            // Input manager
            Components.Add(new InputManager(this));

            // Animation and tweening
            var animationComponent = new AnimationComponent(this);
            Components.Add(animationComponent);
            Components.Add(new TweeningComponent(this, animationComponent));

            // GUI
            GuiManager = new GuiManager(this);

            PerformanceManager.Initialize();

            // Player data
#if ANDROID
            ISharedPreferences prefs = AndroidActivity.GetSharedPreferences("XmasHell", FileCreationMode.Private);
            var androidPreferences = new AndroidPreferences(prefs);
            PlayerData = new PlayerData.PlayerData(androidPreferences);
#elif LINUX
            var desktopPreferences = new DesktopPreferences();
            PlayerData = new PlayerData.PlayerData(desktopPreferences);
#endif

            // Screens
            ScreenManager = new ScreenManager(this);

            if (GameConfig.DebugScreen)
            {
                ScreenManager.AddScreen(new DebugScreen(this));

                ScreenManager.GoTo<DebugScreen>();
            }
            else
            {
                ScreenManager.AddScreen(new MainMenuScreen(this));
                ScreenManager.AddScreen(new BossSelectionScreen(this));
                ScreenManager.AddScreen(new GameScreen(this));

                ScreenManager.GoTo<MainMenuScreen>();
            }
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

#if ANDROID
            Assets.SetActivity(AndroidActivity);
#endif

            Assets.Load(Content, GraphicsDevice);

            SpriteBatchManager.LoadContent();
            CloudManager.LoadContent();
            MusicManager.LoadContent();
            PerformanceManager.LoadContent();

            var gradientBackground = new GradientBackground(this);
            var level = BackgroundLevel.Level1;
            gradientBackground.ChangeGradientColors(GameConfig.BackgroundGradients[level].Item1, GameConfig.BackgroundGradients[level].Item2);
            SpriteBatchManager.Background = gradientBackground;
        }

#if ANDROID
        public void OnDestroy()
        {
            Log.Debug("XmasHell", "Destroy");
        }

        public void OnPause()
        {
            Log.Debug("XmasHell", "Pause");
        }

        public void OnResume()
        {
            Log.Debug("XmasHell", "Resume");
        }
#endif

        protected override void UnloadContent()
        {
            SpriteBatchManager.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            PerformanceManager.StartStopwatch(PerformanceStopwatchType.GlobalFrame);

            PerformanceManager.StartStopwatch(PerformanceStopwatchType.GlobalUpdate);

            HandleKeyboardInputs();

            base.Update(gameTime);

            if (Pause && !_computeNextFrame)
                return;

            MusicManager.Update(gameTime);

            Camera.Update(gameTime);

            ScreenManager.Update(gameTime);

            GuiManager.Update(gameTime);

            SpriteBatchManager.Update(gameTime);

            GameManager.Update(gameTime);

            PerformanceManager.StartStopwatch(PerformanceStopwatchType.PerformanceManagerUpdate);
            PerformanceManager.Update(gameTime);
            PerformanceManager.StopStopwatch(PerformanceStopwatchType.PerformanceManagerUpdate);

            PerformanceManager.StopStopwatch(PerformanceStopwatchType.GlobalUpdate);
        }

        private void HandleKeyboardInputs()
        {
            if (InputManager.KeyPressed(Keys.P))
                Pause = !Pause;

            if (InputManager.KeyPressed(Keys.N))
                _computeNextFrame = true;

            //if (InputManager.PressedCancel())
            //    ScreenManager.Back();

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

            if (InputManager.KeyPressed(Keys.F10))
            {
                Graphics.IsFullScreen = !Graphics.IsFullScreen;

                if (Graphics.IsFullScreen)
                {
                    Graphics.PreferredBackBufferWidth = GameConfig.VirtualResolution.X;
                    Graphics.PreferredBackBufferHeight = GameConfig.VirtualResolution.Y;
                    IsMouseVisible = true;
                    Window.AllowUserResizing = false;
                }
                else
                {
                    Graphics.PreferredBackBufferWidth = 480;
                    Graphics.PreferredBackBufferHeight = 853;
                    IsMouseVisible = true;
                    Window.AllowUserResizing = true;
                }

                Graphics.ApplyChanges();
            }

            // Debug
            if (InputManager.KeyPressed(Keys.F1))
                GameConfig.GodMode = !GameConfig.GodMode;
            else if (InputManager.KeyPressed(Keys.F2))
                GameConfig.DebugPhysics = !GameConfig.DebugPhysics;
            else if (InputManager.KeyPressed(Keys.F3))
                GameConfig.DisableCollision = !GameConfig.DisableCollision;
            else if (InputManager.KeyPressed(Keys.F4))
                GameConfig.EnableBloom = !GameConfig.EnableBloom;
            else if (InputManager.KeyPressed(Keys.F5))
                GameConfig.ShowPerformanceInfo = !GameConfig.ShowPerformanceInfo;
            else if (InputManager.KeyPressed(Keys.F6))
                GameConfig.ShowPerformanceGraph = !GameConfig.ShowPerformanceGraph;
        }

        protected override void Draw(GameTime gameTime)
        {
            PerformanceManager.StartStopwatch(PerformanceStopwatchType.GlobalDraw);

            PerformanceManager.StartStopwatch(PerformanceStopwatchType.ClearColorDraw);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            PerformanceManager.StopStopwatch(PerformanceStopwatchType.ClearColorDraw);

            PerformanceManager.StartStopwatch(PerformanceStopwatchType.SpriteBatchManagerDraw);
            SpriteBatchManager.Draw(SpriteBatch);
            PerformanceManager.StopStopwatch(PerformanceStopwatchType.SpriteBatchManagerDraw);

            base.Draw(gameTime);

            if (GameConfig.DebugPhysics)
                GameManager.CollisionWorld.Draw();

            PerformanceManager.StartStopwatch(PerformanceStopwatchType.PerformanceManagerDraw);
            PerformanceManager.Draw(gameTime);
            PerformanceManager.StopStopwatch(PerformanceStopwatchType.PerformanceManagerDraw);

            PerformanceManager.StopStopwatch(PerformanceStopwatchType.GlobalDraw);

            if (Pause)
                _computeNextFrame = false;

            PerformanceManager.StopStopwatch(PerformanceStopwatchType.GlobalFrame);
        }
    }
}
