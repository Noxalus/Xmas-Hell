using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using SpriterDotNet;
using SpriterDotNet.MonoGame;
using SpriterDotNet.MonoGame.Sprites;

namespace XmasHell.Spriter
{
    public class CustomSpriterAnimator : MonoGameAnimator
    {
        private readonly IDictionary<string, ISprite> _boxTextures = new Dictionary<string, ISprite>();
        private readonly ISprite _pointTexture;
        public bool StretchOut = true;

        public CustomSpriterAnimator(
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

                sprite.Draw(spriteBatch, di.Pivot, di.Position, di.Scale, di.Rotation, di.Color, di.Depth, StretchOut);
            }
        }
    }
}