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

namespace Xmas_Hell.Entities.Bosses.XmasBall
{
    class XmasBall : Boss
    {
        public XmasBall(XmasHell game, PositionDelegate playerPositionDelegate) : base(game, playerPositionDelegate)
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

            CurrentAnimator.AnimationFinished += delegate (string animationName)
            {
                if (animationName == "Breathe_In")
                    CurrentAnimator.Play("Big_Idle");
                else if (animationName == "Breathe_Out")
                    CurrentAnimator.Play("Idle");
            };

            // BulletML
            BulletPatternFiles.Add("sample");
            BulletPatternFiles.Add("XmasBall/pattern1");
            BulletPatternFiles.Add("XmasBall/pattern3");
            BulletPatternFiles.Add("XmasBall/pattern4");

            // Physics
            Game.GameManager.CollisionWorld.BossHitbox = new CollisionCircle(this, Vector2.Zero, 86f);

            // Behaviours
            Behaviours.Add(new XmasBallBehaviour1(this));
            Behaviours.Add(new XmasBallBehaviour2(this));
            Behaviours.Add(new XmasBallBehaviour3(this));
            Behaviours.Add(new XmasBallBehaviour4(this));
        }

        protected override void Reset()
        {
            base.Reset();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void UpdateBehaviourIndex()
        {
            base.UpdateBehaviourIndex();
            //CurrentBehaviourIndex = 3;
        }
    }
}