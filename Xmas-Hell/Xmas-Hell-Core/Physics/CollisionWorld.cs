using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XmasHell.BulletML;
using XmasHell.Entities;
using XmasHell.Entities.Bosses;
using XmasHell.Physics.Collision;

namespace XmasHell.Physics
{
    public class CollisionWorld
    {
        private XmasHell _game;
        private CollisionCircle _playerHitbox;
        private List<CollisionElement> BossHitboxes = new List<CollisionElement>();
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

            }
        }

        public void AddBossHitBox(CollisionElement element)
        {
            BossHitboxes.Add(element);
        }

        public void AddPlayerBulletHitbox(CollisionElement element)
        {
            _playerBulletHitboxes.Add(element);
        }

        public void AddBossBulletHitbox(CollisionElement element)
        {
            _bossBulletHitboxes.Add(element);
        }

        public void RemoveBossHitBox(CollisionElement element)
        {
            BossHitboxes.Remove(element);
        }

        public void ClearBossHitboxes()
        {
            BossHitboxes.Clear();
        }

        public void ClearBossBullets()
        {
            _bossBulletHitboxes.Clear();
        }

        public void RemovePlayerBulletHitbox(CollisionElement element)
        {
            _playerBulletHitboxes.Remove(element);
        }

        public void RemoveBossBulletHitbox(CollisionElement element)
        {
            _bossBulletHitboxes.Remove(element);
        }

        public CollisionWorld(XmasHell game)
        {
            _game = game;
        }

        public void Update(GameTime gameTime)
        {
            // Check collision between player's bullets and boss hitbox
            if (BossHitboxes != null)
            {
                for (int index = 0; index < _playerBulletHitboxes.Count; index++)
                {
                    var playerBulletHitbox = _playerBulletHitboxes[index];

                    foreach (var bossHitBox in BossHitboxes)
                    {
                        if (playerBulletHitbox.Intersects(bossHitBox))
                        {
                            var playerBullet = (Bullet) playerBulletHitbox.Entity;
                            playerBullet.Destroy();
                            bossHitBox.Entity.TakeDamage(1);
                            _game.GameManager.ParticleManager.EmitBossHitParticles(playerBullet.Position());
                        }
                    }
                }
            }

            // Check collision between player's hitbox and boss hitbox
            if (PlayerHitbox != null && BossHitboxes != null)
            {
                foreach (var bossHitBox in BossHitboxes)
                {
                    if (PlayerHitbox.Intersects(bossHitBox))
                    {
                        ((Player) PlayerHitbox.Entity).Destroy();
                    }
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
            _playerBulletHitboxes.RemoveAll(hb => !((Bullet)hb.Entity).Used);
            _bossBulletHitboxes.RemoveAll(hb => !((Mover)hb.Entity).Used);
        }

        // For Debug purpose only
        public void Draw()
        {
            _game.SpriteBatch.Begin(
                samplerState: SamplerState.PointClamp,
                blendState: BlendState.AlphaBlend,
                transformMatrix: _game.Camera.GetViewMatrix()
            );

            // Draw hitboxes
            _playerHitbox.Draw(_game.SpriteBatch);
            BossHitboxes.ForEach(hb => hb.Draw(_game.SpriteBatch));

            // Draw bullet's colliders
            _playerBulletHitboxes.ForEach(hb => hb.Draw(_game.SpriteBatch));
            _bossBulletHitboxes.ForEach(hb => hb.Draw(_game.SpriteBatch));

            _game.SpriteBatch.End();

        }
    }
}