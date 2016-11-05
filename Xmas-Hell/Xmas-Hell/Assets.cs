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
                content.Load<Texture2D>("Graphics/Pictures/snow"),

                // Sprites
                content.Load<Texture2D>("Graphics/Sprites/Bullets/bullet1"),
                content.Load<Texture2D>("Graphics/Sprites/Bullets/bullet2"),
                content.Load<Texture2D>("Graphics/Sprites/Bullets/bullet3"),
                content.Load<Texture2D>("Graphics/Sprites/player")
            };

            // Load fonts
            _fonts = new List<BitmapFont>()
            {
                content.Load<BitmapFont>("Graphics/Fonts/main")
            };

            // Load BulletML files
            _patternSteams = new Dictionary<string, Stream>
            {
                { "sample", activity.ApplicationContext.Assets.Open("Patterns/sample.xml") },
                { "XmasBall/pattern1", activity.ApplicationContext.Assets.Open("Patterns/XmasBall/pattern1.xml") },
                { "XmasBall/pattern3", activity.ApplicationContext.Assets.Open("Patterns/XmasBall/pattern3.xml") },
                { "XmasBall/pattern4", activity.ApplicationContext.Assets.Open("Patterns/XmasBall/pattern4.xml") }
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