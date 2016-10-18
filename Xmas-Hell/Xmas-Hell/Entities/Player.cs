using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;

namespace Xmas_Hell.Entities
{
    class Player
    {
        private readonly XmasHell _game;
        private Sprite _sprite;

        public Player(XmasHell game)
        {
            _game = game;
        }

        public void LoadContent()
        {
            var playerTexture = _game.Content.Load<Texture2D>("Graphics/Sprites/player");

            _sprite = new Sprite(playerTexture)
            {
                Origin = new Vector2(playerTexture.Width / 2f, playerTexture.Height / 2f),
                Position = new Vector2(720f / 2f + (playerTexture.Width / 2f), 1280f - 150),
                Scale = Vector2.One
            };
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw()
        {
            _game.SpriteBatch.Draw(_sprite);
        }
    }
}