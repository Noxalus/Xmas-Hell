using System;
using System.Collections.Generic;
using System.Linq;
using BulletML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using SpriterDotNet;
using SpriterDotNet.MonoGame;
using SpriterDotNet.Providers;
using Xmas_Hell.Physics.Collision;
using Xmas_Hell.Spriter;

namespace Xmas_Hell.Entities.Bosses
{
    class XmasBall : Boss
    {
        public XmasBall(XmasHell game) : base(game)
        {
            Game = game;

            // Spriter
            DefaultProviderFactory<Sprite, SoundEffect> factory = new DefaultProviderFactory<Sprite, SoundEffect>(DefaultAnimatorConfig, true);

            SpriterContentLoader loader = new SpriterContentLoader(Game.Content, "Graphics/Sprites/Bosses/XmasBall/xmas-ball");
            loader.Fill(factory);

            foreach (SpriterEntity entity in loader.Spriter.Entities)
            {
                var animator = new MonoGameDebugAnimator(entity, Game.GraphicsDevice, factory);
                Animators.Add(animator);
            }

            CurrentAnimator = Animators.First();
            CurrentAnimator.EventTriggered += CurrentAnimator_EventTriggered;

            // BulletML
            BossPatterns = new List<BulletPattern>();
            BossBulletFrequence = TimeSpan.Zero;

            // Load the pattern
            var pattern = new BulletPattern();
            BossPatterns.Add(pattern);

            var filename = "sample";
            pattern.ParseStream(filename, Assets.GetPattern(filename));

            // Physics
            Game.GameManager.CollisionWorld.BossHitbox = new CollisionCircle(this, Vector2.Zero, 86f);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

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

            if (BossBulletFrequence.TotalMilliseconds > 0)
                BossBulletFrequence -= gameTime.ElapsedGameTime;
            else
            {
                BossBulletFrequence = TimeSpan.FromTicks(GameConfig.PlayerShootFrequency.Ticks);
                AddBullet();
            }
        }
    }
}