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
using System.Linq;

namespace XmasHell.Screens
{
    public class MainMenuScreen : Screen
    {
        private string _patternFile = "MainMenu/snowflake";
        private TimeSpan _shootFrequency;
        private Song _introSong;
        private Song _mainSong;

        private GuiButton _playButton;
        private TweenAnimation<GuiButton> _playButtonPulseTweenChain;
        private TweenAnimation<GuiButton> _playButtonRotateTweenChain;

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

        private readonly IList<CustomSpriterAnimator> _animators = new List<CustomSpriterAnimator>();
        public CustomSpriterAnimator CurrentAnimator;

        public MainMenuScreen(XmasHell game) : base(game)
        {
            SpriterFilename = "Graphics/GUI/MainMenu/main-menu";
        }

        public override void Initialize()
        {
            _shootFrequency = TimeSpan.Zero;

            base.Initialize();
        }

        private void PlayButtonTweenPulse()
        {
            _playButtonPulseTweenChain = _playButton.CreateTweenChain(PlayButtonTweenPulse)
                .Scale(new Vector2(1.1f), 0.75f, EasingFunctions.SineEaseInOut)
                .Scale(new Vector2(1f), 0.75f, EasingFunctions.SineEaseIn)
            ;
        }

        private void PlayButtonTweenRotate()
        {
            _playButtonRotateTweenChain = _playButton.CreateTweenChain(PlayButtonTweenRotate)
                .RotateTo(MathHelper.PiOver4 / 2f, 1.0f, EasingFunctions.SineEaseIn)
                .RotateTo(-MathHelper.PiOver4 / 2f, 1.0f, EasingFunctions.SineEaseOut)
            ;
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
            _playButton = new GuiButton(Game.ViewportAdapter, "play-button", new Sprite(Assets.GetTexture2D("Graphics/GUI/MainMenu/play-button")));

#if ANDROID
            _playButton.Tap += (s, e) => OnPlayButtonAction();
#else
            _playButton.Click += (s, e) => OnPlayButtonAction();
#endif

            LoadSpriterSprite();
        }

        private void InitializeGuiButtons()
        {
            // Place buttons according to their dummy positions on the Spriter file
            var spriterPlayButtonPosition = SpriterUtils.GetSpriterFilePosition("play-button.png", CurrentAnimator);
            _playButton.Position = Game.ViewportAdapter.Center.ToVector2() + spriterPlayButtonPosition;
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
                _animators.Add(animator);
            }

            CurrentAnimator = _animators.First();
            CurrentAnimator.Position = Game.ViewportAdapter.Center.ToVector2();
            Game.SpriteBatchManager.BackgroundSpriterAnimators.Add(CurrentAnimator);
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

            // GUI
            _playButton.Scale = Vector2.One;
            _playButton.Rotation = 0f;
            Game.GuiManager.AddButton(_playButton);

            PlayButtonTweenPulse();
            PlayButtonTweenRotate();
        }

        public override void Hide()
        {
            base.Hide();

            MediaPlayer.Stop();

            MediaPlayer.MediaStateChanged -= MediaPlayerOnMediaStateChanged;
            MediaPlayer.ActiveSongChanged -= MediaPlayerOnActiveSongChanged;

            // GUI
            Game.GuiManager.RemoveButton(_playButton);
            Game.SpriteBatchManager.BackgroundSpriterAnimators.Remove(CurrentAnimator);

            _playButtonPulseTweenChain.Stop();
            _playButtonRotateTweenChain.Stop();
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

            if (!spriterFrameDataAvailable && CurrentAnimator.FrameData != null)
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