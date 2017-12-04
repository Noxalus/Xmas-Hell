using System.Linq;
using BulletML;
using Microsoft.Xna.Framework;
using XmasHell.Physics.Collision;
using XmasHell.Spriter;

namespace XmasHell.Entities.Bosses.XmasLog
{
    class XmasLog : Boss
    {
        public CustomSpriterAnimator BabyLogAnimator;

        public XmasLog(XmasHell game, PositionDelegate playerPositionDelegate) :
            base(game, BossType.XmasLog, playerPositionDelegate)
        {
            // BulletML
            BulletPatternFiles.Add("XmasLog/pattern1");

            // Spriter
            SpriterFilename = "Graphics/Sprites/Bosses/XmasLog/xmas-log";

            // Behaviours
            Behaviours.Add(new XmasLogBehaviour1(this));
            Behaviours.Add(new XmasLogBehaviour2(this));
            Behaviours.Add(new XmasLogBehaviour3(this));

            InitialPosition = new Vector2(GameConfig.VirtualResolution.X / 2f, GameConfig.VirtualResolution.Y * 0.15f);
        }

        public override void Reset()
        {
            base.Reset();

            Speed = GameConfig.BossDefaultSpeed * 3f;
        }

        protected override void LoadSpriterSprite()
        {
            base.LoadSpriterSprite();

            BabyLogAnimator = Animators.First(a => a.Entity != null && a.Entity.Name == "BabyLog");
        }

        protected override void InitializePhysics(bool setupPhysicsWorld = false)
        {
            base.InitializePhysics(setupPhysicsWorld);

            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionConvexPolygon(this, "body.png"));
        }

        protected override void UpdateBehaviourIndex()
        {
            base.UpdateBehaviourIndex();

            CurrentBehaviourIndex = 2;
        }
    }
}