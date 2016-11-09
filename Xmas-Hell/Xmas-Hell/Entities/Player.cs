using System;
using Android.InputMethodServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Extended.Sprites;
using XmasHell.Physics;
using XmasHell.Physics.Collision;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;

namespace XmasHell.Entities
{
    class Player : IPhysicsEntity
    {
        public bool Invincible;
        public TimeSpan _invincibleTimer;

        private readonly XmasHell _game;
        private Sprite _sprite;
        private CollisionCircle _hitbox;
        private Sprite _hitboxSprite;

        private Vector2 _initialSpritePosition;
        private Point _initialTouchPosition;
        private Point _currentTouchPosition;

        private TimeSpan _bulletFrequence;

        public Vector2 Position()
        {
            return _sprite.Position;
        }

        public virtual Vector2 LocalPosition()
        {
            return Vector2.Zero;
        }

        public float Rotation()
        {
            return _sprite.Rotation;
        }

        public Vector2 Pivot()
        {
            return _sprite.Origin;
        }

        public Vector2 Scale()
        {
            return _sprite.Scale;
        }

        public Player(XmasHell game)
        {
            _game = game;
            _bulletFrequence = TimeSpan.Zero;

            var playerTexture = Assets.GetTexture2D("Graphics/Sprites/player");
            var playerHitboxTexture = Assets.GetTexture2D("Graphics/Sprites/hitbox");

            _sprite = new Sprite(playerTexture)
            {
                Origin = new Vector2(playerTexture.Width / 2f, playerTexture.Height / 2f),
                Scale = Vector2.One
            };

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

            _game.SpriteBatchManager.Player = _sprite;
            _game.SpriteBatchManager.PlayerHitbox = _hitboxSprite;
        }

        public void Initialize()
        {
            Invincible = true;
            _invincibleTimer = TimeSpan.FromSeconds(3f);

            _sprite.Position = new Vector2(
                GameConfig.VirtualResolution.X / 2f,
                GameConfig.VirtualResolution.Y - 150
            );

            _initialSpritePosition = _sprite.Position;
            _initialTouchPosition = _currentTouchPosition;
        }

        public void Destroy()
        {
            _game.GameManager.ParticleManager.EmitPlayerDestroyedParticles(Position());
            Initialize();
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
                    _initialSpritePosition = _sprite.Position;
                    _initialTouchPosition = _game.ViewportAdapter.PointToScreen(currentTouchState[0].Position.ToPoint());
                }

                _currentTouchPosition = _game.ViewportAdapter.PointToScreen(currentTouchState[0].Position.ToPoint());
                var touchDelta = (_currentTouchPosition - _initialTouchPosition).ToVector2();

                _sprite.Position = _initialSpritePosition + (touchDelta * GameConfig.PlayerMoveSensitivity);
            }
            else
            {
                _initialSpritePosition = Vector2.Zero;
                _initialTouchPosition = Point.Zero;
            }
        }

        private void CheckOutOfBounds()
        {
            _sprite.Position = new Vector2(
                MathHelper.Clamp(_sprite.Position.X, 0, GameConfig.VirtualResolution.X),
                MathHelper.Clamp(_sprite.Position.Y, 0, GameConfig.VirtualResolution.Y)
            );
        }

        private void UpdateShoot(GameTime gameTime)
        {
            if (_bulletFrequence.TotalMilliseconds > 0)
                _bulletFrequence -= gameTime.ElapsedGameTime;
            else
            {
                _bulletFrequence = TimeSpan.FromTicks(GameConfig.PlayerShootFrequency.Ticks);

                var bullet1 = new PlayerBullet(_game, _sprite.Position, -MathHelper.PiOver4 / 4f, GameConfig.PlayerBulletSpeed);
                var bullet2 = new PlayerBullet(_game, _sprite.Position, -MathHelper.PiOver4 / 8f, GameConfig.PlayerBulletSpeed);
                var bullet3 = new PlayerBullet(_game, _sprite.Position, 0f, GameConfig.PlayerBulletSpeed);
                var bullet4 = new PlayerBullet(_game, _sprite.Position, MathHelper.PiOver4 / 8f, GameConfig.PlayerBulletSpeed);
                var bullet5 = new PlayerBullet(_game, _sprite.Position, MathHelper.PiOver4 / 4f, GameConfig.PlayerBulletSpeed);

                _game.GameManager.AddBullet(bullet1);
                _game.GameManager.AddBullet(bullet2);
                _game.GameManager.AddBullet(bullet3);
                _game.GameManager.AddBullet(bullet4);
                _game.GameManager.AddBullet(bullet5);
            }
        }
    }
}