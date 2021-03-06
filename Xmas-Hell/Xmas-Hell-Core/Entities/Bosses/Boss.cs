using System;
using System.Collections.Generic;
using System.Linq;
using BulletML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended;
using SpriterDotNet;
using SpriterDotNet.MonoGame;
using SpriterDotNet.Providers;
using SpriterDotNet.MonoGame.Content;
using XmasHell.Geometry;
using XmasHell.Physics;
using XmasHell.Spriter;
using XmasHell.BulletML;
using XmasHell.Extensions;
using FarseerPhysics.Dynamics;
using FarseerPhysics;
using FarseerPhysics.Factories;
using XmasHell.Physics.DebugView;
using XmasHell.Physics.Collision;
using XmasHell.Audio;

namespace XmasHell.Entities.Bosses
{
    public enum ScreenSide
    {
        Left,
        Top,
        Right,
        Bottom
    };

    // TODO: Inherit from AbstractEntity
    public abstract class Boss : ISpriterPhysicsEntity, IDisposable
    {
        public XmasHell Game;
        public readonly BossType BossType;
        public Vector2 InitialPosition;
        private bool _destroyed;
        private bool _bossEntranceAnimation;
        private bool _ready;

        public bool Invincible;

        public Vector2 Direction = Vector2.Zero; // values in radians
        public float Speed;
        public Vector2 Acceleration = Vector2.One;
        public float AngularVelocity = 5f;

        // Physics World
        protected List<CollisionElement> HitBoxes;
        public bool PhysicsEnabled = false;
        public World PhysicsWorld;
        public Body PhysicsBody;
        protected Body LeftWallBody;
        protected Body TopWallBody;
        protected Body RightWallBody;
        protected Body BottomWallBody;
        private DebugView _debugView;

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
        public bool Tinted { get; protected set; }
        public Color HitColor;

        private readonly Line _leftWallLine;
        private readonly Line _bottomWallLine;
        private readonly Line _upWallLine;
        private readonly Line _rightWallLine;

        // Behaviours
        protected readonly List<AbstractBossBehaviour> Behaviours;
        protected int PreviousBehaviourIndex;
        public int CurrentBehaviourIndex { get; protected set; }

        // New position timer
        public bool StartNewPositionTimer = false;
        private TimeSpan NewPositionTimer = TimeSpan.Zero;
        public float NewPositionTimerTime = 0f;
        public event EventHandler<float> NewPositionTimerFinished = null;

        private bool _randomPosition;
        private bool _randomPositionLongDistance;
        public Rectangle RandomMovingArea;

        // Shoot timer
        public bool StartShootTimer = false;
        private TimeSpan ShootTimer = TimeSpan.Zero;
        public float ShootTimerTime = 0f;
        public event EventHandler<float> ShootTimerFinished = null;

        public bool IsOutside = false;

        // BulletML
        protected readonly List<string> BulletPatternFiles;

        // Spriter
        protected string SpriterFilename;
        protected static readonly Config DefaultAnimatorConfig = new Config
        {
            MetadataEnabled = true,
            EventsEnabled = true,
            PoolingEnabled = true,
            TagsEnabled = false,
            VarsEnabled = false,
            SoundsEnabled = false
        };

        protected readonly IList<CustomSpriterAnimator> Animators = new List<CustomSpriterAnimator>();
        public CustomSpriterAnimator CurrentAnimator;

        #region Getters

        public virtual Vector2 Position()
        {
            return CurrentAnimator.Position;
        }

        public virtual float Rotation()
        {
            return CurrentAnimator.Rotation;
        }

        public virtual Vector2 Origin()
        {
            return new Vector2(Width() / 2f, Height() / 2f);
        }

        public virtual Vector2 ScaleVector()
        {
            return CurrentAnimator.Scale;
        }

        public CustomSpriterAnimator GetCurrentAnimator()
        {
            return CurrentAnimator;
        }

