using System;
using System.Collections.Generic;
using System.Linq;
using BulletML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using SpriterDotNet;
using SpriterDotNet.MonoGame;
using SpriterDotNet.Providers;
using XmasHell.Geometry;
using XmasHell.Physics;
using XmasHell.Spriter;
using Sprite = MonoGame.Extended.Sprites.Sprite;

namespace XmasHell.Entities.Bosses
{
    public enum BossType
    {
        XmasBall,
        XmasBell,
        XmasCandy,
        XmasSnowflake,
        XmasLog,
        XmasTree,
        XmasGift,
        XmasReinder,
        XmasSnowman,
        XmasSanta
    }

    public abstract class Boss : ISpriterPhysicsEntity
    {
        public XmasHell Game;
        protected Vector2 InitialPosition;
        protected float InitialLife;
        protected float Life;
        private Sprite _hpBar;

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
        public bool Tinted;
        public Color HitColor;

        // Behaviours
        protected readonly List<AbstractBossBehaviour> Behaviours;
        protected int PreviousBehaviourIndex;
        protected int CurrentBehaviourIndex;

        // BulletML
        protected readonly List<string> BulletPatternFiles;

        // Spriter
        protected string SpriterFilename;
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
            return CurrentAnimator.Position;
        }

        public virtual float Rotation()
        {
            return CurrentAnimator.Rotation;
        }

        public virtual Vector2 Scale()
        {
            return CurrentAnimator.Scale;
        }

        public MonoGameAnimator GetCurrentAnimator()
        {
            return CurrentAnimator;
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

        public float ActionPointDirection()
        {
            if (CurrentAnimator.FrameData != null && CurrentAnimator.FrameData.PointData.ContainsKey("action_point"))
            {
                var actionPoint = CurrentAnimator.FrameData.PointData["action_point"];
                return actionPoint.Angle;
            }

            return Rotation();
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

        protected float GetSpritePartWidth(string name)
        {
            if (CurrentAnimator != null)
            {
                var spriteBodyPart = Array.Find(CurrentAnimator.Entity.Spriter.Folders[0].Files, (file) => file.Name == name);
                return spriteBodyPart.Width;
            }

            return 0f;
        }

        protected float GetSpritePartHeight(string name)
        {
            if (CurrentAnimator != null)
            {
                var spriteBodyPart = Array.Find(CurrentAnimator.Entity.Spriter.Folders[0].Files, (file) => file.Name == name);
                return spriteBodyPart.Height;
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
            BulletPatternFiles = new List<string>();

            HitColor = Color.White * 0.5f;

            _hpBar = new Sprite(
                new TextureRegion2D(
                    Assets.GetTexture2D("pixel"),
                    0, 0, GameConfig.VirtualResolution.X, 20
                )
            )
            {
                Origin = Vector2.Zero
            };

            _hpBar.Color = Color.Red;

            Game.SpriteBatchManager.Boss = this;
            Game.SpriteBatchManager.UISprites.Add(_hpBar);
        }

        public virtual void Initialize()
        {
            LoadBulletPatterns();
            LoadSpriterSprite();
            InitializePhysics();

            Reset();
        }

        protected virtual void LoadSpriterSprite()
        {
            if (SpriterFilename == string.Empty)
                throw new Exception("You need to specify a path to the spriter file of this boss");

            var factory = new DefaultProviderFactory<SpriterDotNet.MonoGame.Sprite, SoundEffect>(DefaultAnimatorConfig, true);

            var loader = new SpriterContentLoader(Game.Content, SpriterFilename);
            loader.Fill(factory);

            foreach (var entity in loader.Spriter.Entities)
            {
                var animator = new MonoGameDebugAnimator(entity, Game.GraphicsDevice, factory);
                Animators.Add(animator);
            }

            CurrentAnimator = Animators.First();
            CurrentAnimator.Position = InitialPosition;

            CurrentAnimator.EventTriggered += CurrentAnimator_EventTriggered;
        }

        protected virtual void InitializePhysics()
        {
        }

        protected virtual void Reset()
        {
            Game.GameManager.MoverManager.Clear();
            Life = InitialLife;
            CurrentAnimator.Position = InitialPosition;
            Invincible = false;
            Tinted = false;

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
                Game.GameManager.MoverManager.AddPattern(bulletPatternFile, pattern);
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

            Tinted = _hitTimer.TotalMilliseconds > 0;

            var portion = (InitialLife/Behaviours.Count);
            var value = Life - (InitialLife - (CurrentBehaviourIndex + 1)*portion);

            _hpBar.Scale = new Vector2(value / portion, 1f);
            _hpBar.Color = GameConfig.BossHPBarColors[CurrentBehaviourIndex];

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

                if (Behaviours.Count > 0)
                    Behaviours[CurrentBehaviourIndex].Start();
            }

            if (Behaviours.Count > 0)
                Behaviours[CurrentBehaviourIndex].Update(gameTime);

            PreviousBehaviourIndex = CurrentBehaviourIndex;
        }

        protected virtual void UpdateBehaviourIndex()
        {
            if (Behaviours.Count == 0)
                return;

            CurrentBehaviourIndex = (int)Math.Floor((1f - (Life / InitialLife)) * Behaviours.Count) % Behaviours.Count;
        }

        public virtual void Draw()
        {
            CurrentAnimator.Draw(Game.SpriteBatch);
        }
    }
}