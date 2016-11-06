using BulletML;
using Microsoft.Xna.Framework;
using Xmas_Hell.Physics.Collision;

namespace Xmas_Hell.Entities.Bosses.XmasBell
{
    class XmasBell : Boss
    {
        public XmasBell(XmasHell game, PositionDelegate playerPositionDelegate) : base(game, playerPositionDelegate)
        {
            // Spriter
            SpriterFilename = "Graphics/Sprites/Bosses/XmasBell/xmas-bell";

            // BulletML
            BulletPatternFiles.Add("XmasBell/pattern1");

            // Behaviours
            Behaviours.Add(new XmasBellBehaviour1(this));

            // Physics
            Game.GameManager.CollisionWorld.BossHitbox = new CollisionCircle(this, Vector2.Zero, 150f);
        }
    }
}