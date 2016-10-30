using System.Collections.Generic;
using Android.App;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.BitmapFonts;
using Stream = System.IO.Stream;

namespace Xmas_Hell
{
    public static class Assets
    {
        private static List<BitmapFont> _fonts;
        private static List<Texture2D> _textures;
        private static Dictionary<string, Stream> _patternSteams;
        private static List<Song> _musics;

        public static void Load(Activity activity, ContentManager content, GraphicsDevice graphicsDevice)
        {
            var pixel = new Texture2D(graphicsDevice, 1, 1) {Name = "pixel"};
            pixel.SetData(new[] { Color.White });

            // Load sprites
            _textures = new List<Texture2D>()
            {
                // Pictures
                pixel,
                content.Load<Texture2D>("Graphics/Pictures/background"),

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

            // Load BulletML files
            _patternSteams = new Dictionary<string, Stream>
            {
                { "sample", activity.ApplicationContext.Assets.Open("Patterns/sample.xml") }
            };

            // Load musics
            _musics = new List<Song>()
            {
                content.Load<Song>("Audio/BGM/boss-theme")
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

        public static Stream GetPattern(string patternName)
        {
            return _patternSteams[patternName];
        }

        public static Song GetMusic(string musicName)
        {
            return _musics.Find(m => m.Name == musicName);
        }
    }
}