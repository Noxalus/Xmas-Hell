using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using SpriterDotNet.MonoGame;
using SpriterDotNet.Providers;
using XmasHell.Physics;
using XmasHell.Physics.Collision;
using XmasHell.Spriter;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;
using Sprite = MonoGame.Extended.Sprites.Sprite;
using SpriterDotNet;

namespace XmasHell.Entities
{
    public class Player : IPhysicsEntity
    {
        public bool Invincible;
        public TimeSpan _invincibleTimer;

        private readonly XmasHell _game;
        private CollisionCircle _hitbox;
        private Sprite _hitboxSprite;

        private IList<MonoGameAnimator> _animators = new List<MonoGameAnimator>();
        public MonoGameAnimator CurrentAnimator;

        private Vector2 _initialSpritePosition;
        private Point _initialTouchPosition;
        private Point _currentTouchPosition;
        private Point _previousTouchPosition;

        private TimeSpan _bulletFrequence;

        public Vector2 Position()
        {
            return CurrentAnimator.Position;
        }

        public virtual Vector2 LocalPosition()
        {
            return Vector2.Zero;
        }

        public float Rotation()
        {
            return CurrentAnimator.Rotation;
        }

        public Vector2 Pivot()
        {
            return Vector2.Zero;
        }

        public Vector2 Scale()
        {
            return CurrentAnimator.Scale;
        }

        public Player(XmasHell game)
        {
            _game = game;
            _bulletFrequence = TimeSpan.Zero;

            var playerHitboxTexture = Assets.GetTexture2D("Graphics/Sprites/hitbox");

            var animatorConfig = new Config
            {
                MetadataEnabled = false,
                EventsEnabled = false,
                PoolingEnabled = true,
                TagsEnabled = false,
                VarsEnabled = false,
                SoundsEnabled = false
            };

            var factory = new DefaultProviderFactory<SpriterDotNet.MonoGame.Sprite, SoundEffect>(animatorConfig, true);

            var loader = new SpriterContentLoader(_game.Content, "Graphics/Sprites/Player/player");
            loader.Fill(factory);

            foreach (var entity in loader.Spriter.Entities)
            {
                var animator = new MonoGameDebugAnimator(entity, _game.GraphicsDevice, factory);
                _animators.Add(animator);
            }

            CurrentAnimator = _animators.First();
            var spriteSize = new Vector2(60, 82);
            CurrentAnimator.Position = new Vector2(spriteSize.X / 2f, spriteSize.Y / 2f);
            CurrentAnimator.Play("Idle");

            CurrentAnimator.AnimationFinished += AnimationFinished;

            _hitboxSprite = new Sprite(playerHitboxTexture)
            {
                Scale = new Vector2(
                    GameConfig.PlayerHitboxRadius / playerHitboxTexture.Width,
                    GameConfig.PlayerHitboxRadius / playerHitboxTexture.Height
                )
            };
            _hitbox = new CollisionCircle(this, new Vector2(0f, 0f), GameConfig.PlayerHitboxRadius);
            _game.GameManager.CollisionWorld.PlayerHitbox = _hitbox;

            // Don't forget to set the player position delegate to the MoverManager
            _game.GameManager.MoverManager.SetPlayerPositionDelegate(Position);

            Initialize();

            _game.SpriteBatchManager.Player = this;
            _game.SpriteBatchManager.PlayerHitbox = _hitboxSprite;
        }

        public void Initialize()
        {
            Invincible = true;
            _invincibleTimer = TimeSpan.FromSeconds(3f);

            _initialSpritePosition = new Vector2(
                GameConfig.VirtualResolution.X / 2f,
                GameConfig.VirtualResolution.Y - 150
            );

            CurrentAnimator.Position = _initialSpritePosition;
            _initialTouchPosition = _currentTouchPosition;
        }

        public void Destroy()
        {
            _game.GameManager.ParticleManager.EmitPlayerDestroyedParticles(Position());
            Initialize();
        }

        private void AnimationFinished(string animationName)
        {
            if (animationName == "Left")
                CurrentAnimator.Play("LeftIdle");
            else if (animationName == "Right")
                CurrentAnimator.Play("RightIdle");
        }

