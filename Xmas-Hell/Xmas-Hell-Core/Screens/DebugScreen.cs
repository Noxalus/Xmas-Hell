using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using XmasHell.Entities;
using XmasHell.Entities.Bosses;

namespace XmasHell.Screens
{
    public class DebugScreen : Screen
    {
        private readonly XmasHell _game;
        private Player _player;
        private Boss _boss;

        private float GetRank()
        {
            return 1f;
        }

        public DebugScreen(XmasHell game)
        {
            _game = game;
            GameManager.GameDifficulty = GetRank;

            _player = new Player(_game);
        }

        // TODO: This should be handled by the ScreenManager
        public void Show()
        {
            _player.Initialize();
            _boss = BossFactory.CreateBoss(BossType.Debug, _game, _player.Position);
            _boss.Initialize();
        }

        public override void Dispose()
        {
            base.Dispose();

            _boss.Dispose();
            _player.Dispose();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_game.Pause)
                return;

            if (_player.Alive())
                _player.Update(gameTime);

            _boss.Update(gameTime);
        }
    }
}