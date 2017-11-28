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

        public void Position(Vector2 value)
        {
            _line.First = value;
            _line.Second.X = value.X;
        }

        public float Rotation()
        {
            return MathExtension.LineToAngle(_line);
        }

        public Vector2 ScaleVector()
        {
            return _scale;
        }

        public void ScaleX(float value)
        {
            _scale.X = value;
        }

        public Vector2 Origin()
        {
            return _origin;
        }

        public void TakeDamage(float damage)
        {
            throw new NotImplementedException();
        }

        public void SetStartPoint(Vector2 position)
        {
            _line.First = position;
        }

        public void SetEndPoint(Vector2 position)
        {
            _line.Second = position;

            // Make sure to update the laser scale
            if (_laserTexture != null)
                _scale = new Vector2(_scale.X, _line.Distance() / _laserTexture.Height);
        }

        public void SetLine(Line line)
        {
            _line = line;
        }

        public Laser(XmasHell game, Line line, float scale = 1f)
        {
            _game = game;
            _line = line;
            _used = true;

            LoadContent();

            var vertices = new List<Vector2>
            {
                Vector2.Zero,
                new Vector2(_laserTexture.Width, 0),
                new Vector2(_laserTexture.Width, _laserTexture.Height),
                new Vector2(0, _laserTexture.Height)
            };

            _hitbox = new CollisionConvexPolygon(this, -_origin, vertices);
            _game.GameManager.CollisionWorld.AddBossBulletHitbox(_hitbox);

            _scale = new Vector2(scale, line.Distance() / _laserTexture.Height);
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
            // TODO: Update UV
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                _laserTexture, Position(), null, Color.White, Rotation(), _origin, _scale, SpriteEffects.None, 1f
            );

            spriteBatch.Draw(
                Assets.GetTexture2D("pixel"),
                new Rectangle(
                    _line.First.ToPoint() - new Point(10 / 2, 10 / 2),
                    new Point(10, 10)
                ),
                Color.Orange
            );

            spriteBatch.Draw(
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
