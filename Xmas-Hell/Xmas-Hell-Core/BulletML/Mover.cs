﻿using BulletML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using XmasHell.Entities.Bosses;
using XmasHell.Physics;
using XmasHell.Physics.Collision;

namespace XmasHell.BulletML
{
    public class Mover : Bullet, IPhysicsEntity, IBossDeadlyEntity
    {
        private XmasHell _game;
        private Vector2 _position;
        public bool TopBullet;
        public Texture2D Texture;
        public Sprite Sprite;
        private short _currentSpriteIndex;

        private CollisionElement _hitbox;
        private Vector2 _origin;
        private bool _used;

        public Vector2 Position()
        {
            return _position;
        }

        public virtual Vector2 LocalPosition()
        {
            return Vector2.Zero;
        }

        public float Rotation()
        {
            return Direction;
        }

        public Vector2 Origin()
        {
            return Vector2.Zero;
        }

        public Vector2 Scale()
        {
            // TODO: Get the proper scale
            return Vector2.One;
        }

        public void Position(Vector2 value)
        {
            _position = value;
        }

        public override float X
        {
            get { return Position().X; }
            set { _position.X = value; }
        }

        public override float Y
        {
            get { return Position().Y; }
            set { _position.Y = value; }
        }

        public bool Used()
        {
            return _used;
        }

        public void Used(bool value)
        {
            _used = value;
        }

        public Mover(XmasHell game, IBulletManager bulletManager, bool topBullet = false) : base(bulletManager)
        {
            _currentSpriteIndex = SpriteIndex;
            _game = game;
            TopBullet = topBullet;
        }

        public void Init(bool topBullet)
        {
            _used = true;

            Sprite = new Sprite(Assets.GetTexture2D("Graphics/Sprites/Bullets/bullet1"));
            Sprite.Alpha = 0f;
            Sprite.Scale = new Vector2(2.5f);

            _game.SpriteBatchManager.BossBullets.Add(this);

            if (!topBullet)
            {
                _hitbox = new CollisionCircle(this, Vector2.Zero, Sprite.BoundingRectangle.Width / 2f);
                _game.GameManager.CollisionWorld.AddBossBulletHitbox(_hitbox);
            }
        }

        public void Destroy()
        {
            _used = false;
            _game.SpriteBatchManager.BossBullets.Remove(this);
            _game.GameManager.CollisionWorld.RemoveBossBulletHitbox(_hitbox);

            // TODO: Animation or particles
        }

        public override void Update()
        {
            base.Update();

            // SpriteIndex changed? => we need to update the bullet texture
            if (_currentSpriteIndex != SpriteIndex)
            {
                _currentSpriteIndex = SpriteIndex;
                var moverManager = (MoverManager)BulletManager;
                if (SpriteIndex < moverManager.BulletTextures.Count)
                    Sprite = new Sprite(moverManager.BulletTextures[SpriteIndex]);
            }

            Sprite.Alpha = MathHelper.Lerp(Sprite.Alpha, 1f, 0.05f);
            Sprite.Scale = new Vector2(
                MathHelper.Lerp(Sprite.Scale.X, 1f, 0.05f),
                MathHelper.Lerp(Sprite.Scale.Y, 1f, 0.05f)
            );

            Sprite.Position = _position;
            Sprite.Rotation = Direction;
            Sprite.Scale = Scale();

            Sprite.Color = Color;

            if (X < GameConfig.BulletArea.X || X > _game.ViewportAdapter.VirtualWidth + GameConfig.BulletArea.Width ||
                Y < GameConfig.BulletArea.Y || Y > _game.ViewportAdapter.VirtualHeight + GameConfig.BulletArea.Height)
            {
                Destroy();
            }
        }

        public void TakeDamage(float damage)
        {
            // Nothing
        }
    }
}