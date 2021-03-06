﻿using Microsoft.Xna.Framework;
using XmasHell.Physics;
using XmasHell.Physics.Collision;
using XmasHell.Spriter;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;
using XmasHell.Geometry;
using XmasHell.Rendering;

namespace XmasHell.Entities.Bosses.XmasSnowman
{
    class Hat : ISpriterPhysicsEntity, IMovable
    {
        private readonly XmasSnowman _boss;
        private CollisionElement _boundingBox;
        private CustomSpriterAnimator _animator;
        private Vector2 _initialPosition;
        public bool Destroyed = false;
        private float _speed;
        private Laser _laser;
        private bool _scaleDownLaser;

        public CustomSpriterAnimator GetCurrentAnimator()
        {
            return _animator;
        }

        public Vector2 Position()
        {
            return _animator.Position;
        }

        Vector2 IMovable.Position
        {
            get { return Position(); }
            set { Position(value); }
        }

        public void Position(Vector2 value)
        {
            _animator.Position = value;
        }

        public float Rotation()
        {
            return _animator.Rotation;
        }

        public void Rotation(float value)
        {
            _animator.Rotation = value;
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

        public Hat(XmasSnowman boss, CustomSpriterAnimator animator, Vector2 position)
        {
            _boss = boss;
            _animator = animator.Clone();
            _speed = 200;

            _initialPosition = position;
            _scaleDownLaser = true;

            Position(_initialPosition);

            // Physics
            _boundingBox = new SpriterCollisionCircle(this, "hat.png", Vector2.Zero, 0.7f);

            _boss.AddHitBox(_boundingBox);

            // Animations
            _animator.Play("Idle");

            ChangeHorizontalPosition();
            ShootLaser();

            _boss.Game.SpriteBatchManager.AddSpriterAnimator(_animator, Layer.FRONT);
        }

        public void Dispose()
        {
            _boss.Game.SpriteBatchManager.RemoveSpriterAnimator(_animator, Layer.FRONT);
            _boss.Game.GameManager.CollisionWorld.RemoveBossBulletHitbox(_boundingBox);

            if (_laser != null)
            {
                _boss.Game.SpriteBatchManager.Lasers.Remove(_laser);
                _laser = null;
            }

            Destroyed = true;
        }

        private void ShootLaser()
        {
            _laser = new Laser(_boss.Game, new Line(Position(), Position() + Vector2.UnitY * GameConfig.VirtualResolution.Y), 4f);
            _boss.Game.SpriteBatchManager.Lasers.Add(_laser);
        }

        private void ChangeHorizontalPosition()
        {
            this.CreateTweenChain(ChangeHorizontalPosition)
                .MoveTo(new Vector2(_boss.GetRandomHorizontalPosition(), _initialPosition.Y), 2f, EasingFunctions.ElasticInOut)
                ;
        }

        public void Update(GameTime gameTime)
        {
            _animator.Update(gameTime.ElapsedGameTime.Milliseconds);

            if (_laser != null)
            {
                _laser.SetStartPoint(Position());
                _laser.SetEndPoint(Position() + Vector2.UnitY * GameConfig.VirtualResolution.Y);

                var scaleAmount = gameTime.GetElapsedSeconds() * 10f;
                if (_scaleDownLaser)
                    _laser.ScaleX(_laser.ScaleVector().X - scaleAmount);
                else
                    _laser.ScaleX(_laser.ScaleVector().X + scaleAmount);

                if (_scaleDownLaser && _laser.ScaleVector().X < 3f)
                    _scaleDownLaser = false;
                else if (_laser.ScaleVector().X > 4f)
                    _scaleDownLaser = true;

                _laser.Update(gameTime);
            }
        }
    }
}
