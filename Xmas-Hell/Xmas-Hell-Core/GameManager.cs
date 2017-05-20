using System;
using System.Collections.Generic;
using System.Linq;
using BulletML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Timers;
using XmasHell.BulletML;
using XmasHell.Entities;
using XmasHell.Particles;
using XmasHell.Physics;
using XmasHell.Screens;
using Bullet = XmasHell.Entities.Bullet;

namespace XmasHell
{
    public class GameManager
    {
        private XmasHell _game;
        private List<Bullet> _bullets;
        private List<Laser> _lasers;

        private bool _endGame;
        private CountdownTimer _endGameTimer;
        private bool _endGameFirstTime;

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

        public bool EndGame()
        {
            return _endGame;
        }

        public void EndGame(bool value)
        {
            _endGame = value;
            _endGameTimer.Restart();
        }

        public GameManager(XmasHell game)
        {
            _game = game;
            _bullets = new List<Bullet>();
            _lasers = new List<Laser>();
            _endGame = false;

            MoverManager = new MoverManager(_game);
            CollisionWorld = new CollisionWorld(_game);
            ParticleManager = new ParticleManager(_game);

            //Random = new Random(GameConfig.RandomSeed);
            Random = new Random();

            _endGameTimer = new CountdownTimer(1);
            _endGameTimer.Stop();
            _endGameTimer.Completed += EndGameTimerCompleted;
            _endGameFirstTime = true;
        }

        private void EndGameTimerCompleted(object sender, EventArgs e)
        {
            if (_endGameFirstTime)
            {
                _endGame = false;
                _endGameFirstTime = false;
                _endGameTimer.Restart();
            }
            else
            {
                _endGameFirstTime = true;
                //_game.GameScreen.Dispose();
                //_game.GameScreen.Show<MainMenuScreen>();
                _endGameTimer.Stop();
                _game.Camera.Zoom = 1f;
            }
        }

        public void Initialize()
        {
            ParticleManager.Initialize();
        }

        public void Update(GameTime gameTime)
        {
            _endGameTimer.Update(gameTime);

            if (_endGame)
                return;

            foreach (var bullet in _bullets)
                bullet.Update(gameTime);

            foreach (var laser in _lasers)
                laser.Update(gameTime);

            MoverManager.Update();
            CollisionWorld.Update(gameTime);
            ParticleManager.Update(gameTime);

            _bullets.RemoveAll(b => !b.Used);
        }

        public void AddBullet(Bullet bullet)
        {
            _bullets.Add(bullet);
        }

        public void AddLaser(Laser laser)
        {
            _lasers.Add(laser);
            _game.SpriteBatchManager.Lasers.Add(laser);
        }

        public void RemoveLaser(Laser laser)
        {
            _lasers.Remove(laser);
            _game.SpriteBatchManager.Lasers.Remove(laser);
        }

        public Vector2 GetRandomPosition(bool normalized = false)
        {
            float randomX = Random.Next(0, GameConfig.VirtualResolution.X);
            float randomY = Random.Next(0, GameConfig.VirtualResolution.Y);

            if (normalized)
            {
                randomX /= (float)GameConfig.VirtualResolution.X;
                randomY /= (float)GameConfig.VirtualResolution.Y;
            }

            return new Vector2(randomX, randomY);
        }
    }
}