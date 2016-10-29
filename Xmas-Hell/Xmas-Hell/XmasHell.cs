using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        private FramesPerSecondCounterComponent _fpsCounter;

        public XmasHell()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Graphics.IsFullScreen = true;
            Graphics.SupportedOrientations = DisplayOrientation.Portrait;
        }

        protected override void Initialize()
        {
            ViewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y);

            GameManager = new GameManager(this);

            base.Initialize();

            ScreenComponent screenComponent;
            Components.Add(screenComponent = new ScreenComponent(this));

            //screenComponent.Register(new MainMenuScreen(this));
            screenComponent.Register(new GameScreen(this));

            Components.Add(_fpsCounter = new FramesPerSecondCounterComponent(this));
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            Assets.Load(Content);
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

            GameManager.Draw(gameTime);

            base.Draw(gameTime);

            SpriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: ViewportAdapter.GetScaleMatrix());

            SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"), $"FPS: {_fpsCounter.AverageFramesPerSecond:0}", Vector2.Zero, Color.White);
            SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"), $"Player's bullets: {GameManager.GetPlayerBullets().Count:0}", new Vector2(0, 20), Color.White);
            SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"), $"Boss' bullets: {GameManager.GetBossBullets().Count:0}", new Vector2(0, 40), Color.White);

            SpriteBatch.End();
        }
    }
}
