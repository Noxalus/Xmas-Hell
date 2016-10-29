using System;
using System.Collections.Generic;
using BulletML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using Xmas_Hell.BulletML;
using Xmas_Hell.Entities;

namespace Xmas_Hell.Screens
{
    class GameScreen : Screen
    {
        private readonly XmasHell _game;
        private Player _player;
        private Boss _boss;
        private MoverManager _moverManager;
        private Mover _mover;
        private List<BulletPattern> _bossPatterns;

        private TimeSpan _bossBulletFrequence;

        private float GetRank()
        {
            return 1f;
        }

        public GameScreen(XmasHell game)
        {
            _game = game;
            _bossPatterns = new List<BulletPattern>();
        }

        public override void Initialize()
        {
            _bossBulletFrequence = TimeSpan.Zero;

            _player = new Player(_game);
            var bossPosition = new Vector2(
                GameConfig.VirtualResolution.X / 2f,
                150f
            );
            _boss = new Boss(_game, bossPosition, 100);

            base.Initialize();

            _moverManager = new MoverManager(_player.Position);

            // Load the pattern
            var pattern = new BulletPattern();
            _bossPatterns.Add(pattern);

            var filename = "sample";
            pattern.ParseStream(filename, Assets.GetPattern(filename));

            GameManager.GameDifficulty = GetRank;

            AddBullet(true);
        }

        private void AddBullet(bool clear = false)
        {
            if (clear)
                _moverManager.Clear();

            // Add a new bullet in the center of the screen
            _mover = (Mover)_moverManager.CreateBullet(true);
            _mover.Position = _boss.ActionPointPosition();
            _mover.InitTopNode(_bossPatterns[0].RootNode);
        }

        public override void Update(GameTime gameTime)
        {
            if (_bossBulletFrequence.TotalMilliseconds > 0)
                _bossBulletFrequence -= gameTime.ElapsedGameTime;
            else
            {
                _bossBulletFrequence = TimeSpan.FromTicks(GameConfig.PlayerShootFrequency.Ticks);
                AddBullet(false);
            }

            _player.Update(gameTime);
            _boss.Update(gameTime);

            _moverManager.Update();
        }

        public override void Draw(GameTime gameTime)
        {
            _game.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: _game.ViewportAdapter.GetScaleMatrix());

            _player.Draw(gameTime);
            _boss.Draw(gameTime);

            foreach (var mover in _moverManager.Movers)
            {
                _game.SpriteBatch.Draw(Assets.GetTexture2D("Graphics/Sprites/bullet"), mover.Position, Color.White);
            }

            _game.SpriteBatch.End();
        }
    }
}