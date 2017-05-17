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

        private float GetRank()
        {
            return 1f;
        }

        public GameScreen(XmasHell game) : base(game)
        {
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
            _boss.Initialize();
        }

        // TODO: This should be handled by the ScreenManager
        public override void Show(bool reset = false)
        {
            base.Show(reset);

            _player.Initialize();

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
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputManager.PressedCancel())
            {
                Hide();
                Game.ScreenManager.GoTo<MainMenuScreen>();
            }

            if (Game.Pause)
                return;

            if (!Game.GameManager.EndGame())
            {
                if (_player.Alive())
                    _player.Update(gameTime);

                _boss.Update(gameTime);
            }
        }
    }
}