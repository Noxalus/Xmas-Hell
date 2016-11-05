using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Screens;
using Xmas_Hell.Entities;
using Xmas_Hell.Entities.Bosses;
using Xmas_Hell.Entities.Bosses.XmasBall;

namespace Xmas_Hell.Screens
{
    class GameScreen : Screen
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
      }

        public override void Initialize()
        {
            _player = new Player(_game);
            _boss = new XmasBall(_game, _player.Position);

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

        public override void Draw(GameTime gameTime)
        {
            _game.SpriteBatch.Begin(
                samplerState: SamplerState.PointClamp,
                blendState: BlendState.AlphaBlend,
                transformMatrix: _game.Camera.GetViewMatrix()
            );

            _player.Draw(gameTime);
            _boss.Draw(gameTime);

            _game.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}