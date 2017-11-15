using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.TextureAtlases;

namespace XmasHell.Particles
{
    public class ParticleManager
    {
        private XmasHell _game;
        private List<ParticleEffect> _particleEffects;
        private ParticleEffect _bossHitParticles;
        private ParticleEffect _playerDestroyedParticles;
        private ParticleEffect _bossDestroyedParticles;
        private ParticleEffect _snowFallParticles;

        public int ActiveParticlesCount()
        {
            var counter = 0;
            foreach (var particleEffect in _particleEffects)
                counter += particleEffect.ActiveParticles;

            return counter;
        }

        public ParticleManager(XmasHell game)
        {
            _game = game;
            _particleEffects = new List<ParticleEffect>();
        }

        public void Initialize()
        {
            InitializeParticleEffects();
        }

        public void Clear()
        {
            //foreach (var particleEffet in _particleEffects)
            //{
            //    foreach (var particleEmitter in particleEffet.Emitters)
            //    {
            //        particleEmitter.Dispose();
            //    }
            //}
        }

        private void InitializeParticleEffects()
        {
            _particleEffects.Clear();

            var pixelTextureRegion = new TextureRegion2D(Assets.GetTexture2D("pixel"));

            _bossHitParticles = new ParticleEffect
            {
                Name = "BossHitParticles",
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
                Name = "PlayerDestroyedParticles",
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
                            new RotationModifier { RotationRate = -2.1f }
                        }
                    }
                }
            };

            _bossDestroyedParticles = new ParticleEffect
            {
                Name = "BossDestroyedParticles",
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
                        120,
                        TimeSpan.FromSeconds(10),
                        Profile.Line(new Vector2(0f, -1f), 30f),
                        false
                    )
                    {
                        Parameters = new ParticleReleaseParameters
                        {
                            Speed = new Range<float>(50f, 100f),
                            Scale = new Range<float>(30.0f, 100.0f),
                            Color = (Color.White * 0.75f).ToHsl()
                        }
                        //,
                        //Modifiers = new IModifier[]
                        //{
                        //    new OpacityFastFadeModifier(),
                        //}
                    }
                }
            };

            _particleEffects.Add(_bossHitParticles);
            _particleEffects.Add(_playerDestroyedParticles);
            _particleEffects.Add(_bossDestroyedParticles);
            _particleEffects.Add(_snowFallParticles);

            //for (int i = 0; i < 1000; i++)
            //{
            //    _snowFallParticles.Trigger(new Vector2(GameConfig.VirtualResolution.X / 2f, -10f));
            //    _snowFallParticles.Update(1 / 60f);
            //}

            _game.SpriteBatchManager.BackgroundParticles.Add(_snowFallParticles);
            _game.SpriteBatchManager.GameParticles.Add(_playerDestroyedParticles);
            _game.SpriteBatchManager.BackgroundParticles.Add(_bossHitParticles);
            _game.SpriteBatchManager.BackgroundParticles.Add(_bossDestroyedParticles);
        }

        public void EmitBossHitParticles(Vector2 position)
        {
            //if (!GameConfig.DisableParticles)
            //    _bossHitParticles.Trigger(position);
        }

        public void EmitPlayerDestroyedParticles(Vector2 position)
        {
            if (!GameConfig.DisableParticles)
                _playerDestroyedParticles.Trigger(position);
        }

        public void EmitBossDestroyedParticles(Vector2 position)
        {
            if (!GameConfig.DisableParticles)
                _bossDestroyedParticles.Trigger(position);
        }

        public void Update(GameTime gameTime)
        {
            if (GameConfig.DisableParticles)
                return;

            foreach (var particleEffect in _particleEffects)
                particleEffect.Update(gameTime.GetElapsedSeconds());

            //_snowFallParticles.Trigger(new Vector2(GameConfig.VirtualResolution.X / 2f, GameConfig.VirtualResolution.Y / 2f));
        }
    }
}