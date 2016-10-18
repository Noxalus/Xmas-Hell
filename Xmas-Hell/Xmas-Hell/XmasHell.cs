using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BulletML;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Sprites;
using Xmas_Hell.Entities;

namespace Xmas_Hell
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class XmasHell : Game
    {
        public GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;

        private Player _player;
        private Sprite _bulletSprite;
        private BitmapFont _font;

        public XmasHell()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Graphics.IsFullScreen = true;
            Graphics.PreferredBackBufferWidth = 720;
            Graphics.PreferredBackBufferHeight = 1280;
            Graphics.SupportedOrientations = DisplayOrientation.Portrait;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _player = new Player(this);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            _player.LoadContent();

            var bulletTexture = Content.Load<Texture2D>(@"Graphics/Sprites/bullet");

            _bulletSprite = new Sprite(bulletTexture)
            {
                Origin = new Vector2(bulletTexture.Width / 2f, bulletTexture.Height / 2f),
                Position = new Vector2(720f / 2f + (bulletTexture.Width / 2f), 150),
                Scale = Vector2.One
            };

            _font = Content.Load<BitmapFont>(@"Graphics/Fonts/main");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteBatch.Begin();

            _player.Draw();

            _bulletSprite.Draw(SpriteBatch);

            SpriteBatch.End();

            SpriteBatch.Begin();

            SpriteBatch.DrawString(_font, "COUCOU", Vector2.Zero, Color.White);

            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
