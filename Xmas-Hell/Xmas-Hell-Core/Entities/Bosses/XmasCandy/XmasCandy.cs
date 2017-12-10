using System;
using System.Collections.Generic;
using BulletML;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Profiles;
using XmasHell.Physics.Collision;

namespace XmasHell.Entities.Bosses.XmasCandy
{
    class XmasCandy : Boss
    {
        public XmasCandy(XmasHell game, PositionDelegate playerPositionDelegate) :
            base(game, BossType.XmasCandy, playerPositionDelegate)
        {
            // BulletML
            BulletPatternFiles.Add("XmasCandy/pattern1");
            BulletPatternFiles.Add("XmasCandy/pattern2");
            BulletPatternFiles.Add("XmasCandy/pattern3");
            BulletPatternFiles.Add("XmasCandy/pattern4");

            // Behaviours
            Behaviours.Add(new XmasCandyBehaviour1(this));
            Behaviours.Add(new XmasCandyBehaviour2(this));
            Behaviours.Add(new XmasCandyBehaviour3(this));
            Behaviours.Add(new XmasCandyBehaviour4(this));

            SpriterFilename = "Graphics/Sprites/Bosses/XmasCandy/xmas-candy";
        }

        protected override void LoadSpriterSprite()
        {
            base.LoadSpriterSprite();
            CurrentAnimator.StretchOut = false;
        }

        public override void Reset()
        {
            base.Reset();

            // For the 4th pattern, the initial bounding boxes are removed
            // We need to make sure we re-add them properly
            Game.GameManager.CollisionWorld.ClearBossHitboxes();
            InitializePhysics();

            // Same thing for stretch out behaviour, disabled for the 4th pattern
            CurrentAnimator.StretchOut = false;
        }

        protected override void InitializePhysics(bool setupPhysicsWorld = false)
        {
            base.InitializePhysics();

            // Top part
            AddHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(-75, 20), 0.3f));
            AddHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(-65, -20), 0.3f));
            AddHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(-20, -50), 0.3f));
            AddHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(20, -50), 0.3f));
            AddHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(65, -20), 0.3f));
            AddHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(75, 20), 0.3f));
            AddHitBox(new SpriterCollisionCircle(this, "body.png", new Vector2(75, 60), 0.3f));

            AddHitBox(new SpriterCollisionConvexPolygon(this, "body2.png"));
            AddHitBox(new SpriterCollisionCircle(this, "body3.png"));
        }

        protected override void PlayExplosionAnimation()
        {
            foreach (var hitbox in HitBoxes)
            {
                if (hitbox is SpriterCollisionCircle)
                {
                    var particleEffect = new ParticleEffect
                    {
                        Name = "BossDestroyedParticles",

                        Emitters = new[]
                        {
                            new ParticleEmitter(
                                Game.GameManager.ParticleManager.Pixel(),
                                500 / HitBoxes.Count,
                                TimeSpan.FromSeconds(2.5),
                                Profile.Point(),
                                false)
                            {
                                Parameters = new ParticleReleaseParameters
                                {
                                    Speed = new Range<float>(10f, 500f),
                                    Quantity = 500 / HitBoxes.Count,
                                    Rotation = new Range<float>(-1f, 1f),
                                    Scale = new Range<float>(3.0f, 6.0f),
                                    Color = Game.GameManager.Random.NextDouble() > 0.5f ? Color.White.ToHsl() : Color.Red.ToHsl()
                                },
                                Modifiers = new IModifier[]
                                {
                                    new RotationModifier { RotationRate = -2.1f }
                                }
                            }
                        }
                    };

                    Game.GameManager.ParticleManager.EmitParticles(particleEffect, ((SpriterCollisionCircle)hitbox).GetCenter());
                }
            }
        }

        protected override void UpdateBehaviourIndex()
        {
            base.UpdateBehaviourIndex();
        }
    }
}