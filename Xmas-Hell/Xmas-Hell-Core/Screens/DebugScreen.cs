using System;
using Microsoft.Xna.Framework;
using XmasHell.Entities.Bosses;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended;
using MonoGame.Extended.Particles.Profiles;

namespace XmasHell.Screens
{
    public class DebugScreen : GameScreen
    {
        public DebugScreen(XmasHell game) : base(game)
        {
            Type = ScreenType.Game;

            var particleEffect = new ParticleEffect
            {
                Name = "StartLaser",

                Emitters = new[]
                {
                    new ParticleEmitter(
                        Game.GameManager.ParticleManager.Pixel(),
                        500,
                        TimeSpan.FromSeconds(10.5),
                        Profile.Point(),
                        true)
                    {
                        Parameters = new ParticleReleaseParameters
                        {
                            Speed = new Range<float>(10f, 500f),
                            Quantity = 500,
                            Rotation = new Range<float>(-1f, 1f),
                            Scale = new Range<float>(3.0f, 6.0f),
                            Color = Color.DarkGoldenrod.ToHsl()
                        },
                        Modifiers = new IModifier[]
                        {
                            new RotationModifier { RotationRate = -2.1f }
                        }
                    }
                }
            };

            game.GameManager.ParticleManager.EmitParticles(particleEffect, new Vector2(GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y));
        }

        public override void Show(bool reset = false)
        {
            Game.GameManager.LoadBoss(BossType.Debug);

            base.Show(reset);
        }

        public override void Hide()
        {
            base.Hide();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}