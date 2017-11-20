using System;
using FarseerPhysics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;

namespace XmasHell.Entities.Bosses.XmasGift
{
    class XmasGiftBehaviour3 : AbstractBossBehaviour
    {
        private List<Gift> _gifts = new List<Gift>();
        private TimeSpan _spawnGiftTimer;

        public XmasGiftBehaviour3(Boss boss) : base(boss)
        {
        }

        public override void Start()
        {
            base.Start();

            Boss.PhysicsBody.Position = ConvertUnits.ToSimUnits(Boss.Position());
            Boss.PhysicsWorld.Gravity = GameConfig.DefaultGravity;
            Boss.PhysicsEnabled = true;

            Boss.PhysicsBody.IgnoreGravity = true;
            Boss.PhysicsBody.BodyType = BodyType.Static;
            Boss.PhysicsBody.CollidesWith = Category.None;

            Boss.CurrentAnimator.Play("Idle");

            ResetGiftTimer();
            Boss.MoveToInitialPosition();
        }

        public override void Stop()
        {
            base.Stop();

            _gifts.Clear();
            Boss.PhysicsWorld.Gravity = Vector2.Zero;
            Boss.PhysicsEnabled = false;
        }

        private void ResetGiftTimer()
        {
            _spawnGiftTimer = TimeSpan.FromSeconds(Boss.Game.GameManager.Random.Next(1, 1));
        }

        public override void Update(GameTime gameTime)
        {
            if (!Boss.TargetingPosition)
            {
                if (_spawnGiftTimer.TotalSeconds < 0)
                {
                    _gifts.Add(new Gift((XmasGift) Boss, Boss.CurrentAnimator));
                    ResetGiftTimer();
                }
                else
                    _spawnGiftTimer -= gameTime.ElapsedGameTime;

                foreach (var gift in _gifts)
                    gift.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void DrawAfter(SpriteBatch spriteBatch)
        {
            base.DrawAfter(spriteBatch);

            foreach (var gift in _gifts)
                gift.Draw(spriteBatch);
        }
    }
}