using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using XmasHell.Entities;
using XmasHell.Entities.Bosses;

namespace XmasHell.Screens
{
    public class DebugScreen : Screen
    {
        private Player _player;
        private Boss _boss;

        private float GetRank()
        {
            return 1f;
        }

        public DebugScreen(XmasHell game) : base(game)
        {
            GameManager.GameDifficulty = GetRank;
            _player = new Player(game);
        }

        // TODO: This should be handled by the ScreenManager
        public override void Show(bool reset = false)
        {
            base.Show(reset);

            _player.Initialize();
            _boss = BossFactory.CreateBoss(BossType.Debug, Game, _player.Position);
            _boss.Initialize();
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

            if (Game.Pause)
                return;

            if (!Game.GameManager.GameIsFinished())
            {
                if (_player.Alive())
                    _player.Update(gameTime);

                _boss.Update(gameTime);
            }
        }
    }
}