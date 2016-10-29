using System;
using System.Collections.Generic;
using System.Linq;
using BulletML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using SpriterDotNet;
using SpriterDotNet.MonoGame;
using SpriterDotNet.Providers;
using Xmas_Hell.BulletML;
using SpriterSprite = SpriterDotNet.MonoGame.Sprite;
using Xmas_Hell.Spriter;

namespace Xmas_Hell.Entities
{
    class Boss
    {
        private XmasHell _game;
        private float _initialLife;
        private float _life;
        private float _direction = 1f;

        private List<BulletPattern> _bossPatterns;
        private TimeSpan _bossBulletFrequence;

        public Vector2 Position()
        {
            return _currentAnimator.Position;
        }

        public Vector2 ActionPointPosition()
        {
            if (_currentAnimator.FrameData != null && _currentAnimator.FrameData.PointData.ContainsKey("action_point"))
            {
                var actionPoint = _currentAnimator.FrameData.PointData["action_point"];
                return Position() + new Vector2(actionPoint.X, -actionPoint.Y);
            }

            return Position();
        }

        // Spriter

        private static readonly Config _animatorConfig = new Config
        {
            MetadataEnabled = true,
            EventsEnabled = true,
            PoolingEnabled = true,
            TagsEnabled = true,
            VarsEnabled = true,
            SoundsEnabled = false
        };

        private IList<MonoGameAnimator> _animators = new List<MonoGameAnimator>();
        private MonoGameAnimator _currentAnimator;

        public Boss(XmasHell game, Vector2 position, float initialLife)
        {
            _game = game;

            _initialLife = initialLife;
            _life = initialLife;

            // Spriter
            DefaultProviderFactory<SpriterSprite, SoundEffect> factory = new DefaultProviderFactory<SpriterSprite, SoundEffect>(_animatorConfig, true);

            SpriterContentLoader loader = new SpriterContentLoader(_game.Content, "Graphics/Sprites/Bosses/XmasBall/xmas-ball");
            loader.Fill(factory);

            foreach (SpriterEntity entity in loader.Spriter.Entities)
            {
                var animator = new MonoGameDebugAnimator(entity, _game.GraphicsDevice, factory);
                _animators.Add(animator);
                animator.Position = position;
            }

            _currentAnimator = _animators.First();
            _currentAnimator.EventTriggered += CurrentAnimator_EventTriggered;

            // BulletML
            _bossPatterns = new List<BulletPattern>();
            _bossBulletFrequence = TimeSpan.Zero;

            // Load the pattern
            var pattern = new BulletPattern();
            _bossPatterns.Add(pattern);

            var filename = "sample";
            pattern.ParseStream(filename, Assets.GetPattern(filename));
        }

        private void CurrentAnimator_EventTriggered(string obj)
        {
            System.Diagnostics.Debug.WriteLine(obj);
        }

        private void AddBullet(bool clear = false)
        {
            if (clear)
                _game.GameManager.MoverManager.Clear();

            // Add a new bullet in the center of the screen
            var mover = (Mover)_game.GameManager.MoverManager.CreateBullet(true);
            mover.Position = ActionPointPosition();
            mover.InitTopNode(_bossPatterns[0].RootNode);
        }

        public void Update(GameTime gameTime)
        {
            var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
            _life -= 0.01f;

            if (_currentAnimator.Position.X > GameConfig.VirtualResolution.X)
                _direction = -1f;
            else if (_currentAnimator.Position.X < 0)
                _direction = 1f;

            _currentAnimator.Position += new Vector2(500f * dt, 0f) * _direction;

            _currentAnimator.Update(gameTime.ElapsedGameTime.Milliseconds);

            if (_bossBulletFrequence.TotalMilliseconds > 0)
                _bossBulletFrequence -= gameTime.ElapsedGameTime;
            else
            {
                _bossBulletFrequence = TimeSpan.FromTicks(GameConfig.PlayerShootFrequency.Ticks);
                AddBullet();
            }
        }

        public void Draw(GameTime gameTime)
        {
            _currentAnimator.Draw(_game.SpriteBatch);

            var percent = _life / _initialLife;
            _game.SpriteBatch.Draw(
                Assets.GetTexture2D("Graphics/Pictures/pixel"),
                new Rectangle(0, 0, (int)(percent * GameConfig.VirtualResolution.X), 20),
                Color.Black
            );
        }
    }
}