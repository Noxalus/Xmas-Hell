using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using SpriterDotNet;
using SpriterDotNet.MonoGame;
using SpriterDotNet.MonoGame.Sprites;

namespace XmasHell.Spriter
{
    public class MonoGameDebugAnimator : MonoGameAnimator
    {
        private readonly IDictionary<string, ISprite> _boxTextures = new Dictionary<string, ISprite>();
        private readonly ISprite _pointTexture;

        public MonoGameDebugAnimator(
            SpriterEntity entity,
            GraphicsDevice graphicsDevice,
            IProviderFactory<ISprite, SoundEffect> providerFactory = null,
            Stack<SpriteDrawInfo> drawInfoPool = null
        ) : base(entity, providerFactory, drawInfoPool)
        {
            _pointTexture = new TextureSprite(TextureUtil.CreateCircle(graphicsDevice, 1, Color.Cyan));

            if (entity.ObjectInfos != null)
            {
                foreach (SpriterObjectInfo objInfo in entity.ObjectInfos)
                {
                    if (objInfo.ObjectType != SpriterObjectType.Box) continue;
                    _boxTextures[objInfo.Name] = new TextureSprite(TextureUtil.CreateRectangle(graphicsDevice, (int)objInfo.Width, (int)objInfo.Height, Color.Cyan));
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