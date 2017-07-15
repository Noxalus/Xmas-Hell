using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XmasHell.Physics;
using XmasHell.Geometry;
using XmasHell.Physics.Collision;
using XmasHell.Entities.Bosses;
using XmasHell.Extensions;

namespace XmasHell.Entities
{
    public class Laser : IPhysicsEntity, IBossDeadlyEntity
    {
        protected XmasHell _game;
        public Line _line;
        public Texture2D _laserTexture;
        private Vector2 _scale;
        private Vector2 _origin;
        private CollisionConvexPolygon _hitbox;
        private bool _used;

        public Vector2 Position()
        {
            return _line.First;
        }

        public float Rotation()
        {
            return MathExtension.LineToAngle(_line);
        }

        public Vector2 Scale()
        {
            return _scale;
        }

        public Vector2 Origin()
        {
            return _origin;
        }

        public void TakeDamage(float damage)
        {
            throw new NotImplementedException();
        }

        public Laser(XmasHell game, Line line, float scale = 1f)
        {
            _game = game;
            _line = line;
            _used = true;

            LoadContent();
            _scale = new Vector2(scale, line.Distance() / _laserTexture.Height);

            var vertices = new List<Vector2>
            {
                Vector2.Zero,
                new Vector2(_laserTexture.Width, 0),
                new Vector2(_laserTexture.Width, _laserTexture.Height),
                new Vector2(0, _laserTexture.Height)
            };

            _hitbox = new CollisionConvexPolygon(this, -_origin, vertices);
            _game.GameManager.CollisionWorld.AddBossBulletHitbox(_hitbox);
        }

        public void Initialize()
        {
        }

        public void LoadContent()
        {
            _laserTexture = Assets.GetTexture2D("Graphics/Sprites/Bullets/laser");
            _origin = new Vector2(_laserTexture.Width / 2f, 0f);
        }

        public void Update(GameTime gameTime)
        {
            var d = (100f + (float)gameTime.TotalGameTime.TotalMilliseconds / 10);

            _line.Second = new Vector2(
                _game.ViewportAdapter.Center.ToVector2().X + (float)Math.Cos(gameTime.TotalGameTime.TotalSeconds) * d,
                _game.ViewportAdapter.Center.ToVector2().Y + (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds) * d
            );

            _scale.X += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _scale.Y = (float)Math.Ceiling(_line.Distance() / _laserTexture.Height);
        }

        public void Draw(GameTime gameTime)
        {
            _game.SpriteBatch.Draw(
                _laserTexture, Position(), null, Color.White, Rotation(), _origin, _scale, SpriteEffects.None, 1f
            );

            _game.SpriteBatch.Draw(
                Assets.GetTexture2D("pixel"),
                new Rectangle(
                    _line.First.ToPoint() - new Point(10 / 2, 10 / 2),
                    new Point(10, 10)
                ),
                Color.Orange
            );

            _game.SpriteBatch.Draw(
                Assets.GetTexture2D("pixel"),
                new Rectangle(
                    _line.Second.ToPoint() - new Point(10 / 2, 10 / 2),
                    new Point(10, 10)
                ),
                Color.Green
            );
        }

        public bool Used()
        {
            return _used;
        }

        public void Used(bool value)
        {
            _used = value;
        }

        public void Destroy()
        {
            _used = false;

            _game.GameManager.RemoveLaser(this);
            _game.GameManager.CollisionWorld.RemoveBossBulletHitbox(_hitbox);
        }
    }
}
