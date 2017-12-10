using System;
using BulletML;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Profiles;
using XmasHell.Physics.Collision;

namespace XmasHell.Entities.Bosses.XmasBell
{
    class XmasBell : Boss
    {
        private ParticleEffect _destroyedParticles;

        public XmasBell(XmasHell game, PositionDelegate playerPositionDelegate) :
            base(game, BossType.XmasBell, playerPositionDelegate)
        {
            // Spriter
            SpriterFilename = "Graphics/Sprites/Bosses/XmasBell/xmas-bell";

            // BulletML
            BulletPatternFiles.Add("XmasBell/pattern1");
            BulletPatternFiles.Add("XmasBell/pattern2");
            BulletPatternFiles.Add("XmasBell/pattern4");
            BulletPatternFiles.Add("XmasBell/pattern5");

            // Behaviours
            Behaviours.Add(new XmasBellBehaviour1(this));
            Behaviours.Add(new XmasBellBehaviour2(this));
            //Behaviours.Add(new XmasBellBehaviour3(this));
            Behaviours.Add(new XmasBellBehaviour4(this));
            Behaviours.Add(new XmasBellBehaviour5(this));

            InitializeDestroyedParticleEffect();
        }

        public override void Reset()
        {
            base.Reset();

            RandomMovingArea = new Rectangle(150, 225, Game.ViewportAdapter.VirtualWidth - 150, 225);
        }

        protected override void InitializePhysics(bool setupPhysicsWorld = false)
        {
            base.InitializePhysics();

            // Setup the physics
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(0f, 10f), 0.90f));
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionConvexPolygon(this, "clapper.png"));
            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionCircle(this, "clapper-ball.png"));
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
                            Color = Color.DarkGoldenrod.ToHsl()
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