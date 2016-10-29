using System.Collections.Generic;
using System.Linq;
using Java.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using SpriterDotNet;
using SpriterDotNet.MonoGame;
using SpriterDotNet.Providers;
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
        }

        private void CurrentAnimator_EventTriggered(string obj)
        {
            System.Diagnostics.Debug.WriteLine(obj);
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