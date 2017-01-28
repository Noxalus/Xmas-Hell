using System;
using System.Collections.Generic;
using System.Linq;
using BulletML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XmasHell.BulletML;
using XmasHell.Entities;
using XmasHell.Particles;
using XmasHell.Physics;
using Bullet = XmasHell.Entities.Bullet;

namespace XmasHell
{
    public class GameManager
    {
        private XmasHell _game;
        private List<Bullet> _bullets;

        public Random Random;

        // BulletML
        public MoverManager MoverManager;
        static public FloatDelegate GameDifficulty;

        // Physics
        public readonly CollisionWorld CollisionWorld;

        // Particles
        public readonly ParticleManager ParticleManager;

        public List<Mover> GetBossBullets()
        {
            return MoverManager.Movers;
        }

        public List<Bullet> GetPlayerBullets()
        {
            return _bullets.Where(b => b is PlayerBullet).ToList();
        }

        public GameManager(XmasHell game)
        {
            _game = game;
            _bullets = new List<Bullet>();
            MoverManager = new MoverManager(_game);
            CollisionWorld = new CollisionWorld(_game);
            ParticleManager = new ParticleManager(_game);

            //Random = new Random(GameConfig.RandomSeed);
            Random = new Random();
        }

        public void Initialize()
        {
            ParticleManager.Initialize();
        }

        public void Update(GameTime gameTime)
        {
            foreach (var bullet in _bullets)
                bullet.Update(gameTime);

            MoverManager.Update();
            CollisionWorld.Update(gameTime);
            ParticleManager.Update(gameTime);

            _bullets.RemoveAll(b => !b.Used);
        }

        public void AddBullet(Bullet bullet)
        {
            _bullets.Add(bullet);
        }
    }
}