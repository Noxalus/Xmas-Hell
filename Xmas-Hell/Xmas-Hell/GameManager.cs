using System.Collections.Generic;
using System.Linq;
using BulletML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xmas_Hell.BulletML;
using Xmas_Hell.Entities;
using Bullet = Xmas_Hell.Entities.Bullet;

namespace Xmas_Hell
{
    public class GameManager
    {
        private XmasHell _game;
        private List<Bullet> _bullets;

        public MoverManager MoverManager;
        static public FloatDelegate GameDifficulty;

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
            MoverManager = new MoverManager();
        }

        public void Update(GameTime gameTime)
        {
            for (int index = 0; index < _bullets.Count; index++)
            {
                var bullet = _bullets[index];
                bullet.Update(gameTime);

                CheckCollision(bullet);
            }

            MoverManager.Update();
        }

        private void CheckCollision(Bullet bullet)
        {
            // TODO: Check collision

            if (bullet.Position.X < 0 || bullet.Position.X > GameConfig.VirtualResolution.X ||
                bullet.Position.Y < 0 || bullet.Position.Y > GameConfig.VirtualResolution.Y)
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
            _game.SpriteBatch.Begin(
                samplerState: SamplerState.PointClamp,
                sortMode: SpriteSortMode.Immediate,
                blendState: BlendState.AlphaBlend,
                transformMatrix: _game.ViewportAdapter.GetScaleMatrix());

            foreach (var bullet in _bullets)
                bullet.Draw(gameTime);

            foreach (var mover in MoverManager.Movers)
            {
                _game.SpriteBatch.Draw(Assets.GetTexture2D("Graphics/Sprites/bullet"), mover.Position, Color.White);
            }

            _game.SpriteBatch.End();
        }
    }
}