using System;
using BulletML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Screens;
using XmasHell.BulletML;
using XmasHell.Entities.Bosses;
using Xmas_Hell_Core.Controls;

namespace XmasHell.Screens
{
    public class MainMenuScreen : Screen
    {
        private XmasHell _game;
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
            if (_game.GameManager.MoverManager.FindPattern(_patternFile) == null)
            {
                var pattern = new BulletPattern();
                pattern.ParseStream(_patternFile, Assets.GetPattern(_patternFile));

                _game.GameManager.MoverManager.AddPattern(_patternFile, pattern);
            }

            _shootFrequency = TimeSpan.Zero;

            base.Initialize();

            // Should play music (doesn't seem to work for now...)
            MediaPlayer.Volume = 1f;
            _introSong = Assets.GetMusic("boss-theme-intro");
            _mainSong = Assets.GetMusic("boss-theme-main");

            MediaPlayer.MediaStateChanged += MediaPlayerOnMediaStateChanged;
            MediaPlayer.ActiveSongChanged += MediaPlayerOnActiveSongChanged;

            MediaPlayer.Stop();

            //MediaPlayer.Play(_introSong);
            //MediaPlayer.Play(_mainSong);
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

            if (_game.Pause)
                return;

            if (InputManager.TouchUp() || InputManager.Clicked())
            {
#if ANDROID
                var position = _game.ViewportAdapter.PointToScreen(InputManager.TouchPosition());
#else
                var position = _game.ViewportAdapter.PointToScreen(InputManager.ClickPosition());
#endif

                if (position.X < GameConfig.VirtualResolution.X / 2f && position.Y < GameConfig.VirtualResolution.Y / 2f)
                    _game.GameScreen.LoadBoss(BossType.XmasBall);
                else if (position.X > GameConfig.VirtualResolution.X / 2f && position.Y < GameConfig.VirtualResolution.Y / 2f)
                    _game.GameScreen.LoadBoss(BossType.XmasBell);
                else if (position.X < GameConfig.VirtualResolution.X / 2f && position.Y > GameConfig.VirtualResolution.Y / 2f)
                    _game.GameScreen.LoadBoss(BossType.XmasGift);
                else if (position.X > GameConfig.VirtualResolution.X / 2f && position.Y > GameConfig.VirtualResolution.Y / 2f)
                    _game.GameScreen.LoadBoss(BossType.XmasSnowflake);

                Show<GameScreen>();
            }

            if (_shootFrequency.TotalMilliseconds < 0)
            {
                _game.GameManager.MoverManager.TriggerPattern(_patternFile, BulletType.Type1, false);
                _shootFrequency = TimeSpan.FromSeconds(1);
            }
            else
            {
                _shootFrequency -= gameTime.ElapsedGameTime;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _game.SpriteBatch.Begin(transformMatrix: _game.Camera.GetViewMatrix());
            _game.SpriteBatch.Draw(Assets.GetTexture2D("pixel"), new Rectangle(0, 0, GameConfig.VirtualResolution.X / 2, GameConfig.VirtualResolution.Y / 2), null, Color.Red);
            _game.SpriteBatch.Draw(Assets.GetTexture2D("pixel"), new Rectangle(GameConfig.VirtualResolution.X / 2, 0, GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y / 2), null, Color.Green);
            _game.SpriteBatch.Draw(Assets.GetTexture2D("pixel"), new Rectangle(0, GameConfig.VirtualResolution.Y / 2, GameConfig.VirtualResolution.X / 2, GameConfig.VirtualResolution.Y / 2), null, Color.Yellow);
            _game.SpriteBatch.Draw(Assets.GetTexture2D("pixel"), new Rectangle(GameConfig.VirtualResolution.X / 2, GameConfig.VirtualResolution.Y / 2, GameConfig.VirtualResolution.X / 2, GameConfig.VirtualResolution.Y / 2), null, Color.Blue);
            _game.SpriteBatch.End();
        }
    }
}