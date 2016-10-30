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
    class XmasLog : Boss
    {
        public override Vector2 Position()
        {
            if (CurrentAnimator.FrameData != null && CurrentAnimator.FrameData.SpriteData.Count > 0)
            {
                var spriteData = CurrentAnimator.FrameData.SpriteData[0];
                return base.Position() + new Vector2(spriteData.X, -spriteData.Y);
            }

            return base.Position();
        }

        public override float Rotation()
        {
            if (CurrentAnimator.FrameData != null && CurrentAnimator.FrameData.SpriteData.Count > 0)
            {
                var spriteData = CurrentAnimator.FrameData.SpriteData[0];
                return MathHelper.ToRadians(-spriteData.Angle);
            }

            return base.Rotation();
        }

        public override Vector2 Scale()
        {
            if (CurrentAnimator.FrameData != null && CurrentAnimator.FrameData.SpriteData.Count > 0)
            {
                var spriteData = CurrentAnimator.FrameData.SpriteData[0];
                return new Vector2(spriteData.ScaleX, spriteData.ScaleY);
            }

            return base.Scale();
        }

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

            CurrentAnimator.Play("Expand");

            // BulletML
            BossPatterns = new List<BulletPattern>();
            BossBulletFrequence = TimeSpan.Zero;

            // Load the pattern
            var pattern = new BulletPattern();
            BossPatterns.Add(pattern);

            var filename = "sample";
            pattern.ParseStream(filename, Assets.GetPattern(filename));

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