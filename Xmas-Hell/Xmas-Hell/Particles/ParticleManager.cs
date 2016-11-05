using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.TextureAtlases;

namespace Xmas_Hell.Particles
{
    public class ParticleManager
    {
        private XmasHell _game;
        private List<ParticleEffect> _particleEffects;
        private ParticleEffect _bossHitParticles;
        private ParticleEffect _playerDestroyedParticles;
        private ParticleEffect _bossDestroyedParticles;
        private ParticleEffect _snowFallParticles;

        public ParticleManager(XmasHell game)
        {
            _game = game;
            _particleEffects = new List<ParticleEffect>();
        }

        public void Initialize()
        {
            InitializeParticleEffects();
        }

        private void InitializeParticleEffects()
        {
            _particleEffects.Clear();

            var pixelTextureRegion = new TextureRegion2D(Assets.GetTexture2D("pixel"));

            _bossHitParticles = new ParticleEffect
            {
                Emitters = new[]
                {
                    new ParticleEmitter(
                        pixelTextureRegion,
                        5000,
                        TimeSpan.FromSeconds(2.5),
                        Profile.Circle(30f, Profile.CircleRadiation.Out),
                        false
                    )
                    {
                        Parameters = new ParticleReleaseParameters
                        {
                            Speed = new Range<float>(0f, 50f),
                            Quantity = 3,
                            Rotation = new Range<float>(-1f, 1f),
                            Scale = new Range<float>(3.0f, 4.0f)
                        },
                        Modifiers = new IModifier[]
                        {
                            new RotationModifier { RotationRate = -2.1f },
                        }
                    }
                }
            };

            _playerDestroyedParticles = new ParticleEffect
            {
                Emitters = new[]
                {
                    new ParticleEmitter(
                        pixelTextureRegion,
                        500,
                        TimeSpan.FromSeconds(2.5),
                        Profile.Point(),
                        false
                    )
                    {
                        Parameters = new ParticleReleaseParameters
                        {
                            Speed = new Range<float>(10f, 500f),
                            Quantity = 500,
                            Rotation = new Range<float>(-1f, 1f),
                            Scale = new Range<float>(3.0f, 6.0f),
                            Color = Color.White.ToHsl()
                        },
                        Modifiers = new IModifier[]
                        {
                            new AgeModifier
                            {
                                Interpolators = new IInterpolator[]
                                {
                                    new ColorInterpolator
                                    {
                                        InitialColor = Color.White.ToHsl(),
                                        FinalColor = Color.Black.ToHsl()
                                    }
                                }
                            },
                            new RotationModifier { RotationRate = -2.1f },
                            new RectangleContainerModifier {  Width = 720, Height = 1280 },
                        }
                    }
                }
            };

            _bossDestroyedParticles = new ParticleEffect
            {
                Emitters = new[]
                {
                    new ParticleEmitter(
                        pixelTextureRegion,
                        500,
                        TimeSpan.FromSeconds(2.5),
                        Profile.Point(),
                        false)
                    {
                        Parameters = new ParticleReleaseParameters
                        {
                            Speed = new Range<float>(10f, 500f),
                            Quantity = 500,
                            Rotation = new Range<float>(-1f, 1f),
                            Scale = new Range<float>(3.0f, 6.0f),
                            Color = Color.DarkRed.ToHsl()
                        },
                        Modifiers = new IModifier[]
                        {
                            new RotationModifier { RotationRate = -2.1f },
                            new RectangleContainerModifier {  Width = 720, Height = 1280 },
                        }
                    }
                }
            };

            var snowTextureRegion = new TextureRegion2D(Assets.GetTexture2D("Graphics/Pictures/snow"));

            _snowFallParticles = new ParticleEffect
            {
                Emitters = new[]
                {
                    new ParticleEmitter(
                        snowTextureRegion,
                        5000,
                        TimeSpan.FromSeconds(60),
                        Profile.Spray(new Vector2(0f, -1f), 30f),
                        true
                    )
                    {
                        Parameters = new ParticleReleaseParameters
                        {
                            Speed = new Range<float>(70f, 100f),
                            Scale = new Range<float>(30.0f, 60.0f),
                            Color = (Color.White * 0.75f).ToHsl()
                        }
                    }
                }
            };

            _particleEffects.Add(_bossHitParticles);
            _particleEffects.Add(_playerDestroyedParticles);
            _particleEffects.Add(_bossDestroyedParticles);

            for (int i = 0; i < 1000; i++)
            {
                _snowFallParticles.Trigger(new Vector2(GameConfig.VirtualResolution.X / 2f, -10f));
                _snowFallParticles.Update(1 / 60f);
            }
        }

        public void EmitBossHitParticles(Vector2 position)
        {
            //_bossHitParticles.Trigger(position);
        }

        public void EmitPlayerDestroyedParticles(Vector2 position)
        {
            //_playerDestroyedParticles.Trigger(position);
        }

        public void EmitBossDestroyedParticles(Vector2 position)
        {
            _bossDestroyedParticles.Trigger(position);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var particleEffect in _particleEffects)
                particleEffect.Update(gameTime.GetElapsedSeconds());

            _snowFallParticles.Trigger(new Vector2(GameConfig.VirtualResolution.X / 2f, -10f));
            _snowFallParticles.Update(gameTime.GetElapsedSeconds());
        }

        public void DrawSnowFall()
        {
            _game.SpriteBatch.Draw(_snowFallParticles);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var particleEffect in _particleEffects)
                _game.SpriteBatch.Draw(particleEffect);
        }
    }
}