using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using Xmas_Hell.Geometry;

namespace Xmas_Hell.Entities.Bosses.XmasBall
{
    class XmasBallBehaviour3 : AbstractBossBehaviour
    {
        private Line _bossPlayerLine;
        private readonly Line _leftWallLine;
        private readonly Line _bottomWallLine;
        private readonly Line _upWallLine;
        private readonly Line _rightWallLine;

        public XmasBallBehaviour3(Boss boss) : base(boss)
        {
            _bottomWallLine = new Line(
                new Vector2(0f, GameConfig.VirtualResolution.Y),
                new Vector2(GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y)
            );

            _leftWallLine = new Line(
                new Vector2(0f, 0f),
                new Vector2(0f, GameConfig.VirtualResolution.Y)
            );

            _rightWallLine = new Line(
                new Vector2(GameConfig.VirtualResolution.X, 0f),
                new Vector2(GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y)
            );

            _upWallLine = new Line(
                new Vector2(0f, 0f),
                new Vector2(GameConfig.VirtualResolution.X, 0f)
            );
        }

        public override void Start()
        {
            base.Start();

            Boss.Speed = 500f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Compute the ray between boss position and player's position
            var currentPosition = Boss.CurrentAnimator.Position;
            var playerDirection = Boss.GetPlayerDirection();
            var maxDistance = (float)Math.Sqrt(
                GameConfig.VirtualResolution.X*GameConfig.VirtualResolution.X +
                GameConfig.VirtualResolution.Y*GameConfig.VirtualResolution.Y
            );

            var fartherPlayerPosition = currentPosition + (playerDirection * maxDistance);

            _bossPlayerLine = new Line(
                currentPosition,
                fartherPlayerPosition
            );

            var newPosition = Vector2.Zero;

            var haveNewPosition =
                MathHelperExtension.LinesIntersect(_bottomWallLine, _bossPlayerLine, ref newPosition) ||
                MathHelperExtension.LinesIntersect(_leftWallLine, _bossPlayerLine, ref newPosition) ||
                MathHelperExtension.LinesIntersect(_rightWallLine, _bossPlayerLine, ref newPosition) ||
                MathHelperExtension.LinesIntersect(_upWallLine, _bossPlayerLine, ref newPosition);

            if (haveNewPosition)
                Boss.MoveTo(newPosition);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawLine(_bossPlayerLine.First, _bossPlayerLine.Second, Color.Red, 5f);
            spriteBatch.DrawLine(_leftWallLine.First, _leftWallLine.Second, Color.White, 5f);
            spriteBatch.DrawLine(_bottomWallLine.First, _bottomWallLine.Second, Color.Yellow, 5f);
            spriteBatch.DrawLine(_rightWallLine.First, _rightWallLine.Second, Color.Brown, 5f);
            spriteBatch.DrawLine(_upWallLine.First, _upWallLine.Second, Color.Green, 5f);
        }
    }
}