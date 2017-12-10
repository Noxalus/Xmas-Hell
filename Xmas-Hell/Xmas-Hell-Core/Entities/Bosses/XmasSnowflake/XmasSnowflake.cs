using System;
using BulletML;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Profiles;
using XmasHell.Physics.Collision;

namespace XmasHell.Entities.Bosses.XmasSnowflake
{
    class XmasSnowflake : Boss
    {
        private ParticleEffect _destroyedParticles;

        public XmasSnowflake(XmasHell game, PositionDelegate playerPositionDelegate) :
            base(game, BossType.XmasSnowflake, playerPositionDelegate)
        {
            // BulletML
            BulletPatternFiles.Add("XmasSnowflake/pattern1");
            BulletPatternFiles.Add("XmasSnowflake/pattern2");
            BulletPatternFiles.Add("XmasSnowflake/pattern3");
            BulletPatternFiles.Add("XmasSnowflake/pattern4");

            // Behaviours
            Behaviours.Add(new XmasSnowflakeBehaviour1(this));
            Behaviours.Add(new XmasSnowflakeBehaviour2(this));
            Behaviours.Add(new XmasSnowflakeBehaviour3(this));
            Behaviours.Add(new XmasSnowflakeBehaviour4(this));

            SpriterFilename = "Graphics/Sprites/Bosses/XmasSnowflake/xmas-snowflake";

            InitializeDestroyedParticleEffect();
        }

        protected override void LoadSpriterSprite()
        {
            base.LoadSpriterSprite();
        }

        protected override void InitializePhysics(bool setupPhysicsWorld = false)
        {
            base.InitializePhysics();

            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png"));
        }

        private void InitializeDestroyedParticleEffect()
        {
            _destroyedParticles = new ParticleEffect
            {
                Name = "BossDestroyedParticles",

                Emitters = new[]
                {
                    new ParticleEmitter(
                        Game.GameManager.ParticleManager.Pixel(),
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
                            Color = new Color(0.35f, 0.78f, 0.97f).ToHsl()
                        },
                        Modifiers = new IModifier[]
                        {
                            new RotationModifier { RotationRate = -2.1f }
                        }
                    }
                }
            };
        }

        protected override void PlayExplosionAnimation()
        {
            Game.GameManager.ParticleManager.EmitParticles(_destroyedParticles, Position());
        }

        protected override void UpdateBehaviourIndex()
        {
            base.UpdateBehaviourIndex();
        }
    }
}