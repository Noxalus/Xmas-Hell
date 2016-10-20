using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace Xmas_Hell
{
    public static class Assets
    {
        private static List<BitmapFont> _fonts;
        private static List<Texture2D> _textures;

        public static void Load(ContentManager content)
        {
            // Load sprites
            _textures = new List<Texture2D>()
            {
                // Pictures
                content.Load<Texture2D>("Graphics/Pictures/pixel"),

                // Sprites
                content.Load<Texture2D>("Graphics/Sprites/bullet"),
                content.Load<Texture2D>("Graphics/Sprites/bullet2"),
                content.Load<Texture2D>("Graphics/Sprites/player"),
                content.Load<Texture2D>("Graphics/Sprites/boss")
            };

            // Load fonts
            _fonts = new List<BitmapFont>()
            {
                content.Load<BitmapFont>("Graphics/Fonts/main")
            };
        }

        public static Texture2D GetTexture2D(string textureName)
        {
            return _textures.Find(t => t.Name == textureName);
        }

        public static BitmapFont GetFont(string fontName)
        {
            return _fonts.Find(f => f.Name == fontName);
        }
    }
}