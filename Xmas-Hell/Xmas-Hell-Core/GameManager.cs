using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using XmasHell.Entities.Bosses;
using XmasHell.Audio;

namespace XmasHell
{
    public class GameManager
    {
        private XmasHell _game;

        private Boss _boss;
        private Player _player;

        private bool _ready;

        private List<Bullet> _bullets;
        private List<Laser> _lasers;

        private TimeSpan _timer;

        private bool _endGame;
        private CountdownTimer _endGameTimer;
        private bool _endGameFirstTime;
        private bool _transitioningToEndGame;
        private bool _gameIsFinished;
        private bool _won;
        private bool _cantMove;
        private TimeSpan _playTime;

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

        public Boss GetCurrentBoss()
        {
            return _boss;
        }

        public TimeSpan GetCurrentTime()
        {
            return _timer;
        }

        public List<Bullet> GetPlayerBullets()
        {
            return _bullets.Where(b => b is PlayerBullet).ToList();
        }

        public bool GameIsFinished()
        {
            return _gameIsFinished;
        }

        public bool EndGame()
        {
            return _endGame;
        }

        public bool CantMove()
        {
            return _cantMove;
        }

        public bool Won()
        {
            return _won;
        }

        public void EndGame(bool value, bool won)
        {
            _endGame = value;
            _endGameTimer.Restart();
            _endGameFirstTime = true;
            _won = won;
            _cantMove = true;
        }

        public bool TransitioningToEndGame()
        {
            return _transitioningToEndGame;
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
            _transitioningToEndGame = false;
            _cantMove = false;
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

            _player = new Player(_game);
        }

        public void StartNewGame()
        {
            _boss.Initialize();
            _player.Initialize();

            Reset();
        }

        public void Dispose()
        {
            _game.PlayerData.BossPlayTime(_boss.BossType, _game.PlayerData.BossPlayTime(_boss.BossType) + _playTime);

            MoverManager.Clear();
            ParticleManager.Clear();

            foreach (var bullet in _bullets)
                bullet.Destroy();

            _bullets.RemoveAll(b => !b.Used);

            _boss.Dispose();
            _player.Dispose();

            // Should be loaded
            _boss = null;

            _endGame = false;
            _endGameFirstTime = true;
            _endGameTimer.Stop();
            _gameIsFinished = false;
            _ready = false;
            _game.Camera.Zoom = 1f;
            _transitioningToEndGame = false;
            _cantMove = false;
        }

        public void Reset()
        {
            _playTime = TimeSpan.Zero;
            _timer = TimeSpan.Zero;
            _gameIsFinished = false;
            _transitioningToEndGame = false;
            _won = false;
            _cantMove = false;

            _player.Reset();

            if (_boss != null)
            {
                _boss.Reset();
                _game.PlayerData.BossAttempts(_boss.BossType, _game.PlayerData.BossAttempts(_boss.BossType) + 1);
            }

            _ready = true;
        }

        private void EndGameTimerCompleted(object sender, EventArgs e)
        {
            if (_endGameFirstTime)
            {
                _endGame = false;
                _endGameFirstTime = false;
                _endGameTimer.Restart();
                SoundManager.PlaySound(Assets.GetSound("Audio/SE/player-death"));
                _transitioningToEndGame = true;
            }
            else
            {
                _endGameFirstTime = true;
                _gameIsFinished = true;

                _game.PlayerData.BossPlayTime(_boss.BossType, _game.PlayerData.BossPlayTime(_boss.BossType) + _playTime);

                _endGameTimer.Stop();
                _game.Camera.Zoom = 1f;
            }
        }

        public void LoadBoss(BossType bossType)
        {
            _boss = BossFactory.CreateBoss(bossType, _game, _player.Position);
        }

        public void Update(GameTime gameTime)
        {
            if (!_endGame || GameIsFinished())
                MoverManager.Update();

            if (!_ready)
                return;

            _endGameTimer.Update(gameTime);

            if (_boss != null && (!_endGame || GameIsFinished()))
                _boss.Update(gameTime);

            if (!_endGame || GameIsFinished())
            {
                foreach (var bullet in _bullets)
                    bullet.Update(gameTime);

                _bullets.RemoveAll(b => !b.Used);

                foreach (var laser in _lasers)
                    laser.Update(gameTime);
            }

            if (GameIsFinished())
                return;

            _playTime += gameTime.ElapsedGameTime;

            if (_boss != null && _boss.IsReady() && !GameIsFinished() && _endGameFirstTime)
                _timer += gameTime.ElapsedGameTime;

            CollisionWorld.Update(gameTime);
            ParticleManager.Update(gameTime);

            _player.Update(gameTime);
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