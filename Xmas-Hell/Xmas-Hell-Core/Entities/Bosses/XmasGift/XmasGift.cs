using BulletML;
using Microsoft.Xna.Framework;
using XmasHell.Physics.Collision;

namespace XmasHell.Entities.Bosses.XmasGift
{
    class XmasGift : Boss
    {
        public XmasGift(XmasHell game, PositionDelegate playerPositionDelegate) : base(game, playerPositionDelegate)
        {
            // Spriter
            SpriterFilename = "Graphics/Sprites/Bosses/XmasGift/xmas-gift";

            // BulletML
            BulletPatternFiles.Add("sample");

            // Behaviours
            Behaviours.Add(new XmasGiftBehaviour1(this));

            // Physics
            Game.GameManager.CollisionWorld.BossHitbox = new SpriterCollisionCircle(this, "body.png", new Vector2(0f, 10f), 0.90f);
        }
    }
}