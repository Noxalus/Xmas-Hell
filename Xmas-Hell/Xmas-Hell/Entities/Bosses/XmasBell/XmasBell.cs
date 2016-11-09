using BulletML;
using Microsoft.Xna.Framework;
using XmasHell.Physics.Collision;

namespace XmasHell.Entities.Bosses.XmasBell
{
    class XmasBell : Boss
    {
        public XmasBell(XmasHell game, PositionDelegate playerPositionDelegate) : base(game, playerPositionDelegate)
        {
            // Spriter
            SpriterFilename = "Graphics/Sprites/Bosses/XmasBell/xmas-bell";

            // BulletML
            BulletPatternFiles.Add("XmasBell/pattern1");
            BulletPatternFiles.Add("XmasBell/pattern2");

            // Behaviours
            //Behaviours.Add(new XmasBellBehaviour1(this));
            //Behaviours.Add(new XmasBellBehaviour2(this));
            //Behaviours.Add(new XmasBellBehaviour3(this));
            Behaviours.Add(new XmasBellBehaviour4(this));

            // Physics
            //Game.GameManager.CollisionWorld.BossHitbox = new CollisionCircle(this, new Vector2(0f, 80f), 80f);
            Game.GameManager.CollisionWorld.BossHitbox = new CollisionCircle(this, Vector2.Zero, 80f);
        }
    }
}