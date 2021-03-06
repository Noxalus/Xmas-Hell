using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using SpriterDotNet;
using SpriterDotNet.MonoGame;
using SpriterDotNet.MonoGame.Sprites;
using System;
using System.IO;
using XmasHell.Rendering;
using System.Diagnostics;
using MonoGame.Extended;
using XmasHell.Physics;

namespace XmasHell.Spriter
{
    public class CustomSpriterAnimator : MonoGameAnimator, IComparable<CustomSpriterAnimator>, ISpriterPhysicsEntity, IMovable, IRotatable, IScalable
    {
        private readonly IProviderFactory<ISprite, SoundEffect> _providerFactory;
        private readonly Stack<SpriteDrawInfo> _drawInfoPool;
        private readonly IDictionary<string, ISprite> _boxTextures = new Dictionary<string, ISprite>();
        private readonly ISprite _pointTexture;
        private List<string> _hiddenTextures;
        private Dictionary<string, Texture2D> _textureSwapMap;
        public bool StretchOut = true;
        private int _ZIndex;

        public int ZIndex()
        {
            return _ZIndex;
        }

        public void ZIndex(int value, Layer? layer = null)
        {
            _ZIndex = value;
            XmasHell.Instance().SpriteBatchManager.SortSpriterAnimator(layer);
        }

        public CustomSpriterAnimator(
            SpriterEntity entity,
            IProviderFactory<ISprite, SoundEffect> providerFactory = null,
            Stack<SpriteDrawInfo> drawInfoPool = null
        ) : base(entity, providerFactory, drawInfoPool)
        {
            _providerFactory = providerFactory;
            _drawInfoPool = drawInfoPool;

            _hiddenTextures = new List<string>();
            _textureSwapMap = new Dictionary<string, Texture2D>();
            _pointTexture = new TextureSprite(TextureUtil.CreateCircle(XmasHell.Instance().GraphicsDevice, 1, Color.Cyan));

            if (entity.ObjectInfos != null)
            {
                foreach (SpriterObjectInfo objInfo in entity.ObjectInfos)
                {
                    if (objInfo.ObjectType != SpriterObjectType.Box) continue;
                    _boxTextures[objInfo.Name] = new TextureSprite(TextureUtil.CreateRectangle(XmasHell.Instance().GraphicsDevice, (int)objInfo.Width, (int)objInfo.Height, Color.Cyan));
                }
            }
        }

        public CustomSpriterAnimator Clone()
        {
            var clone = new CustomSpriterAnimator(Entity, _providerFactory, _drawInfoPool);
            clone.ZIndex(_ZIndex);
            return clone;
        }

        public void AddHiddenTexture(string hiddenTextureName)
        {
            var textureName = Path.GetFileNameWithoutExtension(hiddenTextureName);
            if (!_hiddenTextures.Contains(textureName))
                _hiddenTextures.Add(textureName);
        }

        public void AddTextureSwap(string originalTextureName, Texture2D textureToUse)
        {
            if (_textureSwapMap.ContainsKey(originalTextureName))
                _textureSwapMap.Remove(originalTextureName);

            _textureSwapMap.Add(originalTextureName, textureToUse);
        }

        protected override void ApplyPointTransform(string name, SpriterObject info)
        {
            if (!GameConfig.DebugPhysics || _pointTexture == null) return;
            ApplySpriteTransform(_pointTexture, info);
        }

        protected override void ApplyBoxTransform(SpriterObjectInfo objInfo, SpriterObject info)
        {
            if (!GameConfig.DebugPhysics) return;

            ApplySpriteTransform(_boxTextures[objInfo.Name], info);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < DrawInfos.Count; ++i)
            {
                SpriteDrawInfo di = DrawInfos[i];
                ISprite sprite = di.Drawable;

                if (sprite == null || sprite.Texture() == null)
                    continue;

                var currentTextureName = Path.GetFileNameWithoutExtension(sprite.Texture().Name);

                if (_hiddenTextures.Find(n => n == currentTextureName) == null)
                {
                    if (sprite.Texture().Name != null && _textureSwapMap.ContainsKey(sprite.Texture().Name) && _textureSwapMap[sprite.Texture().Name] != null)
                        sprite = new TextureSprite(_textureSwapMap[sprite.Texture().Name]);

                    sprite.Draw(spriteBatch, di.Pivot, di.Position, di.Scale, di.Rotation, di.Color, di.Depth, StretchOut);
                }
            }
        }

        public int CompareTo(CustomSpriterAnimator other)
        {
            return _ZIndex.CompareTo(other.ZIndex());
        }

        public CustomSpriterAnimator GetCurrentAnimator()
        {
            return this;
        }

        Vector2 IPhysicsEntity.Position()
        {
            return Position;
        }

        float IPhysicsEntity.Rotation()
        {
            return Rotation;
        }

        Vector2 IPhysicsEntity.ScaleVector()
        {
            return Scale;
        }

        public Vector2 Origin()
        {
            // TODO
            return Vector2.Zero;
        }

        public void TakeDamage(float damage)
        {
        }
    }
}