using System.Collections.Generic;
using System.Linq;
using BulletML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using SpriterDotNet;
using SpriterDotNet.MonoGame;
using SpriterDotNet.Providers;
using XmasHell.Physics.Collision;
using XmasHell.Spriter;

namespace XmasHell.Entities.Bosses.XmasLog
{
    class XmasLog : Boss
    {
        private bool _pause = false;
        private KeyboardState _oldKeyboardState;

        public XmasLog(XmasHell game, PositionDelegate playerPositionDelegate) : base(game, playerPositionDelegate)
        {
            // BulletML
            BulletPatternFiles.Add("sample");

            SpriterFilename = "Graphics/Sprites/Bosses/XmasLog/xmas-log";
        }

        protected override void LoadSpriterSprite()
        {
            base.LoadSpriterSprite();

            CurrentAnimator.EventTriggered += CurrentAnimator_EventTriggered;

            CurrentAnimator.Play("Whirligig");
            CurrentAnimator.Speed = 0.25f;

            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionConvexPolygon(this, "body.png", Vector2.Zero));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.A) && _oldKeyboardState.IsKeyUp(Keys.A))
                _pause = !_pause;

            _oldKeyboardState = Keyboard.GetState();

            if (_pause)
                return;

            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //if (_currentAnimator.Position.X > GameConfig.VirtualResolution.X)
            //    _direction = -1f;
            //else if (_currentAnimator.Position.X < 0)
            //    _direction = 1f;

            if (CurrentAnimator.Position.Y > GameConfig.VirtualResolution.Y)
                Direction = new Vector2(0f, -1f);
            else if (CurrentAnimator.Position.Y < 0)
                Direction = new Vector2(0f, 1f);

            CurrentAnimator.Position += (Speed * dt) * Direction;

            CurrentAnimator.Update(gameTime.ElapsedGameTime.Milliseconds);
        }
    }
}