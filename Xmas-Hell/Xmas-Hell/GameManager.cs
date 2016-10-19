using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xmas_Hell.Entities;

namespace Xmas_Hell
{
    public class GameManager
    {
        private XmasHell _game;
        private List<Bullet> _bullets;
        private List<PlayerBullet> _playerBullets;

        public GameManager(XmasHell game)
        {
            _game = game;
            _bullets = new List<Bullet>();
            _playerBullets = new List<PlayerBullet>();
        }

        public void Update(GameTime gameTime)
        {
            foreach (var bullet in _bullets)
                bullet.Update(gameTime);

            foreach (var bullet in _playerBullets)
                bullet.Update(gameTime);

            // TODO: Check collision
        }

        public void AddBullet(Bullet bullet)
        {
            _bullets.Add(bullet);
        }

        public void AddPlayerBullet(PlayerBullet bullet)
        {
            _playerBullets.Add(bullet);
        }

        public void Draw(GameTime gameTime)
        {
            // TODO: Apply a bloom effect on all bullets
            _game.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: _game.ViewportAdapter.GetScaleMatrix());

            foreach (var bullet in _bullets)
                bullet.Draw(gameTime);

            foreach (var bullet in _playerBullets)
                bullet.Draw(gameTime);

            _game.SpriteBatch.End();
        }
    }
}