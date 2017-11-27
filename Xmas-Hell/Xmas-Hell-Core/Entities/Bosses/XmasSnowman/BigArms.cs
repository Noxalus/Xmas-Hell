using Microsoft.Xna.Framework;
using XmasHell.Physics;
using XmasHell.Physics.Collision;
using XmasHell.Spriter;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;

namespace XmasHell.Entities.Bosses.XmasSnowman
{
    class BigArms : ISpriterPhysicsEntity, IMovable
    {
        private readonly XmasSnowman _boss;
        private List<CollisionElement> _boundingBoxes;
        private CustomSpriterAnimator _animator;
        private Vector2 _initialPosition;
        public bool Destroyed = false;
        private int _verticalDirection;
        private int _horizontalDirection;
        private float _speed;
        private bool _rightArm;
        private Point _horizontalLimits;

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

        public BigArms(XmasSnowman boss, CustomSpriterAnimator animator, bool rightArm)
        {
            _boss = boss;
            _animator = animator.Clone();
            _rightArm = rightArm;
            _verticalDirection = 1;
            _speed = 200;

            _initialPosition = new Vector2(GameConfig.VirtualResolution.X / 2f, GameConfig.VirtualResolution.Y / 2f + 300);

            Position(_initialPosition);

            // Physics
            _boundingBoxes = new List<CollisionElement>
            {
                new SpriterCollisionConvexPolygon(this, "big-arm.png", new Vector2(177f, 10f), new Vector2(0.1f, 0.95f)),
                new SpriterCollisionCircle(this, "big-arm.png", new Vector2(-50f, -250f), 0.1f),
                new SpriterCollisionCircle(this, "big-arm.png", new Vector2(-105f, -295f), 0.1f),
                new SpriterCollisionCircle(this, "big-arm.png", new Vector2(-160f, -340f), 0.1f),
                new SpriterCollisionCircle(this, "big-arm.png", new Vector2(50f, -250f), 0.1f),
                new SpriterCollisionCircle(this, "big-arm.png", new Vector2(105f, -295f), 0.1f),
                new SpriterCollisionCircle(this, "big-arm.png", new Vector2(160f, -340f), 0.1f),
            };

            foreach (var boundingBox in _boundingBoxes)
                _boss.Game.GameManager.CollisionWorld.AddBossHitBox(boundingBox);

            // Animations
            _animator.Play("Idle");

            var spriteSize = SpriterUtils.GetSpriterFileSize("big-arm.png", _animator);

            if (rightArm)
            {
                _horizontalDirection = -1;
                _horizontalLimits = new Point(GameConfig.VirtualResolution.X + 200, GameConfig.VirtualResolution.X + 200 + spriteSize.Y / 2);
                Rotation(MathHelper.Pi);
            }
            else
            {
                _horizontalDirection = 1;
                _horizontalLimits = new Point(-200 - spriteSize.Y / 2, -200);
            }

            Position(new Vector2(_horizontalLimits.X, 0));
            ChangeHorizontalPosition();
        }

        public void Dispose()
        {
            foreach (var boundingBox in _boundingBoxes)
                _boss.Game.GameManager.CollisionWorld.RemoveBossHitBox(boundingBox);

            _boundingBoxes.Clear();
            Destroyed = true;
        }

        private void ChangeHorizontalPosition()
        {
            this.CreateTweenChain(ChangeHorizontalPosition)
                .MoveTo(new Vector2(_boss.Game.GameManager.Random.Next(_horizontalLimits.X, _horizontalLimits.Y), _boss.GetRandomVerticalPosition()), 2f, EasingFunctions.ElasticInOut)
                ;
        }

        public void Update(GameTime gameTime)
        {
            //if (_verticalDirection == 1 && Position().Y >= GameConfig.VirtualResolution.Y)
            //    _verticalDirection = -1;
            //else if (_verticalDirection == -1 && Position().Y <= 0)
            //    _verticalDirection = 1;

            //Position(new Vector2(Position().X, Position().Y + (_speed * _verticalDirection * gameTime.GetElapsedSeconds())));
            _animator.Update(gameTime.ElapsedGameTime.Milliseconds);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _animator.Draw(spriteBatch);
        }
    }
}
