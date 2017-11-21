using Microsoft.Xna.Framework;
using XmasHell.Physics;
using XmasHell.Physics.Collision;
using XmasHell.Spriter;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace XmasHell.Entities.Bosses.XmasSnowman
{
    class BigArms : ISpriterPhysicsEntity
    {
        private readonly XmasSnowman _boss;
        private List<CollisionElement> _boundingBoxes;
        private CustomSpriterAnimator _animator;
        private Vector2 _initialPosition;
        public bool Destroyed = false;

        public CustomSpriterAnimator GetCurrentAnimator()
        {
            return _animator;
        }

        public Vector2 Position()
        {
            return _animator.Position;
        }

        public void Position(Vector2 value)
        {
            _animator.Position = value;
        }

        public float Rotation()
        {
            return _animator.Rotation;
        }

        public Vector2 Origin()
        {
            return _animator.Origin();
        }

        public Vector2 ScaleVector()
        {
            return _animator.Scale;
        }

        public void TakeDamage(float damage)
        {
            _boss.TakeDamage(damage * 0.25f);
        }

        public BigArms(XmasSnowman boss, CustomSpriterAnimator animator)
        {
            _boss = boss;
            _animator = animator.Clone();

            _initialPosition = new Vector2(GameConfig.VirtualResolution.X / 2f, GameConfig.VirtualResolution.Y / 2f + 300);

            Position(_initialPosition);

            // Physics
            _boundingBoxes = new List<CollisionElement>()
            {
                new SpriterCollisionConvexPolygon(_animator, "big-arm.png", new Vector2(177f, 10f), new Vector2(0.1f, 0.95f)),
                new SpriterCollisionCircle(_animator, "big-arm.png", new Vector2(-50f, -250f), 0.1f),
                new SpriterCollisionCircle(_animator, "big-arm.png", new Vector2(-105f, -295f), 0.1f),
                new SpriterCollisionCircle(_animator, "big-arm.png", new Vector2(-160f, -340f), 0.1f),
                new SpriterCollisionCircle(_animator, "big-arm.png", new Vector2(50f, -250f), 0.1f),
                new SpriterCollisionCircle(_animator, "big-arm.png", new Vector2(105f, -295f), 0.1f),
                new SpriterCollisionCircle(_animator, "big-arm.png", new Vector2(160f, -340f), 0.1f),
            };

            foreach (var boundingBox in _boundingBoxes)
                _boss.Game.GameManager.CollisionWorld.AddBossHitBox(boundingBox);

            // Animations
            _animator.Play("Idle");
        }

        public void Dispose()
        {
            foreach (var boundingBox in _boundingBoxes)
                _boss.Game.GameManager.CollisionWorld.RemoveBossBulletHitbox(boundingBox);

            Destroyed = true;
        }

        public void Update(GameTime gameTime)
        {
            _animator.Update(gameTime.ElapsedGameTime.Milliseconds);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _animator.Draw(spriteBatch);
        }
    }
}
