using System;
using BulletML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Screens;
using XmasHell.BulletML;
using XmasHell.Entities.Bosses;

namespace XmasHell.Screens
{
    public class MainMenuScreen : Screen
    {
        private XmasHell _game;
        private TouchCollection _previousTouchState;
        private string _patternFile = "MainMenu/snowflake";
        private TimeSpan _shootFrequency;
        private Song _introSong;
        private Song _mainSong;

        public MainMenuScreen(XmasHell game)
        {
            _game = game;
        }

        public override void Initialize()
        {
            var pattern = new BulletPattern();
            pattern.ParseStream(_patternFile, Assets.GetPattern(_patternFile));

            _game.GameManager.MoverManager.AddPattern(_patternFile, pattern);

            _shootFrequency = TimeSpan.Zero;

            base.Initialize();

            // Should play music (doesn't seem to work for now...)
            MediaPlayer.Volume = 1f;
            _introSong = Assets.GetMusic("boss-theme-intro");
            _mainSong = Assets.GetMusic("boss-theme-main");

            MediaPlayer.MediaStateChanged += MediaPlayerOnMediaStateChanged;
            MediaPlayer.ActiveSongChanged += MediaPlayerOnActiveSongChanged;

            //MediaPlayer.Stop();

            MediaPlayer.Play(_introSong);
            MediaPlayer.Play(_mainSong);
        }

        private void MediaPlayerOnActiveSongChanged(object sender, EventArgs eventArgs)
        {
            //throw new NotImplementedException();
        }

        private void MediaPlayerOnMediaStateChanged(object sender, EventArgs eventArgs)
        {
            if (MediaPlayer.Queue.ActiveSong.Name == _introSong.Name &&
                MediaPlayer.Queue.ActiveSong.Position >= _introSong.Duration)
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(_mainSong);
            }
        }

        public override void Update(GameTime gameTime)
        {
            //Console.WriteLine("Position: " + _introSong.Position + " | Duration: " + _introSong.Duration);
            _game.GameScreen.LoadBoss(BossType.XmasBall);
            Show<GameScreen>();
            return;

            var currentTouchState = TouchPanel.GetState();

            if (currentTouchState.Count == 0 && _previousTouchState.Count == 1)
            {
                var touchPosition = _game.ViewportAdapter.PointToScreen(_previousTouchState[0].Position.ToPoint());

                if (touchPosition.X < GameConfig.VirtualResolution.X / 2f)
                    _game.GameScreen.LoadBoss(BossType.XmasBall);
                else
                    _game.GameScreen.LoadBoss(BossType.XmasBell);

                Show<GameScreen>();
            }

            _previousTouchState = currentTouchState;

            if (_shootFrequency.TotalMilliseconds < 0)
            {
                _game.GameManager.MoverManager.TriggerPattern(_patternFile, BulletType.Type1, false);
                _shootFrequency = TimeSpan.FromSeconds(1);
            }
            else
            {
                _shootFrequency -= gameTime.ElapsedGameTime;
            }

            base.Update(gameTime);
        }
    }
}