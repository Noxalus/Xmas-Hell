using System;
using BulletML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using XmasHell.BulletML;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tweening;
using XmasHell.GUI;
using XmasHell.Spriter;
using SpriterDotNet;
using Microsoft.Xna.Framework.Audio;
using SpriterDotNet.MonoGame;
using SpriterDotNet.Providers;
using SpriterDotNet.MonoGame.Content;
using System.Collections.Generic;

namespace XmasHell.Screens
{
    public class MainMenuScreen : Screen
    {
        private string _patternFile = "MainMenu/snowflake";
        private TimeSpan _shootFrequency;
        private Song _introSong;
        private Song _mainSong;

        private SpriterGuiButton _playButton;

        private bool spriterFrameDataAvailable = false;

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

        private readonly Dictionary<string, CustomSpriterAnimator> _animators = new Dictionary<string, CustomSpriterAnimator>();
        //public CustomSpriterAnimator CurrentAnimator;

        public MainMenuScreen(XmasHell game) : base(game)
        {
            SpriterFilename = "Graphics/GUI/MainMenu/main-menu";
        }

        public override void Initialize()
        {
            _shootFrequency = TimeSpan.Zero;

            base.Initialize();
        }

        private void OnPlayButtonAction()
        {
            Game.ScreenManager.GoTo<BossSelectionScreen>();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            if (Game.GameManager.MoverManager.FindPattern(_patternFile) == null)
            {
                var pattern = new BulletPattern();
                pattern.ParseStream(_patternFile, Assets.GetPattern(_patternFile));

                Game.GameManager.MoverManager.AddPattern(_patternFile, pattern);
            }

            _introSong = Assets.GetMusic("boss-theme-intro");
            _mainSong = Assets.GetMusic("boss-theme-main");

            LoadSpriterSprite();
        }

        private void InitializeGuiButtons()
        {
            // Title
            var xmasTitleDummyPosition = SpriterUtils.GetSpriterFilePosition("xmas-title.png", _animators["MainMenu"]);
            _animators["XmasTitle"].Position = Game.ViewportAdapter.Center.ToVector2() + xmasTitleDummyPosition;

            // Place buttons according to their dummy positions on the Spriter file
            var spriterPlayButtonDummyPosition = SpriterUtils.GetSpriterFilePosition("play-button.png", _animators["MainMenu"]);
            //_playButton.Position = Game.ViewportAdapter.Center.ToVector2() + spriterPlayButtonPosition;
            _animators["PlayButton"].Position = Game.ViewportAdapter.Center.ToVector2() + spriterPlayButtonDummyPosition;

            _playButton = new SpriterGuiButton(Game.ViewportAdapter, "play-button.png", _animators["PlayButton"]);

#if ANDROID
            _playButton.Tap += (s, e) => OnPlayButtonAction();
#else
            _playButton.Click += (s, e) => OnPlayButtonAction();
#endif

            Game.GuiManager.AddButton(_playButton);
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
                var animator = new CustomSpriterAnimator(entity, Game.GraphicsDevice, factory);
                _animators.Add(entity.Name, animator);
            }

            Game.SpriteBatchManager.BackgroundSpriterAnimators.Add(_animators["MainMenu"]);
            Game.SpriteBatchManager.BackgroundSpriterAnimators.Add(_animators["XmasTitle"]);

            _animators["MainMenu"].Position = Game.ViewportAdapter.Center.ToVector2();
        }

        public override void Show(bool reset = false)
        {
            base.Show(reset);

            // Should play music (doesn't seem to work for now...)
            MediaPlayer.Volume = 1f;

            MediaPlayer.MediaStateChanged += MediaPlayerOnMediaStateChanged;
            MediaPlayer.ActiveSongChanged += MediaPlayerOnActiveSongChanged;

            MediaPlayer.Stop();

            //MediaPlayer.Play(_introSong);
            //MediaPlayer.Play(_mainSong);
        }

        public override void Hide()
        {
            base.Hide();

            MediaPlayer.Stop();

            MediaPlayer.MediaStateChanged -= MediaPlayerOnMediaStateChanged;
            MediaPlayer.ActiveSongChanged -= MediaPlayerOnActiveSongChanged;

            // GUI
            Game.GuiManager.RemoveButton(_playButton);

            Game.SpriteBatchManager.BackgroundSpriterAnimators.Remove(_animators["MainMenu"]);
            Game.SpriteBatchManager.BackgroundSpriterAnimators.Remove(_animators["XmasTitle"]);
        }

        private void MediaPlayerOnActiveSongChanged(object sender, EventArgs eventArgs)
        {
            //throw new NotImplementedException();
        }

        private void MediaPlayerOnMediaStateChanged(object sender, EventArgs eventArgs)
        {
            if (MediaPlayer.Queue.ActiveSong == null)
                return;

            if (MediaPlayer.Queue.ActiveSong.Name == _introSong.Name &&
                MediaPlayer.Queue.ActiveSong.Position >= _introSong.Duration)
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(_mainSong);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!spriterFrameDataAvailable && _animators["MainMenu"].FrameData != null)
            {
                InitializeGuiButtons();
                spriterFrameDataAvailable = true;
            }

            if (_shootFrequency.TotalMilliseconds < 0)
            {
                Game.GameManager.MoverManager.TriggerPattern(_patternFile, BulletType.Type1, false);
                _shootFrequency = TimeSpan.FromSeconds(1);
            }
            else
            {
                _shootFrequency -= gameTime.ElapsedGameTime;
            }
        }
    }
}