        public void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Enter))
                _game.GameManager.ParticleManager.EmitPlayerDestroyedParticles(Position());

            if (_invincibleTimer.TotalMilliseconds > 0)
                _invincibleTimer -= gameTime.ElapsedGameTime;
            else
                Invincible = false;

            CurrentAnimator.Update(gameTime.ElapsedGameTime.Milliseconds);

            UpdatePosition(gameTime);
            _hitboxSprite.Position = _hitbox.GetCenter();

            CheckOutOfBounds();
            UpdateShoot(gameTime);
        }

        private void UpdatePosition(GameTime gameTime)
        {
            var currentTouchState = TouchPanel.GetState();

            if (currentTouchState.Count > 0)
            {
                if (currentTouchState[0].State == TouchLocationState.Pressed)
                {
                    _initialSpritePosition = Position();
                    _initialTouchPosition = _game.ViewportAdapter.PointToScreen(currentTouchState[0].Position.ToPoint());
                }

                _previousTouchPosition = _currentTouchPosition;
                _currentTouchPosition = _game.ViewportAdapter.PointToScreen(currentTouchState[0].Position.ToPoint());
                var touchDelta = _currentTouchPosition - _previousTouchPosition;
                var globalTouchDelta = (_currentTouchPosition - _initialTouchPosition).ToVector2();

                Console.WriteLine("Touch delta: " + touchDelta);

                if (touchDelta.X < -10)
                {
                    if (CurrentAnimator.CurrentAnimation.Name != "Left" &&
                        CurrentAnimator.CurrentAnimation.Name != "LeftIdle")
                        CurrentAnimator.Play("Left");
                }
                else if (touchDelta.X > 10)
                {
                    if (CurrentAnimator.CurrentAnimation.Name != "Right" &&
                        CurrentAnimator.CurrentAnimation.Name != "RightIdle")
                        CurrentAnimator.Play("Right");
                }
                else
                {
                    //CurrentAnimator.Play("Idle");
                }

                CurrentAnimator.Position = _initialSpritePosition + (globalTouchDelta * GameConfig.PlayerMoveSensitivity);
            }
            else
            {
                _initialSpritePosition = Vector2.Zero;
                _initialTouchPosition = Point.Zero;
                CurrentAnimator.Play("Idle");
            }
        }

        private void CheckOutOfBounds()
        {
            CurrentAnimator.Position = new Vector2(
                MathHelper.Clamp(CurrentAnimator.Position.X, 0, GameConfig.VirtualResolution.X),
                MathHelper.Clamp(CurrentAnimator.Position.Y, 0, GameConfig.VirtualResolution.Y)
            );
        }

        private void UpdateShoot(GameTime gameTime)
        {
            if (_bulletFrequence.TotalMilliseconds > 0)
                _bulletFrequence -= gameTime.ElapsedGameTime;
            else
            {
                _bulletFrequence = TimeSpan.FromTicks(GameConfig.PlayerShootFrequency.Ticks);

                var bullet1 = new PlayerBullet(_game, CurrentAnimator.Position, -MathHelper.PiOver4 / 4f, GameConfig.PlayerBulletSpeed);
                var bullet2 = new PlayerBullet(_game, CurrentAnimator.Position, -MathHelper.PiOver4 / 8f, GameConfig.PlayerBulletSpeed);
                var bullet3 = new PlayerBullet(_game, CurrentAnimator.Position, 0f, GameConfig.PlayerBulletSpeed);
                var bullet4 = new PlayerBullet(_game, CurrentAnimator.Position, MathHelper.PiOver4 / 8f, GameConfig.PlayerBulletSpeed);
                var bullet5 = new PlayerBullet(_game, CurrentAnimator.Position, MathHelper.PiOver4 / 4f, GameConfig.PlayerBulletSpeed);

                _game.GameManager.AddBullet(bullet1);
                _game.GameManager.AddBullet(bullet2);
                _game.GameManager.AddBullet(bullet3);
                _game.GameManager.AddBullet(bullet4);
                _game.GameManager.AddBullet(bullet5);
            }
        }
    }
}