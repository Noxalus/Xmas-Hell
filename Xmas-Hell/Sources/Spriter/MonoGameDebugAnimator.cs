using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using SpriterDotNet;
using SpriterDotNet.MonoGame;

namespace XmasHell.Spriter
{
    public class MonoGameDebugAnimator : MonoGameAnimator
    {
        private readonly IDictionary<string, Sprite> _boxTextures = new Dictionary<string, Sprite>();
        private readonly Sprite _pointTexture;

        public MonoGameDebugAnimator(SpriterEntity entity, GraphicsDevice graphicsDevice, IProviderFactory<Sprite, SoundEffect> providerFactory = null) : base(entity, providerFactory)
        {
            _pointTexture = new Sprite
            {
                Texture = TextureUtil.CreateCircle(graphicsDevice, 1, Color.Cyan)
            };

            if (entity.ObjectInfos != null)
            {
                foreach (SpriterObjectInfo objInfo in entity.ObjectInfos)
                {
                    if (objInfo.ObjectType != SpriterObjectType.Box) continue;
                    _boxTextures[objInfo.Name] = new Sprite { Texture = TextureUtil.CreateRectangle(graphicsDevice, (int)objInfo.Width, (int)objInfo.Height, Color.Cyan) };
                }
            }
        }

        protected override void ApplyPointTransform(string name, SpriterObject info)
        {
            if (_pointTexture == null) return;
            ApplySpriteTransform(_pointTexture, info);
        }

        protected override void ApplyBoxTransform(SpriterObjectInfo objInfo, SpriterObject info)
        {
            ApplySpriteTransform(_boxTextures[objInfo.Name], info);
        }
    }
}