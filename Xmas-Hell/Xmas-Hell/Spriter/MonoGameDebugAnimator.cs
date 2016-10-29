using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using SpriterDotNet;
using SpriterDotNet.MonoGame;

namespace Xmas_Hell.Spriter
{
    public class MonoGameDebugAnimator : MonoGameAnimator
    {
        private IDictionary<string, Sprite> boxTextures = new Dictionary<string, Sprite>();
        private Sprite pointTexture;

        public MonoGameDebugAnimator(SpriterEntity entity, GraphicsDevice graphicsDevice, IProviderFactory<Sprite, SoundEffect> providerFactory = null) : base(entity, providerFactory)
        {
            pointTexture = new Sprite { Texture = TextureUtil.CreateCircle(graphicsDevice, 5, Color.Cyan) };

            if (entity.ObjectInfos != null)
            {
                foreach (SpriterObjectInfo objInfo in entity.ObjectInfos)
                {
                    if (objInfo.ObjectType != SpriterObjectType.Box) continue;
                    boxTextures[objInfo.Name] = new Sprite { Texture = TextureUtil.CreateRectangle(graphicsDevice, (int)objInfo.Width, (int)objInfo.Height, Color.Cyan) };
                }
            }
        }

        protected override void ApplyPointTransform(string name, SpriterObject info)
        {
            if (pointTexture == null) return;
            ApplySpriteTransform(pointTexture, info);
        }

        protected override void ApplyBoxTransform(SpriterObjectInfo objInfo, SpriterObject info)
        {
            ApplySpriteTransform(boxTextures[objInfo.Name], info);
        }
    }
}