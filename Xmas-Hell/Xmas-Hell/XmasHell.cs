using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens;
using MonoGame.Extended.ViewportAdapters;
using Xmas_Hell.Screens;

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
        public GameManager GameManager;
        private XmasHellActivity _activity;

        private FramesPerSecondCounterComponent _fpsCounter;

        public XmasHell(XmasHellActivity activity)
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Graphics.IsFullScreen = true;
            Graphics.SupportedOrientations = DisplayOrientation.Portrait;

            _activity = activity;

            GameManager = new GameManager(this);
        }

        protected override void Initialize()
        {
            ViewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y);

            base.Initialize();

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
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            GameManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: ViewportAdapter.GetScaleMatrix());
            SpriteBatch.Draw(Assets.GetTexture2D("Graphics/Pictures/background"), new Rectangle(0, 0, 720, 1280), Color.White);
            SpriteBatch.End();

            base.Draw(gameTime);

            GameManager.Draw(gameTime);

            SpriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: ViewportAdapter.GetScaleMatrix());

            SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"), $"FPS: {_fpsCounter.AverageFramesPerSecond:0}", Vector2.Zero, Color.White);
            SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"), $"Player's bullets: {GameManager.GetPlayerBullets().Count:0}", new Vector2(0, 20), Color.White);
            SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"), $"Boss' bullets: {GameManager.GetBossBullets().Count:0}", new Vector2(0, 40), Color.White);

            SpriteBatch.End();
        }
    }
}
