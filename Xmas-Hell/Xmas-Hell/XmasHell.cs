using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
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
        public SpriteBatchManager SpriteBatchManager;
        public ViewportAdapter ViewportAdapter;
        public Camera Camera;
        public GameManager GameManager;
        private XmasHellActivity _activity;
        private KeyboardState _oldKeyboardState;
        private bool _pause;

        private Sprite _backgroundSprite;

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
            SpriteBatchManager = new SpriteBatchManager(this);
        }

        protected override void Initialize()
        {
            ViewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y);

            Camera = new Camera(this, ViewportAdapter);

            SpriteBatchManager.Initialize();

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
                SpriteBatchManager.BloomSettingsIndex = (SpriteBatchManager.BloomSettingsIndex + 1) % BloomSettings.PresetSettings.Length;
                SpriteBatchManager.Bloom.Settings = BloomSettings.PresetSettings[SpriteBatchManager.BloomSettingsIndex];
            }
            // Cycle through the intermediate buffer debug display modes?
            if (IsPressed(Keys.X))
            {
                SpriteBatchManager.Bloom.ShowBuffer++;
                if (SpriteBatchManager.Bloom.ShowBuffer > Bloom.IntermediateBuffer.FinalResult)
                    SpriteBatchManager.Bloom.ShowBuffer = 0;
            }

            _oldKeyboardState = Keyboard.GetState();

            if (_pause)
                return;

            SpriteBatchManager.Update();
            GameManager.Update(gameTime);

            Camera.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            SpriteBatchManager.Draw();

            base.Draw(gameTime);

            SpriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: ViewportAdapter.GetScaleMatrix());

            SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"), $"FPS: {_fpsCounter.AverageFramesPerSecond:0}", Vector2.Zero, Color.White);
            SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"), $"Player's bullets: {GameManager.GetPlayerBullets().Count:0}", new Vector2(0, 20), Color.White);
            SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"), $"Boss' bullets: {GameManager.GetBossBullets().Count:0}", new Vector2(0, 40), Color.White);

            SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"), "A = settings (" + SpriteBatchManager.Bloom.Settings.Name + ")", new Vector2(0, 60), Color.White);
            SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"), "X = show buffer (" + SpriteBatchManager.Bloom.ShowBuffer + ")", new Vector2(0, 80), Color.White);

            SpriteBatch.End();
        }
    }
}
