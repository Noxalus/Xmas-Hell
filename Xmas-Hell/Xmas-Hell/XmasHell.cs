using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens;
using MonoGame.Extended.ViewportAdapters;
using Xmas_Hell.Screens;
using Xmas_Hell.Shaders;

namespace Xmas_Hell
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class XmasHell : Game
    {
        public GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;
        public ViewportAdapter ViewportAdapter;
        public Camera Camera;
        public GameManager GameManager;
        private XmasHellActivity _activity;
        private KeyboardState _oldKeyboardState;
        private bool _pause;

        // Bloom
        private Bloom _bloom;
        private int _bloomSettingsIndex = 0;
        private RenderTarget2D _renderTarget1;
        private RenderTarget2D _renderTarget2;

        private FramesPerSecondCounterComponent _fpsCounter;

        public XmasHell(XmasHellActivity activity)
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Graphics.IsFullScreen = true;
            Graphics.SupportedOrientations = DisplayOrientation.Portrait;

            // Used for bloom effect
            Graphics.PreferredDepthStencilFormat = DepthFormat.Depth16;

            _activity = activity;

            GameManager = new GameManager(this);
        }

        protected override void Initialize()
        {
            ViewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y);

            Camera = new Camera(this, ViewportAdapter);

            _bloom = new Bloom(GraphicsDevice, SpriteBatch);

            var pp = GraphicsDevice.PresentationParameters;

            _renderTarget1 = new RenderTarget2D(
                GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false,
                pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents
            );
            _renderTarget2 = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false,
                pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents
            );

            base.Initialize();

            _pause = false;

            GameManager.Initialize();

            ScreenComponent screenComponent;
            Components.Add(screenComponent = new ScreenComponent(this));

            //screenComponent.Register(new MainMenuScreen(this));
            screenComponent.Register(new GameScreen(this));

            Components.Add(_fpsCounter = new FramesPerSecondCounterComponent(this));
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            Assets.Load(_activity, Content, GraphicsDevice);

            _bloom.LoadContent(Content, GraphicsDevice.PresentationParameters);
        }

        protected override void UnloadContent()
        {
            _bloom.UnloadContent();
            _renderTarget1.Dispose();
            _renderTarget2.Dispose();
        }

        private bool IsPressed(Keys key)
        {
            KeyboardState state = Keyboard.GetState();
            return _oldKeyboardState.IsKeyUp(key) && state.IsKeyDown(key);
        }

        protected override void Update(GameTime gameTime)
        {
            if (IsPressed(Keys.P))
                _pause = !_pause;

            // Switch to the next bloom settings preset?
            if (IsPressed(Keys.A))
            {
                _bloomSettingsIndex = (_bloomSettingsIndex + 1) % BloomSettings.PresetSettings.Length;
                _bloom.Settings = BloomSettings.PresetSettings[_bloomSettingsIndex];
            }
            // Cycle through the intermediate buffer debug display modes?
            if (IsPressed(Keys.X))
            {
                _bloom.ShowBuffer++;
                if (_bloom.ShowBuffer > Bloom.IntermediateBuffer.FinalResult)
                    _bloom.ShowBuffer = 0;
            }

            _oldKeyboardState = Keyboard.GetState();

            if (_pause)
                return;

            GameManager.Update(gameTime);

            Camera.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // The next draw calls will be rendered in the first render target
            GraphicsDevice.SetRenderTarget(_renderTarget1);
            GraphicsDevice.Clear(Color.Transparent);

            GameManager.DrawBloomedSprites(gameTime);

            // Apply bloom effect on the first render target and store the
            // result into the second render target
            _bloom.Draw(_renderTarget1, _renderTarget2);

            // We want to render into the back buffer from now on
            GraphicsDevice.SetRenderTarget(null);

            SpriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: ViewportAdapter.GetScaleMatrix());
            SpriteBatch.Draw(
                Assets.GetTexture2D("Graphics/Pictures/background"),
                new Rectangle(0, 0, GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y),
                Color.White
            );
            SpriteBatch.End();

            base.Draw(gameTime);

            GameManager.Draw(gameTime);

            // Draw the second render target on top of everything
            SpriteBatch.Begin(0, BlendState.AlphaBlend);
            SpriteBatch.Draw(_renderTarget2, new Rectangle(
                0, 0,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight
            ), Color.White);
            SpriteBatch.End();

            SpriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: ViewportAdapter.GetScaleMatrix());

            SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"), $"FPS: {_fpsCounter.AverageFramesPerSecond:0}", Vector2.Zero, Color.White);
            SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"), $"Player's bullets: {GameManager.GetPlayerBullets().Count:0}", new Vector2(0, 20), Color.White);
            SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"), $"Boss' bullets: {GameManager.GetBossBullets().Count:0}", new Vector2(0, 40), Color.White);

            SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"), "A = settings (" + _bloom.Settings.Name + ")", new Vector2(0, 60), Color.White);
            SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"), "X = show buffer (" + _bloom.ShowBuffer.ToString() + ")", new Vector2(0, 80), Color.White);

            SpriteBatch.End();
        }
    }
}
