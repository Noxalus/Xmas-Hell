using Microsoft.Xna.Framework.Graphics;

namespace XmasHell.BulletML
{
    public enum BulletType
    {
        Type1 = 0,
        Type2 = 1,
        Type3 = 2,
        Type4 = 3
    }

    public static class BulletTypeUtils
    {
        public static Texture2D BulletTypeToTexture(BulletType type)
        {
            switch (type)
            {
                case BulletType.Type1:
                    return Assets.GetTexture2D("Graphics/Sprites/Bullets/bullet1");

                case BulletType.Type2:
                    return Assets.GetTexture2D("Graphics/Sprites/Bullets/bullet2");

                case BulletType.Type3:
                    return Assets.GetTexture2D("Graphics/Sprites/Bullets/bullet3");

                case BulletType.Type4:
                    return Assets.GetTexture2D("Graphics/Sprites/Bullets/bullet4");

                default:
                    return Assets.GetTexture2D("Graphics/Sprites/Bullets/bullet1");
            }
        }
    }
}