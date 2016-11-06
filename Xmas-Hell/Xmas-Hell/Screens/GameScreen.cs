using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Screens;
using Xmas_Hell.Entities;
using Xmas_Hell.Entities.Bosses;

namespace Xmas_Hell.Screens
{
    public class GameScreen : Screen
    {
        private readonly XmasHell _game;
        private Player _player;
        private Boss _boss;

        private float GetRank()
        {
            return 1f;
        }

        public GameScreen(XmasHell game)
        {
            _game = game;
            GameManager.GameDifficulty = GetRank;

            _player = new Player(_game);
        }

        public override void Initialize()
        {
            _player.Initialize();

            base.Initialize();

            // Should play music (doesn't seem to work for now...)
            MediaPlayer.Volume = 1f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(Assets.GetMusic("Audio/BGM/boss-theme"));
        }

        public void LoadBoss(BossType bossType)
        {
            _boss = BossFactory.CreateBoss(bossType, _game, _player.Position);
            _boss.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            _player.Update(gameTime);
            _boss.Update(gameTime);

            base.Update(gameTime);
        }
    }
}