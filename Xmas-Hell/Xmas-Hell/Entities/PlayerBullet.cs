using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;
using Xmas_Hell.Physics;

namespace Xmas_Hell.Entities
{
    public class PlayerBullet : Bullet
    {
        public PlayerBullet(XmasHell game, Vector2 position, float rotation, float speed) :
            base(game, position, rotation, speed)
        {
            Sprite.Color = Color.White * 0.2f;

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