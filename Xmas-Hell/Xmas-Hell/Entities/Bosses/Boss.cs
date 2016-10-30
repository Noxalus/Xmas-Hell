using System;
using System.Collections.Generic;
using System.Linq;
using BulletML;
using Microsoft.Xna.Framework;
using SpriterDotNet;
using SpriterDotNet.MonoGame;
using Xmas_Hell.BulletML;
using Xmas_Hell.Physics;

namespace Xmas_Hell.Entities
{
    public abstract class Boss : IPhysicsEntity
    {
        protected XmasHell Game;
        protected Vector2 InitialPosition;
        protected float InitialLife;
        protected float Life;
        protected float Direction = 1f;

        public virtual Vector2 Position()
        {
            return CurrentAnimator.Position;
        }

        public virtual float Rotation()
        {
            return CurrentAnimator.Rotation;
        }

        public virtual Vector2 Scale()
        {
            return CurrentAnimator.Scale;
        }

        public Vector2 ActionPointPosition()
        {
            if (CurrentAnimator.FrameData != null && CurrentAnimator.FrameData.PointData.ContainsKey("action_point"))
            {
                var actionPoint = CurrentAnimator.FrameData.PointData["action_point"];
                return Position() + new Vector2(actionPoint.X, -actionPoint.Y);
            }

            return Position();
        }

        // BulletML
        protected List<BulletPattern> BossPatterns;
        protected TimeSpan BossBulletFrequence;

        // Spriter

        protected static readonly Config DefaultAnimatorConfig = new Config
        {
            MetadataEnabled = true,
            EventsEnabled = true,
            PoolingEnabled = true,
            TagsEnabled = true,
            VarsEnabled = true,
            SoundsEnabled = false
        };

        protected IList<MonoGameAnimator> Animators = new List<MonoGameAnimator>();
        protected MonoGameAnimator CurrentAnimator;

        protected Boss(XmasHell game)
        {
            Game = game;

            InitialLife = GameConfig.BossDefaultLife;
            InitialPosition = new Vector2(
                GameConfig.VirtualResolution.X / 2f,
                150f
            );
        }

        public void Initialize()
        {
            Game.GameManager.MoverManager.Clear();
            Life = InitialLife;
            CurrentAnimator.Position = InitialPosition;
        }

        public void Destroy()
        {
            Game.GameManager.ParticleManager.EmitBossDestroyedParticles(CurrentAnimator.Position);
            Initialize();
        }

        protected void CurrentAnimator_EventTriggered(string obj)
        {
            System.Diagnostics.Debug.WriteLine(obj);
        }

        protected void AddBullet(bool clear = false)
        {
            if (clear)
                Game.GameManager.MoverManager.Clear();

            // Add a new bullet in the center of the screen
            var mover = (Mover)Game.GameManager.MoverManager.CreateBullet(true);
            mover.Texture = Assets.GetTexture2D("Graphics/Sprites/bullet");
            mover.Position(ActionPointPosition());
            mover.InitTopNode(BossPatterns[0].RootNode);
        }

        public void TakeDamage(float amount)
        {
            Life -= amount;

            if (Life < 0f)
                Destroy();
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime)
        {
            CurrentAnimator.Draw(Game.SpriteBatch);

            var percent = Life / InitialLife;
            Game.SpriteBatch.Draw(
                Assets.GetTexture2D("pixel"),
                new Rectangle(0, 0, (int)(percent * GameConfig.VirtualResolution.X), 20),
                Color.Black
            );
        }
    }
}