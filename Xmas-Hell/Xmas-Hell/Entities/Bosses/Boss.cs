using System;
using System.Collections.Generic;
using BulletML;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using SpriterDotNet;
using SpriterDotNet.MonoGame;
using Xmas_Hell.BulletML;
using Xmas_Hell.Entities.Bosses;
using Xmas_Hell.Geometry;
using Xmas_Hell.Physics;

namespace Xmas_Hell.Entities
{
    public abstract class Boss : IPhysicsEntity
    {
        public XmasHell Game;
        protected Vector2 InitialPosition;
        protected float InitialLife;
        protected float Life;
        public bool Invincible;

        public Vector2 Direction = Vector2.Zero; // values in radians
        public float Speed;
        public Vector2 Acceleration = Vector2.One;
        public float AngularVelocity = 5f;

        // Relative to position targeting
        public bool TargetingPosition = false;
        private Vector2 _initialPosition = Vector2.Zero;
        private Vector2 _targetPosition = Vector2.Zero;
        private TimeSpan _targetPositionTimer = TimeSpan.Zero;
        private TimeSpan _targetPositionTime = TimeSpan.Zero;
        private Vector2 _targetDirection = Vector2.Zero;

        // Relative to angle targeting
        public bool TargetingAngle = false;
        private float _initialAngle = 0f;
        private float _targetAngle = 0f;
        private TimeSpan _targetAngleTimer = TimeSpan.Zero;
        private TimeSpan _targetAngleTime = TimeSpan.Zero;

        private readonly PositionDelegate _playerPositionDelegate;

        private TimeSpan _hitTimer = TimeSpan.Zero;

        // Behaviours
        protected readonly List<AbstractBossBehaviour> Behaviours;
        protected int PreviousBehaviourIndex;
        protected int CurrentBehaviourIndex;

        // BulletML
        protected readonly Dictionary<string, BulletPattern> BossPatterns;
        protected readonly List<string> BulletPatternFiles;

        // Spriter

        protected static readonly Config DefaultAnimatorConfig = new Config
        {
            MetadataEnabled = false,
            EventsEnabled = false,
            PoolingEnabled = true,
            TagsEnabled = false,
            VarsEnabled = false,
            SoundsEnabled = false
        };

        protected IList<MonoGameAnimator> Animators = new List<MonoGameAnimator>();
        public MonoGameAnimator CurrentAnimator;

        #region Getters

        public virtual Vector2 Position()
        {
            var currentPosition = CurrentAnimator.Position;
            if (CurrentAnimator.FrameData != null && CurrentAnimator.FrameData.SpriteData.Count > 0)
            {
                var spriteData = CurrentAnimator.FrameData.SpriteData[0];
                return currentPosition + new Vector2(spriteData.X, -spriteData.Y);
            }

            return currentPosition;
        }

        public virtual float Rotation()
        {
            if (CurrentAnimator.FrameData != null && CurrentAnimator.FrameData.SpriteData.Count > 0)
            {
                var spriteData = CurrentAnimator.FrameData.SpriteData[0];
                return MathHelper.ToRadians(-spriteData.Angle);
            }

            return CurrentAnimator.Rotation;
        }

        public virtual Vector2 Scale()
        {
            if (CurrentAnimator.FrameData != null && CurrentAnimator.FrameData.SpriteData.Count > 0)
            {
                var spriteData = CurrentAnimator.FrameData.SpriteData[0];
                return new Vector2(spriteData.ScaleX, spriteData.ScaleY);
            }

            return CurrentAnimator.Scale;
        }

        public Vector2 ActionPointPosition()
        {
            if (CurrentAnimator.FrameData != null && CurrentAnimator.FrameData.PointData.ContainsKey("action_point"))
            {
                var actionPoint = CurrentAnimator.FrameData.PointData["action_point"];
                return Position() + new Vector2(actionPoint.X, -actionPoint.Y);
            }

            return Position();
        }

        public virtual float Width()
        {
            if (CurrentAnimator.SpriteProvider != null)
            {
                return CurrentAnimator.SpriteProvider.Get(0, 0).Width;
            }

            return 0f;
        }

        public virtual float Height()
        {
            if (CurrentAnimator.SpriteProvider != null)
            {
                return CurrentAnimator.SpriteProvider.Get(0, 0).Height;
            }

            return 0f;
        }

        #endregion

        #region Setters

        public void Position(Vector2 value)
        {
            CurrentAnimator.Position = value;
        }

        public void Rotation(float value)
        {
            CurrentAnimator.Rotation = value;
        }

        public void Scale(Vector2 value)
        {
            CurrentAnimator.Scale = value;
        }

        #endregion

