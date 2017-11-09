using Microsoft.Xna.Framework;

namespace XmasHell.Background
{
    internal class BackgroundSnowflake
    {
        private XmasHell _game;
        private Vector2 _position;
        private Point _size;
        private float _speed;

        public BackgroundSnowflake(XmasHell game)
        {
            _game = game;
            _position = new Vector2(GameConfig.VirtualResolution.X / 2f, 0f);
            _size = new Point(20, 20);
            _speed = 100f;
        }

        public void Update(GameTime gameTime)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _position.Y += _speed * dt;

            if (_position.Y >= GameConfig.VirtualResolution.Y + _size.Y)
                _position.Y = -_size.Y;
        }

        public void Draw()
        {
            _game.SpriteBatch.Draw(Assets.GetTexture2D("pixel"), new Rectangle((int)_position.X, (int)_position.Y, _size.X, _size.Y), null, Color.White);
        }
    }
}