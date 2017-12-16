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
        private CountdownTimer _cameraZoomTimer;
        private CountdownTimer _explosionTimer;
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

        public void EndGame(bool value, bool won, float zoomTimer = 0f)
        {
            _endGame = value;
            _won = won;

            _cameraZoomTimer.Interval = TimeSpan.FromSeconds(zoomTimer);
            _cameraZoomTimer.Restart();
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

            _cameraZoomTimer = new CountdownTimer(GameConfig.EndGameCameraZoomTime);
            _cameraZoomTimer.Stop();
            _cameraZoomTimer.Completed += CameraZoomTimerCompleted;

            _explosionTimer = new CountdownTimer(GameConfig.EndGameExplosionTime);
            _explosionTimer.Stop();
            _explosionTimer.Completed += ExplosionTimerCompleted;

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
                Assets.GetTexture2D("Graphics/Sprites/Bullets/bullet4"),
                Assets.GetTexture2D("Graphics/Sprites/Bullets/carrot")
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
            _cameraZoomTimer.Stop();
            _explosionTimer.Stop();
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
            _endGame = false;
            _gameIsFinished = false;
            _transitioningToEndGame = false;
            _won = false;
            _cantMove = false;

            _player.Reset();
            _boss?.Reset();

            _ready = true;

            ParticleManager.Clear();
        }

        private void ExplosionTimerCompleted(object sender, EventArgs e)
        {
            _gameIsFinished = true;
            _cantMove = true;

            _game.PlayerData.BossPlayTime(_boss.BossType, _game.PlayerData.BossPlayTime(_boss.BossType) + _playTime);

            _cameraZoomTimer.Stop();
            _game.Camera.Zoom = 1f;
        }

        private void CameraZoomTimerCompleted(object sender, EventArgs e)
        {
            _explosionTimer.Restart();
            SoundManager.PlaySound(Assets.GetSound("Audio/SE/player-death"));
            _transitioningToEndGame = true;
        }

        public void LoadBoss(BossType bossType)
        {
            _boss = BossFactory.CreateBoss(bossType, _game, _player.Position);
        }

        public void Update(GameTime gameTime)
        {
            if (!EndGame() || (GameIsFinished() || TransitioningToEndGame()))
                MoverManager.Update();

            if (!_ready)
                return;

            _cameraZoomTimer.Update(gameTime);
            _explosionTimer.Update(gameTime);

            if (_boss != null && (!EndGame() || (GameIsFinished() || TransitioningToEndGame())))
                _boss.Update(gameTime);

            if (!EndGame() || (GameIsFinished() || TransitioningToEndGame()))
            {
                foreach (var bullet in _bullets)
                    bullet.Update(gameTime);

                _bullets.RemoveAll(b => !b.Used);

                foreach (var laser in _lasers)
                    laser.Update(gameTime);

                ParticleManager.Update(gameTime);
            }

            if (GameIsFinished())
                return;

            _playTime += gameTime.ElapsedGameTime;

            if (_boss != null && _boss.IsReady() && !EndGame())
                _timer += gameTime.ElapsedGameTime;

            CollisionWorld.Update(gameTime);

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