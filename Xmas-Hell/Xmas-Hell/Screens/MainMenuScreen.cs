using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;

namespace Xmas_Hell.Screens
{
    class MainMenuScreen : Screen
    {
        private XmasHell _game;

        public MainMenuScreen(XmasHell game)
        {
            _game = game;
        }

        public override void Initialize()
        {
            base.Initialize();


        }

        public override void Update(GameTime gameTime)
        {
            Show<GameScreen>();

            base.Update(gameTime);
        }
    }
}