        protected Boss(XmasHell game, PositionDelegate playerPositionDelegate)
        {
            Game = game;
            _playerPositionDelegate = playerPositionDelegate;

            InitialPosition = new Vector2(
                GameConfig.VirtualResolution.X / 2f,
                150f
            );
            InitialLife = GameConfig.BossDefaultLife;

            // Behaviours
            Behaviours = new List<AbstractBossBehaviour>();


            // BulletML
            BossPatterns = new Dictionary<string, BulletPattern>();
            BulletPatternFiles = new List<string>();
        }

        public void Initialize()
        {
            LoadBulletPatterns();

            Reset();
        }

        protected virtual void Reset()
        {
            Game.GameManager.MoverManager.Clear();
            Life = InitialLife;
            CurrentAnimator.Position = InitialPosition;
            Invincible = false;

            Direction = Vector2.Zero;
            Speed = GameConfig.BossDefaultSpeed;
            CurrentBehaviourIndex = 0;
            PreviousBehaviourIndex = -1;
        }

        private void RestoreDefaultState()
        {
            Direction = Vector2.Zero;
            CurrentAnimator.Play("Idle");
        }

        private void LoadBulletPatterns()
        {
            foreach (var bulletPatternFile in BulletPatternFiles)
            {
                var pattern = new BulletPattern();
                pattern.ParseStream(bulletPatternFile, Assets.GetPattern(bulletPatternFile));
                BossPatterns.Add(bulletPatternFile, pattern);
            }
        }

        public void Destroy()
        {
            Game.GameManager.ParticleManager.EmitBossDestroyedParticles(CurrentAnimator.Position);
            Reset();
        }

        // Move to a given position in "time" seconds
        public void MoveTo(Vector2 position, float time, bool force = false)
        {
            if (TargetingPosition && !force)
                return;

            TargetingPosition = true;
            _targetPositionTimer = TimeSpan.FromSeconds(time);
            _targetPositionTime = TimeSpan.FromSeconds(time);
            _targetPosition = position;
            _initialPosition = CurrentAnimator.Position;
        }

        // Move to a given position keeping the actual speed
        public void MoveTo(Vector2 position, bool force = false)
        {
            if (TargetingPosition && !force)
                return;

            TargetingPosition = true;
            _targetPosition = position;
            _targetDirection = Vector2.Normalize(position - CurrentAnimator.Position);
        }

        public void RotateTo(float angle, float time, bool force = false)
        {
            if (TargetingAngle && !force)
                return;

            TargetingAngle = true;
            _targetAngle = angle;
            _initialAngle = CurrentAnimator.Rotation;
            _targetAngleTimer = TimeSpan.FromSeconds(time);
            _targetAngleTime = TimeSpan.FromSeconds(time);
        }

        public void RotateTo(float angle, bool force = false)
        {
            if (TargetingAngle && !force)
                return;

            TargetingAngle = true;
            _targetAngle = angle;
        }

        public void StopMoving()
        {
            TargetingPosition = false;
            _targetPositionTimer = TimeSpan.Zero;
            _targetDirection = Vector2.Zero;
        }

        public Vector2 GetPlayerPosition()
        {
            return _playerPositionDelegate();
        }

        public Vector2 GetPlayerDirection()
        {
            var playerPosition = GetPlayerPosition();
            var currentPosition = CurrentAnimator.Position;
            var angle = (currentPosition - playerPosition).ToAngle();

            angle += MathHelper.PiOver2;

            return MathHelperExtension.AngleToDirection(angle);
        }

        protected void CurrentAnimator_EventTriggered(string obj)
        {
            System.Diagnostics.Debug.WriteLine(obj);
        }

        public void TriggerPattern(string patternName,  BulletType type, bool clear = false, Vector2? position = null)
        {
            if (clear)
                Game.GameManager.MoverManager.Clear();

            Game.GameManager.MoverManager.CurrentBulletType = type;

            // Add a new bullet in the center of the screen
            var mover = (Mover)Game.GameManager.MoverManager.CreateBullet(true);

            if (position != null)
                mover.Position(position.Value);
            else
                mover.Position(ActionPointPosition());

            mover.InitTopNode(BossPatterns[patternName].RootNode);
        }

        public void TakeDamage(float amount)
        {
            if (Invincible)
                return;

            Life -= amount;

            if (Life < 0f)
                Destroy();

            _hitTimer = TimeSpan.FromMilliseconds(20);
        }

        public virtual void Update(GameTime gameTime)
        {
            UpdatePosition(gameTime);
            UpdateRotation(gameTime);
            UpdateBehaviour(gameTime);

            if (_hitTimer.TotalMilliseconds > 0)
                _hitTimer -= gameTime.ElapsedGameTime;

            CurrentAnimator.Update(gameTime.ElapsedGameTime.Milliseconds);
        }

