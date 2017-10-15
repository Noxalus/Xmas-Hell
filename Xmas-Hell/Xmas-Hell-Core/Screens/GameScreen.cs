using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Screens;
using XmasHell.Entities;
using XmasHell.Entities.Bosses;
using Xmas_Hell_Core.Controls;

namespace XmasHell.Screens
{
    public class GameScreen : Screen
    {
        private Player _player;
        private Boss _boss;
        private TimeSpan _playTime;

        private float GetRank()
        {
            return 1f;
        }

        public GameScreen(XmasHell game) : base(game)
        {
            ShouldBeStackInHistory = true;
            GameManager.GameDifficulty = GetRank;

            _player = new Player(game);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void LoadBoss(BossType bossType)
        {
            _boss = BossFactory.CreateBoss(bossType, Game, _player.Position);
        }

        // TODO: This should be handled by the ScreenManager
        public override void Show(bool reset = false)
        {
            base.Show(reset);

            _player.Initialize();
            _boss.Initialize();

            _playTime = TimeSpan.Zero;
            Game.PlayerData.BossAttempts(_boss.BossType, Game.PlayerData.BossAttempts(_boss.BossType) + 1);

            // Should play music (doesn't seem to work for now...)
            //MediaPlayer.Volume = 1f;
            //MediaPlayer.IsRepeating = true;
            //MediaPlayer.Play(Assets.GetMusic("Audio/BGM/boss-theme"));
        }

        public override void Hide()
        {
            base.Hide();

            _boss.Dispose();
            _player.Dispose();
            Game.GameManager.Clear();

            Game.PlayerData.BossPlayTime(_boss.BossType, Game.PlayerData.BossPlayTime(_boss.BossType) + _playTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputManager.PressedCancel())
                Game.ScreenManager.GoTo<BossSelectionScreen>();

            _playTime += gameTime.ElapsedGameTime;

            if (!Game.GameManager.EndGame())
            {
                if (_player.Alive())
                    _player.Update(gameTime);

                if (_boss.Alive())
                    _boss.Update(gameTime);
            }
        }
    }
}