using System;
using BulletML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Screens;
using XmasHell.BulletML;
using XmasHell.Entities.Bosses;
using Xmas_Hell_Core.Controls;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Tweening;
using XmasHell.GUI;

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

        public MainMenuScreen(XmasHell game) : base(game)
        {
        }

        public override void Initialize()
        {
            _shootFrequency = TimeSpan.Zero;

            base.Initialize();

            _playButton.Position = Game.ViewportAdapter.Center.ToVector2();

#if ANDROID
            _playButton.Tap += (s, e) => OnPlayButtonAction();
#else
            _playButton.Click += (s, e) => OnPlayButtonAction();
#endif
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

            if (Game.Pause)
                return;

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