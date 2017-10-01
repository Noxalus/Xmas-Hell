using System;
using BulletML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using XmasHell.BulletML;
using XmasHell.GUI;
using XmasHell.Spriter;
using XmasHell.Rendering;

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

        public MainMenuScreen(XmasHell game) : base(game)
        {
        }

        public override void Initialize()
        {
            _shootFrequency = TimeSpan.Zero;

            base.Initialize();
        }

        private void OnPlayButtonAction(object s, Point e)
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

            LoadSpriterSprite("Graphics/GUI/MainMenu/main-menu");
        }

        protected override void InitializeSpriterGui()
        {
            // Title
            var xmasTitleDummyPosition = SpriterUtils.GetSpriterFilePosition("xmas-title.png", Animators["MainMenu"]);
            Animators["XmasTitle"].Position = Game.ViewportAdapter.Center.ToVector2() + xmasTitleDummyPosition;

            // Place buttons according to their dummy positions on the Spriter file
            var spriterPlayButtonDummyPosition = SpriterUtils.GetSpriterFilePosition("play-button.png", Animators["MainMenu"]);
            Animators["PlayButton"].Position = Game.ViewportAdapter.Center.ToVector2() + spriterPlayButtonDummyPosition;

            _playButton = new SpriterGuiButton(Game.ViewportAdapter, "PlayButton", "play-button.png", Animators["PlayButton"]);

            ResetUI();
        }

        private void ResetUI()
        {
            if (_playButton != null)
            {
#if ANDROID
                _playButton.Tap += OnPlayButtonAction;
#else
                _playButton.Click += OnPlayButtonAction;
#endif
                Game.GuiManager.AddButton(_playButton);
            }

            if (Animators["MainMenu"] != null)
                Game.SpriteBatchManager.AddSpriterAnimator(Animators["MainMenu"], Layer.BACKGROUND);
            if (Animators["XmasTitle"] != null)
                Game.SpriteBatchManager.AddSpriterAnimator(Animators["XmasTitle"], Layer.BACKGROUND);
        }

        protected override void LoadSpriterSprite(String spriterFilename)
        {
            base.LoadSpriterSprite(spriterFilename);

            Animators["MainMenu"].Position = Game.ViewportAdapter.Center.ToVector2();
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

            ResetUI();
        }

        public override void Hide()
        {
            base.Hide();

            MediaPlayer.Stop();

            MediaPlayer.MediaStateChanged -= MediaPlayerOnMediaStateChanged;
            MediaPlayer.ActiveSongChanged -= MediaPlayerOnActiveSongChanged;

            // GUI
            Game.GuiManager.RemoveButton(_playButton);

#if ANDROID
            _playButton.Tap -= OnPlayButtonAction;
#else
            _playButton.Click -= OnPlayButtonAction;
#endif

            Game.SpriteBatchManager.RemoveSpriterAnimator(Animators["MainMenu"], Layer.BACKGROUND);
            Game.SpriteBatchManager.RemoveSpriterAnimator(Animators["XmasTitle"], Layer.BACKGROUND);
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