using System;
using System.Collections.Generic;
using System.Linq;
using BulletML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended;
using SpriterDotNet;
using SpriterDotNet.MonoGame;
using SpriterDotNet.Providers;
using Xmas_Hell.Physics.Collision;
using Xmas_Hell.Spriter;

namespace Xmas_Hell.Entities.Bosses
{
    class XmasBall : Boss
    {
        private enum BossState
        {
            Pattern1,
            Pattern2,
            Pattern3,
            Pattern4
        };

        private BossState _currentPatternState;
        private TimeSpan _newPositionTime;

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

            // Update boss state
            if (Life < InitialLife/4f)
                _currentPatternState = BossState.Pattern4;
            else if (Life < 2 * (InitialLife / 4f))
                _currentPatternState = BossState.Pattern3;
            else if (Life < 3 * (InitialLife / 4f))
                _currentPatternState = BossState.Pattern2;
            else if (Life <= InitialLife)
                _currentPatternState = BossState.Pattern1;

            switch (_currentPatternState)
            {
                case BossState.Pattern1:
                    Behaviour1(gameTime);
                    break;

                case BossState.Pattern2:
                    Behaviour2(gameTime);
                break;

                case BossState.Pattern3:
                    Behaviour3(gameTime);
                break;

                case BossState.Pattern4:
                    Behaviour4(gameTime);
                break;
            }

            CurrentAnimator.Update(gameTime.ElapsedGameTime.Milliseconds);
        }

        private void Behaviour1(GameTime gameTime)
        {
            if (CurrentAnimator.Position.X > GameConfig.VirtualResolution.X - (Width() / 2f))
                Direction = -(float)Math.PI;
            else if (CurrentAnimator.Position.X < Width() / 2f)
                Direction = 0f;

            // TODO: Add this to an extension of the MathHelper
            var direction = new Vector2(
                (float)Math.Cos(Direction),
                -(float)Math.Sin(Direction)
            );

            //CurrentAnimator.Position += new Vector2(Speed * gameTime.GetElapsedSeconds(), 0f) * direction;

            if (_newPositionTime.TotalMilliseconds > 0)
            {
                if (!TargetingPosition)
                    _newPositionTime -= gameTime.ElapsedGameTime;
            }
            else
            {
                _newPositionTime = TimeSpan.FromSeconds((Game.GameManager.Random.NextDouble() * 2.5) + 0.5);
                _newPositionTime = TimeSpan.FromMilliseconds(30);

                var newPosition = new Vector2(
                    Game.GameManager.Random.Next((int)(Width() / 2f), GameConfig.VirtualResolution.X - (int)(Width() / 2f)),
                    Game.GameManager.Random.Next((int)(Height() / 2f), GameConfig.VirtualResolution.Y - (int)(Height() / 2f))
                );

                MoveTo(newPosition);
            }

            if (BossBulletFrequence.TotalMilliseconds > 0)
                BossBulletFrequence -= gameTime.ElapsedGameTime;
            else
            {
                BossBulletFrequence = TimeSpan.FromTicks(GameConfig.PlayerShootFrequency.Ticks);
                AddBullet();
            }
        }

        private void Behaviour2(GameTime gameTime)
        {

        }

        private void Behaviour3(GameTime gameTime)
        {

        }

        private void Behaviour4(GameTime gameTime)
        {

        }
    }
}