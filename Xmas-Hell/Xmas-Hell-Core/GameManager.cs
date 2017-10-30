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
using Bullet = XmasHell.Entities.Bullet;
using XmasHell.Sound;

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
        private bool _gameIsFinished;

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

        public bool GameIsFinished()
        {
            return _gameIsFinished;
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

        public void Reset()
        {
            Clear();
            _gameIsFinished = false;
        }

        public void Clear()
        {
            MoverManager.Clear();

            foreach(var bullet in _bullets)
                bullet.Destroy();

            _bullets.RemoveAll(b => !b.Used);
        }

        private void EndGameTimerCompleted(object sender, EventArgs e)
        {
            if (_endGameFirstTime)
            {
                _endGame = false;
                _endGameFirstTime = false;
                _endGameTimer.Restart();
                SoundManager.PlaySound(Assets.GetSound("Audio/SE/player-death"));
            }
            else
            {
                _endGameFirstTime = true;
                _gameIsFinished = true;

                _endGameTimer.Stop();
                _game.Camera.Zoom = 1f;
            }
        }

        public void Initialize()
        {
            ParticleManager.Initialize();

            var bulletTextures = new List<Texture2D>() {
                Assets.GetTexture2D("Graphics/Sprites/Bullets/bullet1"),
                Assets.GetTexture2D("Graphics/Sprites/Bullets/bullet2"),
                Assets.GetTexture2D("Graphics/Sprites/Bullets/bullet3"),
                Assets.GetTexture2D("Graphics/Sprites/Bullets/bullet4")
            };

            MoverManager.BulletTextures = bulletTextures;
        }

        public void StartNewGame()
        {
            _gameIsFinished = false;

            // TODO: Reset game status (including player and boss)
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

        public Vector2 GetRandomPosition(bool normalized = false, Rectangle? bounds = null)
        {
            float randomX;
            float randomY;

            if (bounds.HasValue)
            {
                randomX = Random.Next(bounds.Value.X, bounds.Value.X + bounds.Value.Width);
                randomY = Random.Next(bounds.Value.Y, bounds.Value.Y + bounds.Value.Height);
            }
            else
            {
                randomX = Random.Next(0, GameConfig.VirtualResolution.X);
                randomY = Random.Next(0, GameConfig.VirtualResolution.Y);
            }

            if (normalized)
            {
                randomX /= (float)GameConfig.VirtualResolution.X;
                randomY /= (float)GameConfig.VirtualResolution.Y;
            }

            return new Vector2(randomX, randomY);
        }

        public bool IsOutside(Vector2 position)
        {
            return position.X < 0 || position.X > _game.ViewportAdapter.VirtualWidth ||
                   position.Y < 0 || position.Y > _game.ViewportAdapter.VirtualHeight;
        }
    }
}