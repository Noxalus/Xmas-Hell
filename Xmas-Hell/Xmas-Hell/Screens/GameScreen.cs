using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Screens;
using Xmas_Hell.Entities;
using Xmas_Hell.Entities.Bosses;
using Xmas_Hell.Entities.Bosses.XmasBall;
using Xmas_Hell.Entities.Bosses.XmasBell;

namespace Xmas_Hell.Screens
{
    class GameScreen : Screen
    {
        private readonly XmasHell _game;
        private Player _player;
        private BossType _bossType;
        private Boss _boss;

        private float GetRank()
        {
            return 1f;
        }

        public GameScreen(XmasHell game, BossType bossType)
        {
            _game = game;
            _bossType = bossType;
            GameManager.GameDifficulty = GetRank;
      }

        public override void Initialize()
        {
            _player = new Player(_game);
            _boss = BossFactory.CreateBoss(_bossType, _game, _player.Position);

            base.Initialize();

            _boss.Initialize();

            // Should play music (doesn't seem to work for now...)
            MediaPlayer.Volume = 1f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(Assets.GetMusic("Audio/BGM/boss-theme"));
        }

        public override void Update(GameTime gameTime)
        {
            _player.Update(gameTime);
            _boss.Update(gameTime);

            base.Update(gameTime);
        }
    }
}