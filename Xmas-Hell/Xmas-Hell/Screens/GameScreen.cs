using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Sprites;
using Xmas_Hell.Entities;

namespace Xmas_Hell.Screens
{
    class GameScreen : Screen
    {
        private readonly XmasHell _game;
        private Player _player;
        private Sprite _bulletSprite;

        public GameScreen(XmasHell game)
        {
            _game = game;
            _player = new Player(game);
        }

        public override void Initialize()
        {
            _player = new Player(_game);

            base.Initialize();
        }

        public override void LoadContent()
        {
            _player.LoadContent();

            var bulletTexture = _game.Content.Load<Texture2D>(@"Graphics/Sprites/bullet");

            _bulletSprite = new Sprite(bulletTexture)
            {
                Origin = new Vector2(bulletTexture.Width / 2f, bulletTexture.Height / 2f),
                Position = new Vector2(Config.VirtualResolution.X / 2f + (bulletTexture.Width / 2f), 150),
                Scale = Vector2.One
            };
        }

        public override void Update(GameTime gameTime)
        {
            _player.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _game.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: _game.ViewportAdapter.GetScaleMatrix());

            _player.Draw();
            _bulletSprite.Draw(_game.SpriteBatch);

            _game.SpriteBatch.End();
        }
    }
}