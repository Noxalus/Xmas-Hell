using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Sprites;
using Xmas_Hell.Entities;

namespace Xmas_Hell.Screens
{
    class GameScreen : Screen
    {
        private readonly XmasHell _game;
        private Player _player;
        private Boss _boss;

        public GameScreen(XmasHell game)
        {
            _game = game;
        }

        public override void Initialize()
        {
            _player = new Player(_game);
            var bossPosition = new Vector2(
                Config.VirtualResolution.X / 2f,
                150f
            );
            _boss = new Boss(_game, bossPosition, 100);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            _player.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _game.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: _game.ViewportAdapter.GetScaleMatrix());

            _player.Draw(gameTime);
            _boss.Draw(gameTime);

            _game.SpriteBatch.End();
        }
    }
}