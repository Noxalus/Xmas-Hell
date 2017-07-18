using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;
using XmasHell.Physics;
using XmasHell.Physics.Collision;

namespace XmasHell.Entities.Bosses.XmasCandy
{
    class XmasCandyBar : AbstractDrawableEntity, IPhysicsEntity
    {
        private readonly Boss _boss;
        private CollisionConvexPolygon _boundingBox;

        public float Rotation()
        {
            return Sprite.Rotation;
        }

        public Vector2 Origin()
        {
            return Sprite.Origin;
        }

        public Vector2 Scale()
        {
            return Sprite.Scale;
        }

        public void TakeDamage(float damage)
        {
            _boss.TakeDamage(damage * 0.05f);
        }

        public XmasCandyBar(Boss boss, Sprite sprite) : base(sprite)
        {
            _boss = boss;

            // Physics
            var bbWidth = sprite.TextureRegion.Width;
            var startX = -sprite.TextureRegion.Width / 2f;
            var startY = -sprite.TextureRegion.Height / 2f;
            var vertices = new List<Vector2>()
            {
                new Vector2(startX, startY),
                new Vector2(startX + bbWidth, startY),
                new Vector2(startX + bbWidth, startY + sprite.TextureRegion.Height),
                new Vector2(startX, startY + sprite.TextureRegion.Height)
            };

            var relativePosition = new Vector2(0, sprite.TextureRegion.Height / 2f);

            _boundingBox = new CollisionConvexPolygon(this, relativePosition, vertices);
            _boss.Game.GameManager.CollisionWorld.AddBossHitBox(_boundingBox);
        }

        public void Dispose()
        {
            _boss.Game.GameManager.CollisionWorld.RemoveBossHitBox(_boundingBox);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Sprite.Rotation += 0.01f;
        }
    }
}
