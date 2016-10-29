using System.Collections.Generic;
using Java.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xmas_Hell.BulletML;
using Xmas_Hell.Entities;
using Xmas_Hell.Physics.Collision;

namespace Xmas_Hell.Physics
{
    public class CollisionWorld
    {
        public CollisionCircle PlayerHitbox;
        public CollisionElement BossHitbox;
        public readonly List<CollisionCircle> PlayerBulletHitboxes = new List<CollisionCircle>();
        public readonly List<CollisionElement> BossBulletHitboxes = new List<CollisionElement>();

        public CollisionWorld()
        {
        }

        public void Update(GameTime gameTime)
        {
            // Check collision between player's bullets and boss hitbox
            if (BossHitbox != null)
            {
                foreach (var playerBulletHitbox in PlayerBulletHitboxes)
                {
                    if (playerBulletHitbox.Intersects(BossHitbox))
                    {
                        ((Bullet) playerBulletHitbox.Entity).Destroy();
                        ((Boss) BossHitbox.Entity).TakeDamage(1);
                    }
                }
            }

            // Check collision between player's hitbox and boss hitbox
            if (PlayerHitbox != null && BossHitbox != null)
            {
                if (PlayerHitbox.Intersects(BossHitbox))
                {
                    ((Player) PlayerHitbox.Entity).Destroy();
                }
            }

            // Check collision between boss bullets and player's hitbox
            if (PlayerHitbox != null)
            {
                foreach (var bossBulletHitbox in BossBulletHitboxes)
                {
                    if (PlayerHitbox.Intersects(bossBulletHitbox))
                    {
                        ((Player) PlayerHitbox.Entity).Destroy();
                        ((Mover) bossBulletHitbox.Entity).Used = false;
                    }
                }
            }

            // Clean destroyed elements
            PlayerBulletHitboxes.RemoveAll(hb => !((Bullet) hb.Entity).Used);
            BossBulletHitboxes.RemoveAll(hb => !((Mover)hb.Entity).Used);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            PlayerHitbox?.Draw(spriteBatch);
            BossHitbox?.Draw(spriteBatch);

            foreach (var playerBulletHitbox in PlayerBulletHitboxes)
                playerBulletHitbox.Draw(spriteBatch);

            foreach (var bossBulletHitbox in BossBulletHitboxes)
                bossBulletHitbox.Draw(spriteBatch);
        }
    }
}