using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;

namespace Xmas_Hell.Entities
{
    public class PlayerBullet : Bullet
    {
        public PlayerBullet(XmasHell game, Vector2 position, float speed) : base(game, position, speed)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}