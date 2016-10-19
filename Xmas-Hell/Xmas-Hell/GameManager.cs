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

        public List<Bullet> GetBossBullets()
        {
            return _bullets.Where(b => !(b is PlayerBullet)).ToList();
        }

        public List<Bullet> GetPlayerBullets()
        {
            return _bullets.Where(b => b is PlayerBullet).ToList();
        }

        public GameManager(XmasHell game)
        {
            _game = game;
            _bullets = new List<Bullet>();
        }

        public void Update(GameTime gameTime)
        {
            for (int index = 0; index < _bullets.Count; index++)
            {
                var bullet = _bullets[index];
                bullet.Update(gameTime);

                CheckCollision(bullet);
            }
        }

        private void CheckCollision(Bullet bullet)
        {
            // TODO: Check collision

            if (bullet.Position.X < 0 || bullet.Position.X > _game.ViewportAdapter.Viewport.Width ||
                bullet.Position.Y < 0 || bullet.Position.Y > _game.ViewportAdapter.Viewport.Height)
            {
                RemoveBullet(bullet);
            }
        }

        public void AddBullet(Bullet bullet)
        {
            _bullets.Add(bullet);
        }

        public void RemoveBullet(Bullet bullet)
        {
            _bullets.Remove(bullet);
        }

        public void Draw(GameTime gameTime)
        {
            // TODO: Apply a bloom effect on all bullets
            _game.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: _game.ViewportAdapter.GetScaleMatrix());

            foreach (var bullet in _bullets)
                bullet.Draw(gameTime);

            _game.SpriteBatch.End();
        }
    }
}