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
        private int _previousBehaviourIndex;
        private int _currentBehaviourIndex;

        private readonly List<AbstractBossBehaviour> _behaviours;

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

            CurrentAnimator.AnimationFinished += delegate (string animationName)
            {
                if (animationName == "Breathe_In")
                    CurrentAnimator.Play("Big_Idle");
                else if (animationName == "Breathe_Out")
                    CurrentAnimator.Play("Idle");
            };

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

            // Behaviours
            _behaviours = new List<AbstractBossBehaviour>()
            {
                new XmasBallBehaviour1(this),
                new XmasBallBehaviour2(this)
            };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _previousBehaviourIndex = _currentBehaviourIndex;
            _currentBehaviourIndex = (int)((1f - (Life / InitialLife)) * _behaviours.Count);

            if (_currentBehaviourIndex != _previousBehaviourIndex)
                Game.GameManager.MoverManager.Clear();

            var currentBehaviour = _behaviours[_currentBehaviourIndex];
            currentBehaviour.Update(gameTime);

            CurrentAnimator.Update(gameTime.ElapsedGameTime.Milliseconds);
        }
    }
}