using System.Collections.Generic;
using Java.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xmas_Hell.BulletML;
using Xmas_Hell.Entities;
using Xmas_Hell.Entities.Bosses;
using Xmas_Hell.Physics.Collision;

namespace Xmas_Hell.Physics
{
    public class CollisionWorld
    {
        private XmasHell _game;
        private CollisionCircle _playerHitbox;
        private CollisionElement _bossHitbox;
        private readonly List<CollisionElement> _playerBulletHitboxes = new List<CollisionElement>();
        private readonly List<CollisionElement> _bossBulletHitboxes = new List<CollisionElement>();

        public CollisionCircle PlayerHitbox
        {
            get
            {
                return _playerHitbox;
            }

            set
            {
                _playerHitbox = value;
                _game.SpriteBatchManager.DebugCollisionElements.Add(_playerHitbox);

            }
        }

        public CollisionElement BossHitbox
        {
            get
            {
                return _bossHitbox;
            }

            set
            {
                _bossHitbox = value;
                _game.SpriteBatchManager.DebugCollisionElements.Add(_bossHitbox);
            }
        }

        public void AddPlayerBulletHitbox(CollisionElement element)
        {
            _playerBulletHitboxes.Add(element);
            _game.SpriteBatchManager.DebugCollisionElements.Add(element);
        }

        public void AddBossBulletHitbox(CollisionElement element)
        {
            _bossBulletHitboxes.Add(element);
            _game.SpriteBatchManager.DebugCollisionElements.Add(element);
        }

        public void RemovePlayerBulletHitbox(CollisionElement element)
        {
            _playerBulletHitboxes.Remove(element);
            _game.SpriteBatchManager.DebugCollisionElements.Remove(element);
        }

        public void RemoveBossBulletHitbox(CollisionElement element)
        {
            _bossBulletHitboxes.Remove(element);
            _game.SpriteBatchManager.DebugCollisionElements.Remove(element);
        }

        public CollisionWorld(XmasHell game)
        {
            _game = game;
        }

        public void Update(GameTime gameTime)
        {
            // Check collision between player's bullets and boss hitbox
            if (BossHitbox != null)
            {
                for (int index = 0; index < _playerBulletHitboxes.Count; index++)
                {
                    var playerBulletHitbox = _playerBulletHitboxes[index];
                    if (playerBulletHitbox.Intersects(BossHitbox))
                    {
                        var playerBullet = (Bullet) playerBulletHitbox.Entity;
                        playerBullet.Destroy();
                        ((Boss) BossHitbox.Entity).TakeDamage(1);
                        _game.GameManager.ParticleManager.EmitBossHitParticles(playerBullet.Position());
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
            var player = (Player) PlayerHitbox?.Entity;

            if (player != null && !player.Invincible)
            {
                for (int index = 0; index < _bossBulletHitboxes.Count; index++)
                {
                    var bossBulletHitbox = _bossBulletHitboxes[index];
                    if (PlayerHitbox.Intersects(bossBulletHitbox))
                    {
                        player.Destroy();
                        ((Mover) bossBulletHitbox.Entity).Used = false;
                    }
                }
            }

            // Clean destroyed elements
            _playerBulletHitboxes.RemoveAll(hb => !((Bullet) hb.Entity).Used);
            _bossBulletHitboxes.RemoveAll(hb => !((Mover)hb.Entity).Used);
        }
    }
}