        public float GetLifePercentage()
        {
            return CurrentBehaviourIndex >= Behaviours.Count ? 0 : Behaviours[CurrentBehaviourIndex].GetLifePercentage();
        }

        public Vector2 ActionPointPosition()
        {
            if (CurrentAnimator.FrameData != null)
            {
                foreach (var pointData in CurrentAnimator.FrameData.PointData)
                {
                    if (pointData.Key.StartsWith("action_point"))
                    {
                        var actionPoint = new Vector2(pointData.Value.X, -pointData.Value.Y) * CurrentAnimator.Scale;
                        var rotatedActionPoint = MathExtension.RotatePoint(actionPoint, Rotation());
                        return Position() + rotatedActionPoint;
                    }
                }


            }

            return Position();
        }

        public float ActionPointDirection()
        {
            if (CurrentAnimator.FrameData != null && CurrentAnimator.FrameData.PointData.ContainsKey("action_point"))
            {
                var actionPoint = CurrentAnimator.FrameData.PointData["action_point"];
                return MathHelper.ToRadians(actionPoint.Angle - 90f) + Rotation();
            }

            return Rotation();
        }

        public virtual int Width()
        {
            if (CurrentAnimator.SpriteProvider != null)
            {
                return (int)CurrentAnimator.SpriteProvider.Get(0, 0).Height();
            }

            return 0;
        }

