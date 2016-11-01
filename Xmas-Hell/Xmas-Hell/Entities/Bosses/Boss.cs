using System;
using System.Collections.Generic;
using System.Linq;
using Android.Text.Style;
using BulletML;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using SpriterDotNet;
using SpriterDotNet.MonoGame;
using Xmas_Hell.BulletML;
using Xmas_Hell.Physics;

namespace Xmas_Hell.Entities
{
    public abstract class Boss : IPhysicsEntity
    {
        public XmasHell Game;
        protected Vector2 InitialPosition;
        protected float InitialLife;
        protected float Life;

        public float Direction = 0; // angle in radians
        public float Speed = 200f;
        public Vector2 Acceleration = Vector2.One;

        public bool TargetingPosition = false;
        private Vector2 _initialPosition = Vector2.Zero;
        private Vector2 _targetPosition = Vector2.Zero;
        private TimeSpan _targetPositionTimer = TimeSpan.Zero;
        private TimeSpan _targetPositionTime = TimeSpan.Zero;
        private Vector2 _targetDirection = Vector2.Zero;

        // BulletML
        protected Dictionary<string, BulletPattern> BossPatterns;
        protected List<string> BulletPatternFiles;

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

        protected Boss(XmasHell game)
        {
            Game = game;

            InitialLife = GameConfig.BossDefaultLife;
            InitialPosition = new Vector2(
                GameConfig.VirtualResolution.X / 2f,
                150f
            );

            // BulletML
            BossPatterns = new Dictionary<string, BulletPattern>();
            BulletPatternFiles = new List<string>();
        }

        public void Initialize()
        {
            LoadBulletPatterns();

            Reset();
        }

        private void Reset()
        {
            Game.GameManager.MoverManager.Clear();
            Life = InitialLife;
            CurrentAnimator.Position = InitialPosition;
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
        public void MoveTo(Vector2 position, float time)
        {
            if (TargetingPosition)
                return;

            TargetingPosition = true;
            _targetPositionTimer = TimeSpan.FromSeconds(time);
            _targetPositionTime = TimeSpan.FromSeconds(time);
            _targetPosition = position;
            _initialPosition = CurrentAnimator.Position;
        }

        // Move to a given position keeping the actual speed
        public void MoveTo(Vector2 position)
        {
            if (TargetingPosition)
                return;

            TargetingPosition = true;
            _targetPosition = position;
            _targetDirection = Vector2.Normalize(position - CurrentAnimator.Position);
            _initialPosition = CurrentAnimator.Position;
        }

        protected void CurrentAnimator_EventTriggered(string obj)
        {
            System.Diagnostics.Debug.WriteLine(obj);
        }

        public void AddBullet(string patternName,  BulletType type, bool clear = false)
        {
            if (clear)
                Game.GameManager.MoverManager.Clear();

            Game.GameManager.MoverManager.CurrentBulletType = type;

            // Add a new bullet in the center of the screen
            var mover = (Mover)Game.GameManager.MoverManager.CreateBullet(true);
            mover.Position(ActionPointPosition());
            mover.InitTopNode(BossPatterns[patternName].RootNode);
        }

        public void TakeDamage(float amount)
        {
            Life -= amount;

            if (Life < 0f)
                Destroy();
        }

        public virtual void Update(GameTime gameTime)
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
                        CurrentAnimator.Position = currentPosition + (_targetDirection * deltaDistance);
                    }
                }
                else
                {
                    var newPosition = Vector2.Zero;
                    var lerpAmount = (float)(_targetPositionTime.TotalSeconds / _targetPositionTimer.TotalSeconds);

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
        }

        public void Draw(GameTime gameTime)
        {
            CurrentAnimator.Draw(Game.SpriteBatch);

            var percent = Life / InitialLife;
            Game.SpriteBatch.Draw(
                Assets.GetTexture2D("pixel"),
                new Rectangle(0, 0, (int)(percent * GameConfig.VirtualResolution.X), 20),
                Color.Black
            );
        }
    }
}