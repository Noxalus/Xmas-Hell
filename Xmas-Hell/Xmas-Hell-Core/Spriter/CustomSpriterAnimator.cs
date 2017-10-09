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

namespace XmasHell.Spriter
{
    public class CustomSpriterAnimator : MonoGameAnimator, IComparable<CustomSpriterAnimator>
    {
        private readonly XmasHell _game;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly IProviderFactory<ISprite, SoundEffect> _providerFactory;
        private readonly Stack<SpriteDrawInfo> _drawInfoPool;
        private readonly IDictionary<string, ISprite> _boxTextures = new Dictionary<string, ISprite>();
        private readonly ISprite _pointTexture;
        private List<string> _hiddenTextures;
        private Dictionary<string, Texture2D> _textureSwapMap;
        public bool StretchOut = true;
        private int _zIndex;

        // GUI texts
        private string _selectedBossName = "Unknown";
        private string _selectedBossBestTime = "Best time: ";
        private string _selectedBossPlayTime = "Play time: ";
        private string _selectedBossPlayerDeath = "Player death: ";
        private string _selectedBossBossDeath = "Boss death: ";

        public int zIndex()
        {
            return _zIndex;
        }

        public void zIndex(int value, Layer? layer = null)
        {
            _zIndex = value;
            _game.SpriteBatchManager.SortSpriterAnimator(layer);
        }

        public CustomSpriterAnimator(
            XmasHell game,
            SpriterEntity entity,
            IProviderFactory<ISprite, SoundEffect> providerFactory = null,
            Stack<SpriteDrawInfo> drawInfoPool = null
        ) : base(entity, providerFactory, drawInfoPool)
        {
            _game = game;
            _providerFactory = providerFactory;
            _drawInfoPool = drawInfoPool;

            _hiddenTextures = new List<string>();
            _textureSwapMap = new Dictionary<string, Texture2D>();
            _pointTexture = new TextureSprite(TextureUtil.CreateCircle(_game.GraphicsDevice, 1, Color.Cyan));

            if (entity.ObjectInfos != null)
            {
                foreach (SpriterObjectInfo objInfo in entity.ObjectInfos)
                {
                    if (objInfo.ObjectType != SpriterObjectType.Box) continue;
                    _boxTextures[objInfo.Name] = new TextureSprite(TextureUtil.CreateRectangle(_game.GraphicsDevice, (int)objInfo.Width, (int)objInfo.Height, Color.Cyan));
                }
            }
        }

        public CustomSpriterAnimator Clone()
        {
            var clone = new CustomSpriterAnimator(_game, Entity, _providerFactory, _drawInfoPool);
            clone.zIndex(_zIndex);
            return clone;
        }

        public void SetHiddenTextures(List<string> hiddenTextures)
        {
            _hiddenTextures = hiddenTextures;
        }

        public void AddHiddenTexture(string hiddenTextureName)
        {
            _hiddenTextures.Add(hiddenTextureName);
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

                if (_hiddenTextures.Find(n => Path.GetFileNameWithoutExtension(n) == Path.GetFileNameWithoutExtension(sprite.Texture().Name)) == null)
                {
                    if (sprite.Texture().Name != null && _textureSwapMap.ContainsKey(sprite.Texture().Name) && _textureSwapMap[sprite.Texture().Name] != null)
                        sprite = new TextureSprite(_textureSwapMap[sprite.Texture().Name]);

                    sprite.Draw(spriteBatch, di.Pivot, di.Position, di.Scale, di.Rotation, di.Color, di.Depth, StretchOut);
                }
            }
        }

        public int CompareTo(CustomSpriterAnimator other)
        {
            return _zIndex.CompareTo(other.zIndex());
        }
    }
}