        private void UpdatePosition(GameTime gameTime)
        {
            if (TargetingPosition)
            {
                if (!_targetDirection.Equals(Vector2.Zero))
                {
                    var currentPosition = CurrentAnimator.Position;
                    var distance = Vector2.Distance(currentPosition, _targetPosition);
                    var deltaDistance = Speed * gameTime.GetElapsedSeconds();

                    if (distance < deltaDistance)
                    {
                        TargetingPosition = false;
                        _targetDirection = Vector2.Zero;
                        CurrentAnimator.Position = _targetPosition;
                    }
                    else
                    {
                        // TODO: Perform some cubic interpolation
                        CurrentAnimator.Position = currentPosition + (_targetDirection * deltaDistance) * Acceleration;
                    }
                }
                else
                {
                    var newPosition = Vector2.Zero;
                    var lerpAmount = (float) (_targetPositionTime.TotalSeconds/_targetPositionTimer.TotalSeconds);

                    newPosition.X = MathHelper.SmoothStep(_targetPosition.X, _initialPosition.X, lerpAmount);
                    newPosition.Y = MathHelper.SmoothStep(_targetPosition.Y, _initialPosition.Y, lerpAmount);

                    if (lerpAmount < 0.001f)
                    {
                        TargetingPosition = false;
                        _targetPositionTimer = TimeSpan.Zero;
                        CurrentAnimator.Position = _targetPosition;
                    }
                    else
                        _targetPositionTime -= gameTime.ElapsedGameTime;

                    CurrentAnimator.Position = newPosition;
                }
            }
            else
            {
                CurrentAnimator.Position += Speed * gameTime.GetElapsedSeconds() * Acceleration * Direction;
            }
        }

        private void UpdateRotation(GameTime gameTime)
        {
            if (TargetingAngle)
            {
                // TODO: Add some logic to know if the boss has to turn to the left or to the right

                if (_targetAngleTimer.TotalMilliseconds <= 0)
                {
                    var currentRotation = CurrentAnimator.Rotation;
                    var distance = Math.Abs(currentRotation - _targetAngle);
                    var deltaDistance = AngularVelocity*gameTime.GetElapsedSeconds();

                    if (distance < deltaDistance)
                    {
                        TargetingAngle = false;
                        CurrentAnimator.Rotation = _targetAngle;
                    }
                    else
                    {
                        var factor = (currentRotation < _targetAngle) ? 1 : -1;
                        CurrentAnimator.Rotation = currentRotation + (factor * deltaDistance);
                    }
                }
                else
                {
                    var lerpAmount = (float)(_targetAngleTime.TotalSeconds / _targetAngleTimer.TotalSeconds);
                    var newAngle = MathHelper.Lerp(_targetAngle, _initialAngle, lerpAmount);

                    if (lerpAmount < 0.001f)
                    {
                        TargetingAngle = false;
                        _targetAngleTimer = TimeSpan.Zero;
                        CurrentAnimator.Rotation = _targetAngle;
                    }
                    else
                        _targetAngleTime -= gameTime.ElapsedGameTime;

                    CurrentAnimator.Rotation = newAngle;
                }
            }
        }

        private void UpdateBehaviour(GameTime gameTime)
        {
            UpdateBehaviourIndex();

            if (CurrentBehaviourIndex != PreviousBehaviourIndex)
            {
                if (PreviousBehaviourIndex > 0)
                    Behaviours[PreviousBehaviourIndex].Stop();

                Game.GameManager.MoverManager.Clear();
                RestoreDefaultState();
                Behaviours[CurrentBehaviourIndex].Start();
            }

            Behaviours[CurrentBehaviourIndex].Update(gameTime);

            PreviousBehaviourIndex = CurrentBehaviourIndex;
        }

        protected virtual void UpdateBehaviourIndex()
        {
            CurrentBehaviourIndex = (int)Math.Floor((1f - (Life / InitialLife)) * Behaviours.Count) % Behaviours.Count;
        }

        public void Draw(GameTime gameTime)
        {
            CurrentAnimator.Color = _hitTimer.TotalMilliseconds > 0 ? new Color(Color.White, 0.1f) : Color.White;

            CurrentAnimator.Draw(Game.SpriteBatch);

            Behaviours[CurrentBehaviourIndex].Draw(Game.SpriteBatch);

            var percent = Life / InitialLife;
            Game.SpriteBatch.Draw(
                Assets.GetTexture2D("pixel"),
                new Rectangle(0, 0, (int)(percent * GameConfig.VirtualResolution.X), 20),
                Color.Black
            );
        }
    }
}