        public virtual int Height()
        {
            if (CurrentAnimator.SpriteProvider != null)
            {
                return (int)CurrentAnimator.SpriteProvider.Get(0, 0).Width();
            }

            return 0;
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

        public bool IsReady()
        {
            return _ready;
        }

        public void EnableRandomPosition(bool value, bool longDistance = false)
        {
            _randomPosition = value;
            _randomPositionLongDistance = longDistance;
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

        protected Boss(XmasHell game, BossType type, PositionDelegate playerPositionDelegate)
        {
            Game = game;
            BossType = type;
            _playerPositionDelegate = playerPositionDelegate;

            InitialPosition = GameConfig.BossDefaultPosition;

            // Behaviours
            Behaviours = new List<AbstractBossBehaviour>();

            // BulletML
            BulletPatternFiles = new List<string>();

            HitColor = Color.White * 0.5f;

            HitBoxes = new List<CollisionElement>();

            // To compute line/wall intersection
            _bottomWallLine = new Line(
                new Vector2(0f, GameConfig.VirtualResolution.Y),
                new Vector2(GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y)
            );

            _leftWallLine = new Line(
                new Vector2(0f, 0f),
                new Vector2(0f, GameConfig.VirtualResolution.Y)
            );

            _rightWallLine = new Line(
                new Vector2(GameConfig.VirtualResolution.X, 0f),
                new Vector2(GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y)
            );

            _upWallLine = new Line(
                new Vector2(0f, 0f),
                new Vector2(GameConfig.VirtualResolution.X, 0f)
            );
        }

        public virtual void Initialize()
        {
            LoadBulletPatterns();
            LoadSpriterSprite();

            Reset();
        }

        private void Clear()
        {
            Game.SpriteBatchManager.BossBullets.Clear();
            Game.GameManager.CollisionWorld.ClearBossHitboxes();
            Game.GameManager.CollisionWorld.ClearBossBullets();

            PhysicsBody?.Dispose();
            PhysicsWorld?.Clear();

            Game.GameManager.MoverManager.SetBounceBounds(null);
        }

        public virtual void Reset()
        {
            Clear();

            Game.SpriteBatchManager.Boss = this;
            InitializePhysics();

            foreach (var behaviour in Behaviours)
                behaviour.Reset();

            _randomPosition = false;
            _randomPositionLongDistance = false;
            RandomMovingArea = new Rectangle(
                (int)(Width() / 2f), (int)(Height() / 2f),
                GameConfig.VirtualResolution.X - (int)(Width() / 2f),
                500 - (int)(Height() / 2f)
            );

            _bossEntranceAnimation = true;
            _ready = false;
            _destroyed = false;
            _targetDirection = Vector2.Zero;
            _targetAngle = 0f;

            Game.GameManager.MoverManager.Clear();
            Invincible = true;
            Tinted = false;
            TargetingPosition = false;
            TargetingAngle = false;

            CurrentBehaviourIndex = 0;
            PreviousBehaviourIndex = -1;

            Position(new Vector2(InitialPosition.X, InitialPosition.Y - 500f));
            Rotation(0);
            Scale(Vector2.One);

            Direction = Vector2.Zero;
            Speed = GameConfig.BossDefaultSpeed;

            CurrentAnimator.Play("Idle");
            CurrentAnimator.Progress = 0;
            CurrentAnimator.Speed = 1;

            MoveToInitialPosition(GameConfig.BossEntranceAnimationTime, true);
        }

        public void Dispose()
        {
            Clear();
            Game.SpriteBatchManager.Boss = null;
        }

        public bool Alive()
        {
            return !_destroyed;
        }

        protected virtual void LoadSpriterSprite()
        {
            if (SpriterFilename == string.Empty)
                throw new Exception("You need to specify a path to the spriter file of this boss");

            var factory = new DefaultProviderFactory<ISprite, SoundEffect>(DefaultAnimatorConfig, true);

            var loader = new SpriterContentLoader(Game.Content, SpriterFilename);
            loader.Fill(factory);

            foreach (var entity in loader.Spriter.Entities)
            {
                var animator = new CustomSpriterAnimator(entity, factory);
                Animators.Add(animator);
            }

            CurrentAnimator = Animators.First();
        }

        protected virtual void InitializePhysics(bool setupPhysicsWorld = false)
        {
            if (setupPhysicsWorld)
                SetupPhysicsWorld();
        }

        public void AddHitBox(CollisionElement hitBox)
        {
            Game.GameManager.CollisionWorld.AddBossHitBox(hitBox);
            HitBoxes.Add(hitBox);
        }

        public void RemoveHitBox(CollisionElement hitBox)
        {
            Game.GameManager.CollisionWorld.RemoveBossHitBox(hitBox);
            HitBoxes.Remove(hitBox);
        }

        public void ClearHitBoxes()
        {
            Game.GameManager.CollisionWorld.ClearBossHitboxes();
            HitBoxes.Clear();
        }

        protected virtual void SetupPhysicsWorld()
        {
            PhysicsWorld = new World(GameConfig.DefaultGravity);
            _debugView = new DebugView(PhysicsWorld, Game, 1f);

            // Walls
            BottomWallBody = BodyFactory.CreateEdge(
                PhysicsWorld,
                ConvertUnits.ToSimUnits(0, GameConfig.VirtualResolution.Y),
                ConvertUnits.ToSimUnits(GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y)
            );

            LeftWallBody = BodyFactory.CreateEdge(
                PhysicsWorld,
                ConvertUnits.ToSimUnits(0, 0),
                ConvertUnits.ToSimUnits(0, GameConfig.VirtualResolution.Y)
            );

            RightWallBody = BodyFactory.CreateEdge(
                PhysicsWorld,
                ConvertUnits.ToSimUnits(GameConfig.VirtualResolution.X, 0),
                ConvertUnits.ToSimUnits(GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y)
            );

            TopWallBody = BodyFactory.CreateEdge(
                PhysicsWorld,
                ConvertUnits.ToSimUnits(0, 0),
                ConvertUnits.ToSimUnits(GameConfig.VirtualResolution.X, 0)
            );
        }

        private void RestoreDefaultState()
        {
            Direction = Vector2.Zero;
            Rotation(0);
            CurrentAnimator.Play("Idle");
        }

        private void LoadBulletPatterns()
        {
            foreach (var bulletPatternFile in BulletPatternFiles)
            {
                if (Game.GameManager.MoverManager.FindPattern(bulletPatternFile) == null)
                {
                    var pattern = new BulletPattern();
                    var stream = Assets.GetPattern(bulletPatternFile);
                    pattern.ParseStream(bulletPatternFile, stream);
                    Game.GameManager.MoverManager.AddPattern(bulletPatternFile, pattern);
                }
            }
        }

        public void Destroy()
        {
            if (_destroyed)
                return;

            Game.PlayerData.BossBeatenCounter(BossType, Game.PlayerData.BossBeatenCounter(BossType) + 1);

            var bestTime = Game.PlayerData.BossBestTime(BossType);
            var currentTime = Game.GameManager.GetCurrentTime();
            if (bestTime == TimeSpan.Zero || bestTime > currentTime)
                Game.PlayerData.BossBestTime(BossType, currentTime);

            Game.GameManager.EndGame(true, true, 0f);

            _destroyed = true;

            UnlockBossDefeatAchievement();
            SubmitScore();
        }

        protected virtual void PlayExplosionAnimation()
        {
            Game.GameManager.ParticleManager.EmitBossDestroyedParticles(CurrentAnimator.Position);
        }

        private void UnlockBossDefeatAchievement()
        {
#if ANDROID
            var gameHelper = Game.AndroidActivity.GameHelper;
            gameHelper.UnlockAchievement(gameHelper.BossTypeToAchievementCode(BossType));
#endif
        }

        private void SubmitScore()
        {
#if ANDROID
            var currentTime = (long)Game.GameManager.GetCurrentTime().TotalMilliseconds;

            if (currentTime > 0)
            {
                var gameHelper = Game.AndroidActivity.GameHelper;
                gameHelper.SubmitScore(gameHelper.BossTypeToLeaderboardCode(BossType), currentTime);
            }
#endif
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
            //_targetDirection = Vector2.Normalize(_targetPosition - _initialPosition);
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

        public void MoveToCenter(float time, bool force = false)
        {
            MoveTo(new Vector2(GameConfig.VirtualResolution.X / 2f, GameConfig.VirtualResolution.Y / 2f), time, force);
        }

        public void MoveToCenter(bool force = false)
        {
            MoveTo(new Vector2(GameConfig.VirtualResolution.X / 2f, GameConfig.VirtualResolution.Y / 2f), force);
        }

        public void MoveToInitialPosition(float time, bool force = false)
        {
            MoveTo(InitialPosition, time, force);
        }

        public void MoveToInitialPosition(bool force = false)
        {
            MoveTo(InitialPosition, force);
        }

        public void MoveOutside(float time, bool force = true)
        {
            MoveTo(GetNearestOutsidePosition(), time, force);
        }

        public void MoveOutside(bool force = true)
        {
            MoveTo(GetNearestOutsidePosition(), force);
        }

        public ScreenSide GetRandomSide()
        {
            var randomIndex = Game.GameManager.Random.Next(0, 3);

            if (randomIndex == 0)
                return ScreenSide.Left;
            else if (randomIndex == 1)
                return ScreenSide.Top;
            else if (randomIndex == 2)
                return ScreenSide.Right;
            else
                return ScreenSide.Bottom;
        }

        public ScreenSide GetSideFromPosition(Vector2 position)
        {
            if (position.X <= 0)
                return ScreenSide.Left;
            else if (position.X >= Game.ViewportAdapter.VirtualWidth)
                return ScreenSide.Right;
            else if (position.Y <= 0)
                return ScreenSide.Top;
            else if (position.Y >= Game.ViewportAdapter.VirtualHeight)
                return ScreenSide.Bottom;
            else
                return GetSideFromPosition(GetNearestOutsidePosition());
        }

        public float GetRandomVerticalPosition()
        {
            return Game.GameManager.Random.Next(0, Game.ViewportAdapter.VirtualHeight);
        }

        public float GetRandomHorizontalPosition()
        {
            return Game.GameManager.Random.Next(0, Game.ViewportAdapter.VirtualWidth);
        }

        public Vector2 GetRandomOutsidePosition(ScreenSide? side = null)
        {
            var randomPosition = Vector2.Zero;
            side = side.HasValue ? side.Value : GetRandomSide();

            switch(side)
            {
                case ScreenSide.Left:
                    randomPosition.Y = Game.GameManager.Random.Next(0, Game.ViewportAdapter.VirtualHeight);
                    break;
                case ScreenSide.Right:
                    randomPosition.X = Game.ViewportAdapter.VirtualWidth;
                    randomPosition.Y = Game.GameManager.Random.Next(0, Game.ViewportAdapter.VirtualHeight);
                    break;
                case ScreenSide.Top:
                    randomPosition.X = Game.GameManager.Random.Next(0, Game.ViewportAdapter.VirtualWidth);
                    randomPosition.Y = Game.ViewportAdapter.VirtualHeight;
                    break;
                case ScreenSide.Bottom:
                    randomPosition.X = Game.GameManager.Random.Next(0, Game.ViewportAdapter.VirtualWidth);
                    break;
            }

            return randomPosition;
        }

        public Vector2 GetNearestOutsidePosition()
        {
            // Get the nearest border
            var newPosition = Position();
            ScreenSide side = GetNearestBorder();

            switch (side)
            {
                case ScreenSide.Left:
                    newPosition.X = -Width();
                    break;
                case ScreenSide.Top:
                    newPosition.Y = -Height();
                    break;
                case ScreenSide.Right:
                    newPosition.X = Game.ViewportAdapter.VirtualWidth + Width();
                    break;
                case ScreenSide.Bottom:
                    newPosition.Y = Game.ViewportAdapter.VirtualHeight + Height();
                    break;
                default:
                    break;
            }

            return newPosition;
        }

        public float GetRandomOutsideAngle(ScreenSide side, int maxAngle, int? topAngle = null)
        {
            var randomAngle = Game.GameManager.Random.Next(-maxAngle, maxAngle);

            if (topAngle.HasValue)
                randomAngle -= topAngle.Value;

            switch (side)
            {
                case ScreenSide.Left:
                    randomAngle -= 90;
                    break;
                case ScreenSide.Right:
                    randomAngle += 90;
                    break;
                case ScreenSide.Bottom:
                    randomAngle += 180;
                    break;
                default:
                    break;
            }

            return randomAngle;
        }

        public ScreenSide GetNearestBorder()
        {
            if (Position().X < Game.ViewportAdapter.VirtualWidth / 2f)
            {
                if (Position().Y < Game.ViewportAdapter.VirtualHeight / 2f)
                {
                    if (Position().Y < Position().X)
                    {
                        return ScreenSide.Top;
                    }
                }
                else
                {
                    if (Game.ViewportAdapter.VirtualHeight - Position().Y < Position().X)
                    {
                        return ScreenSide.Bottom;
                    }
                }

                return ScreenSide.Left;
            }
            else
            {
                if (Position().Y < Game.ViewportAdapter.VirtualHeight / 2f)
                {
                    if (Position().Y < Game.ViewportAdapter.VirtualWidth - Position().X)
                    {
                        return ScreenSide.Top;
                    }
                }
                else
                {
                    if (Game.ViewportAdapter.VirtualHeight - Position().Y <
                        Game.ViewportAdapter.VirtualWidth - Position().X)
                    {
                        return ScreenSide.Bottom;
                    }
                }

                return ScreenSide.Right;
            }
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
            _targetPositionTime = TimeSpan.Zero;
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

            return MathExtension.AngleToDirection(angle);
        }

        public float GetPlayerDirectionAngle()
        {
            var playerPosition = GetPlayerPosition();
            var currentPosition = CurrentAnimator.Position;
            var angle = (currentPosition - playerPosition).ToAngle();

            return angle;
        }

        public bool GetLineWallIntersectionPosition(Line line, ref Vector2 newPosition)
        {
            // Make sure the line go out of the screen
            var maxDistance = (float) Math.Sqrt(
                GameConfig.VirtualResolution.X * GameConfig.VirtualResolution.X +
                GameConfig.VirtualResolution.Y * GameConfig.VirtualResolution.Y
            );
            var direction = Vector2.Normalize(line.Second - line.First);
            line.Second += (direction * maxDistance);

            return
                MathExtension.LinesIntersect(_bottomWallLine, line, ref newPosition) ||
                MathExtension.LinesIntersect(_leftWallLine, line, ref newPosition) ||
                MathExtension.LinesIntersect(_rightWallLine, line, ref newPosition) ||
                MathExtension.LinesIntersect(_upWallLine, line, ref newPosition);
        }

        public void TakeDamage(float amount)
        {
            if (Invincible || _destroyed)
                return;

            var currentBehaviour = Behaviours[CurrentBehaviourIndex];
            currentBehaviour.TakeDamage(amount);

            _hitTimer = TimeSpan.FromMilliseconds(20);

            var sounds = new List<SoundEffect>()
                {
                     Assets.GetSound("Audio/SE/boss-hit1"),
                     Assets.GetSound("Audio/SE/boss-hit2"),
                     Assets.GetSound("Audio/SE/boss-hit3")
                };

            SoundManager.PlayRandomSound(sounds);
        }

        public void TriggerPattern(string patternName, BulletType type, bool clear = false, Vector2? position = null, float? direction = null)
        {
            Game.GameManager.MoverManager.TriggerPattern(patternName, type, clear, position, direction);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (_destroyed)
            {
                if (Game.GameManager.TransitioningToEndGame() && Game.SpriteBatchManager.Boss != null)
                {
                    PlayExplosionAnimation();
                    Dispose();
                }

                return;
            }

            if (_bossEntranceAnimation)
            {
                if (Position().EqualsWithTolerence(InitialPosition, 1E-02f))
                {
                    Position(InitialPosition);
                    _ready = true;
                    _bossEntranceAnimation = false;
                    Invincible = false;
                }
            }

            if (!_destroyed && PhysicsEnabled)
            {
                PhysicsWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
                SynchronizeGraphicsWithPhysics();
            }

            if (_randomPosition && !TargetingPosition)
            {
                var newPosition = Vector2.Zero;
                if (_randomPositionLongDistance)
                {
                    var currentPosition = Position().ToPoint();
                    var minDistance = (RandomMovingArea.Width - RandomMovingArea.X) / 2;

                    // Choose a random long distance new X position
                    var leftSpace = currentPosition.X - RandomMovingArea.X;
                    var rightSpace = RandomMovingArea.Width - currentPosition.X;

                    if (leftSpace > minDistance)
                    {
                        if (rightSpace > minDistance)
                        {
                            if (Game.GameManager.Random.NextDouble() > 0.5)
                                newPosition.X = Game.GameManager.Random.Next(currentPosition.X + minDistance, RandomMovingArea.Width);
                            else
                                newPosition.X = Game.GameManager.Random.Next(RandomMovingArea.X, currentPosition.X - minDistance);
                        }
                        else
                            newPosition.X = Game.GameManager.Random.Next(RandomMovingArea.X, currentPosition.X - minDistance);
                    }
                    else
                        newPosition.X = Game.GameManager.Random.Next(currentPosition.X + minDistance, RandomMovingArea.Width);

                    // minDistance only depends on the random area X and width
                    if (RandomMovingArea.Height - RandomMovingArea.Y > minDistance)
                    {
                        // Choose a random long distance new Y position
                        var topSpace = currentPosition.Y - RandomMovingArea.Y;
                        var bottomSpace = RandomMovingArea.Height - currentPosition.Y;

                        if (topSpace > minDistance)
                        {
                            if (bottomSpace > minDistance)
                            {
                                if (Game.GameManager.Random.NextDouble() > 0.5)
                                    newPosition.Y = Game.GameManager.Random.Next(currentPosition.Y + minDistance, RandomMovingArea.Height);
                                else
                                    newPosition.Y = Game.GameManager.Random.Next(RandomMovingArea.Y, currentPosition.Y - minDistance);
                            }
                            else
                                newPosition.Y = Game.GameManager.Random.Next(RandomMovingArea.Y, currentPosition.Y - minDistance);
                        }
                        else
                            newPosition.Y = Game.GameManager.Random.Next(currentPosition.Y + minDistance, RandomMovingArea.Height);
                    }
                    else
                        newPosition.Y = Game.GameManager.Random.Next(RandomMovingArea.Y, RandomMovingArea.Height);
                }
                else
                {
                    newPosition.X = Game.GameManager.Random.Next(RandomMovingArea.X, RandomMovingArea.Width);
                    newPosition.Y = Game.GameManager.Random.Next(RandomMovingArea.Y, RandomMovingArea.Height);
                }

                MoveTo(newPosition, 1.5f);
            }

            UpdatePosition(gameTime);
            UpdateRotation(gameTime);
            UpdateBehaviour(gameTime);

            if (_destroyed)
                return;

            // Is outside of the screen?
            IsOutside = Game.GameManager.IsOutside(Position());

            // New position timer
            if (StartNewPositionTimer && NewPositionTimerFinished != null)
            {
                if (NewPositionTimer.TotalMilliseconds > 0)
                    NewPositionTimer -= gameTime.ElapsedGameTime;
                else
                {
                    NewPositionTimer = TimeSpan.FromSeconds(NewPositionTimerTime);
                    NewPositionTimerFinished.Invoke(this, NewPositionTimerTime);
                }
            }

            // Shoot timer
            if (StartShootTimer && ShootTimerFinished != null)
            {
                if (ShootTimer.TotalMilliseconds > 0)
                    ShootTimer -= gameTime.ElapsedGameTime;
                else
                {
                    ShootTimer = TimeSpan.FromSeconds(ShootTimerTime);
                    ShootTimerFinished.Invoke(this, ShootTimerTime);
                }
            }

            if (_hitTimer.TotalMilliseconds > 0)
                _hitTimer -= gameTime.ElapsedGameTime;

            Tinted = _hitTimer.TotalMilliseconds > 0;

            CurrentAnimator.Update(gameTime.ElapsedGameTime.Milliseconds);
        }

        private void SynchronizeGraphicsWithPhysics()
        {
            if (PhysicsBody == null)
                return;

            Position(ConvertUnits.ToDisplayUnits(PhysicsBody.Position));
            Rotation(PhysicsBody.Rotation);
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
                        _targetPositionTime = TimeSpan.Zero;
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
                    var deltaDistance = AngularVelocity * gameTime.GetElapsedSeconds();

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
            if (!_ready)
                return;

            UpdateBehaviourIndex();

            if (_destroyed)
                return;

            if (CurrentBehaviourIndex != PreviousBehaviourIndex)
            {
                if (PreviousBehaviourIndex >= 0)
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

            if (Behaviours[CurrentBehaviourIndex].IsBehaviourEnded())
            {
                CurrentBehaviourIndex++;

                if (CurrentBehaviourIndex >= Behaviours.Count)
                    Destroy();
            }
        }

        public virtual void Draw()
        {
            if (CurrentBehaviourIndex < Behaviours.Count)
                Behaviours[CurrentBehaviourIndex].Draw(Game.SpriteBatch);

            CurrentAnimator.Draw(Game.SpriteBatch);

            if (CurrentBehaviourIndex < Behaviours.Count)
                Behaviours[CurrentBehaviourIndex].DrawAfter(Game.SpriteBatch);

            if (PhysicsEnabled && GameConfig.DebugPhysics)
            {
                var view = Game.Camera.GetViewMatrix();
                var projection = Matrix.CreateOrthographicOffCenter(
                    0f,
                    ConvertUnits.ToSimUnits(Game.GraphicsDevice.Viewport.Width),
                    ConvertUnits.ToSimUnits(Game.GraphicsDevice.Viewport.Height), 0f, 0f, 1f
                );

                _debugView.Draw(ref projection, ref view);
            }
        }
    }
}