using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using SpriterDotNet.MonoGame;
using SpriterDotNet.Providers;
using SpriterDotNet.MonoGame.Content;
using XmasHell.Spriter;
using SpriterDotNet;
using Microsoft.Xna.Framework.Audio;
using System.Linq;

namespace XmasHell.Screens
{
    public abstract class Screen
    {
        protected XmasHell Game;
        protected bool IsVisible;
        private bool _neverShown;
        protected bool ShouldBeStackInHistory;
        protected bool SpriterGuiInitialized = false;

        public bool StackInHistory => ShouldBeStackInHistory;

        // Spriter
        private bool _spriterFrameDataAvailable;
        protected static readonly Config DefaultAnimatorConfig = new Config
        {
            MetadataEnabled = true,
            EventsEnabled = true,
            PoolingEnabled = true,
            TagsEnabled = false,
            VarsEnabled = false,
            SoundsEnabled = false
        };

        protected readonly Dictionary<string, CustomSpriterAnimator> Animators = new Dictionary<string, CustomSpriterAnimator>();

        public Screen(XmasHell game)
        {
            Game = game;
            _neverShown = true;
            ShouldBeStackInHistory = true;
        }

        public virtual void Initialize()
        {
            // Reset the initial state of the screen
        }

        public virtual void LoadContent()
        {
            // Load content used only by this screen
        }

        protected virtual void LoadSpriterSprite(String spriterFilename)
        {
            if (spriterFilename == string.Empty)
                throw new Exception("You need to specify a path to the spriter file of this boss");

            var factory = new DefaultProviderFactory<ISprite, SoundEffect>(DefaultAnimatorConfig, true);

            var loader = new SpriterContentLoader(Game.Content, spriterFilename);
            loader.Fill(factory);

            foreach (var entity in loader.Spriter.Entities)
            {
                var animator = new CustomSpriterAnimator(Game, entity, factory);
                // Center the animator
                animator.Position = Game.ViewportAdapter.Center.ToVector2();

                Animators.Add(entity.Name, animator);
            }
        }

        protected virtual void InitializeSpriterGui()
        {
            SpriterGuiInitialized = true;
        }

        public virtual void Show(bool reset = false)
        {
            IsVisible = true;

            if (_neverShown)
            {
                LoadContent();
                Initialize();

                _neverShown = false;
            }

            if (reset)
                Initialize();

            // Add elements to the SpriteBatchManager
        }

        public virtual void Hide()
        {
            IsVisible = false;

            // Remove elements from the SpriteBatchManager
        }

        public virtual void Update(GameTime gameTime)
        {
            // Check Spriter element are initialized
            if (!_spriterFrameDataAvailable && Animators.Count > 0)
            {
                InitializeSpriterGui();
                _spriterFrameDataAvailable = true;
            }
        }
    }
}
