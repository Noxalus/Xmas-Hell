using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Extended.Screens;
using Xmas_Hell.Entities.Bosses;

namespace Xmas_Hell.Screens
{
    public class MainMenuScreen : Screen
    {
        private XmasHell _game;
        private TouchCollection _previousTouchState;

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
            var currentTouchState = TouchPanel.GetState();

            if (currentTouchState.Count == 0 && _previousTouchState.Count == 1)
            {
                var touchPosition = _game.ViewportAdapter.PointToScreen(_previousTouchState[0].Position.ToPoint());

                if (touchPosition.X < GameConfig.VirtualResolution.X/2f)
                    _game.GameScreen.LoadBoss(BossType.XmasBall);
                else
                    _game.GameScreen.LoadBoss(BossType.XmasBell);

                Show<GameScreen>();
            }

            _previousTouchState = currentTouchState;

            base.Update(gameTime);
        }
    }
}