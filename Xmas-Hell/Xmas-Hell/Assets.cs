using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.BitmapFonts;

namespace Xmas_Hell
{
    public static class Assets
    {
        private static List<BitmapFont> _fonts;

        public static void Load(ContentManager content)
        {
            // Load fonts
            _fonts = new List<BitmapFont>()
            {
                content.Load<BitmapFont>("Graphics/Fonts/main")
            };
        }

        public static BitmapFont GetFont(string fontName)
        {
            return _fonts.Find(f => f.Name == fontName);
        }
    }
}