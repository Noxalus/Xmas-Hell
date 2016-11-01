using System;
using System.Collections.Generic;
using System.Linq;
using BulletML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using SpriterDotNet;
using SpriterDotNet.MonoGame;
using SpriterDotNet.Providers;
using Xmas_Hell.Physics.Collision;
using Xmas_Hell.Spriter;

namespace Xmas_Hell.Entities.Bosses
{
    class XmasLog : Boss
    {
        private bool _pause = false;
        private KeyboardState _oldKeyboardState;

        public XmasLog(XmasHell game) : base(game)
        {
            Game = game;

            // Spriter
            DefaultProviderFactory<Sprite, SoundEffect> factory = new DefaultProviderFactory<Sprite, SoundEffect>(DefaultAnimatorConfig, true);

            SpriterContentLoader loader = new SpriterContentLoader(Game.Content, "Graphics/Sprites/Bosses/XmasLog/xmas-log");
            loader.Fill(factory);

            foreach (SpriterEntity entity in loader.Spriter.Entities)
            {
                var animator = new MonoGameDebugAnimator(entity, Game.GraphicsDevice, factory);
                Animators.Add(animator);
            }

            CurrentAnimator = Animators.First();
            CurrentAnimator.EventTriggered += CurrentAnimator_EventTriggered;

            CurrentAnimator.Play("Whirligig");
            CurrentAnimator.Speed = 0.25f;

            // Physics
            var bossSpriteSize = new Vector2(119, 231);
            var bossPivot = new Vector2(0.5f, 0.55f);
            var bossCollisionBoxVertices = new List<Vector2>()
            {
                new Vector2(-(bossSpriteSize.X * bossPivot.X),  -(bossSpriteSize.Y * bossPivot.Y)),
                new Vector2(bossSpriteSize.X * bossPivot.X, -(bossSpriteSize.Y * bossPivot.Y)),
                new Vector2(bossSpriteSize.X * bossPivot.X, bossSpriteSize.Y - (bossSpriteSize.Y * bossPivot.Y)),
                new Vector2(-(bossSpriteSize.X * bossPivot.X), bossSpriteSize.Y - (bossSpriteSize.Y * bossPivot.Y))
            };
            Game.GameManager.CollisionWorld.BossHitbox = new CollisionConvexPolygon(this, Vector2.Zero, bossCollisionBoxVertices);
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
                Direction = -1f;
            else if (CurrentAnimator.Position.Y < 0)
                Direction = 1f;

            CurrentAnimator.Position += new Vector2(0f, 200f * dt) * Direction;

            CurrentAnimator.Update(gameTime.ElapsedGameTime.Milliseconds);
        }
    }
}