using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using SpriterDotNet;
using SpriterDotNet.MonoGame;
using SpriterDotNet.Providers;
using Sprite = MonoGame.Extended.Sprites.Sprite;
using SpriterSprite = SpriterDotNet.MonoGame.Sprite;
using Xmas_Hell.Spriter;

namespace Xmas_Hell.Entities
{
    class Boss
    {
        private XmasHell _game;
        private Sprite _sprite;
        private float _initialLife;
        private float _life;

        // Spriter

        private static readonly Config config = new Config
        {
            MetadataEnabled = true,
            EventsEnabled = true,
            PoolingEnabled = true,
            TagsEnabled = true,
            VarsEnabled = true,
            SoundsEnabled = false
        };

        private IList<MonoGameAnimator> animators = new List<MonoGameAnimator>();
        private MonoGameAnimator currentAnimator;

        public Boss(XmasHell game, Vector2 position, float initialLife)
        {
            _game = game;

            _initialLife = initialLife;
            _life = initialLife;

            //_sprite = new Sprite(Assets.GetTexture2D("Graphics/Sprites/boss"))
            //{
            //    Position = position
            //};

            // Spriter
            DefaultProviderFactory<SpriterSprite, SoundEffect> factory = new DefaultProviderFactory<SpriterSprite, SoundEffect>(config, true);

            SpriterContentLoader loader = new SpriterContentLoader(_game.Content, "Graphics/Sprites/Bosses/XmasBall/spriter");
            loader.Fill(factory);

            foreach (SpriterEntity entity in loader.Spriter.Entities)
            {
                var animator = new MonoGameDebugAnimator(entity, _game.GraphicsDevice, factory);
                animators.Add(animator);
                animator.Position = position;
            }

            currentAnimator = animators.First();
            currentAnimator.EventTriggered += CurrentAnimator_EventTriggered;
        }

        private void CurrentAnimator_EventTriggered(string obj)
        {
            System.Diagnostics.Debug.WriteLine(obj);
        }

        public void Update(GameTime gameTime)
        {
            _life -= 0.01f;

            currentAnimator.Update(gameTime.ElapsedGameTime.Milliseconds);
        }

        public void Draw(GameTime gameTime)
        {
            currentAnimator.Draw(_game.SpriteBatch);

            var percent = _life / _initialLife;
            _game.SpriteBatch.Draw(
                Assets.GetTexture2D("Graphics/Pictures/pixel"),
                new Rectangle(0, 0, (int)(percent * GameConfig.VirtualResolution.X), 20),
                Color.Black
            );
        }
    }
}