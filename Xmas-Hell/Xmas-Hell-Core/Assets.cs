using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.BitmapFonts;
using Stream = System.IO.Stream;

#if ANDROID
using Android.App;
#endif

namespace XmasHell
{
    public static class Assets
    {
        private static List<BitmapFont> _fonts;
        private static List<Texture2D> _textures;
        private static Dictionary<string, Stream> _patternSteams;
        private static List<Song> _musics;
        private static List<Effect> _effects;

#if ANDROID
        private static Activity _activity;

        public static void SetActivity(Activity activity)
        {
            _activity = activity;
        }
#endif

        private static string GetPatternsFolder()
        {
#if ANDROID
            return "Patterns/";
#else
            return "Assets/Patterns/";
#endif
        }

        private static Stream OpenRawFile(string path)
        {
#if ANDROID
            return _activity.ApplicationContext.Assets.Open(path);
#else
            return File.Open(path, FileMode.Open);
#endif
        }

        private static string BuildRawAssetPath(string path)
        {
            return GetPatternsFolder() + path;
        }

#if ANDROID
        public static void Load(ContentManager content, GraphicsDevice graphicsDevice)
#else
        public static void Load(ContentManager content, GraphicsDevice graphicsDevice)
#endif
        {
            var pixel = new Texture2D(graphicsDevice, 1, 1) {Name = "pixel"};
            pixel.SetData(new[] { Color.White });

            // Load sprites
            _textures = new List<Texture2D>()
            {
                // Pictures
                pixel,
                content.Load<Texture2D>("Graphics/Pictures/snow"),

                // GUI
                content.Load<Texture2D>("Graphics/GUI/play-button"),
                content.Load<Texture2D>("Graphics/GUI/boss-selection-ground"),
                content.Load<Texture2D>("Graphics/GUI/boss-selection-tree"),
                content.Load<Texture2D>("Graphics/GUI/boss-selection-tree-star"),
                content.Load<Texture2D>("Graphics/GUI/unknown-boss-button"),

                // Sprites
                content.Load<Texture2D>("Graphics/Sprites/Bullets/bullet1"),
                content.Load<Texture2D>("Graphics/Sprites/Bullets/bullet2"),
                content.Load<Texture2D>("Graphics/Sprites/Bullets/bullet3"),
                content.Load<Texture2D>("Graphics/Sprites/Player/hitbox"),
                content.Load<Texture2D>("Graphics/Sprites/Bosses/XmasSnowflake/branch1"),
                content.Load<Texture2D>("Graphics/Sprites/Bosses/XmasSnowflake/branch2"),
            };

            // Load fonts
            _fonts = new List<BitmapFont>()
            {
                content.Load<BitmapFont>("Graphics/Fonts/main")
            };

            // Load BulletML files
            _patternSteams = new Dictionary<string, Stream>
            {
                // General
                { "sample", OpenRawFile(BuildRawAssetPath("sample.xml")) },
                { "MainMenu/snowflake", OpenRawFile(BuildRawAssetPath("MainMenu/snowflake.xml")) },

                // Debug
                { "BossDebug/pattern1", OpenRawFile(BuildRawAssetPath("BossDebug/pattern1.xml")) },

                // Xmas Ball
                { "XmasBall/pattern1", OpenRawFile(BuildRawAssetPath("XmasBall/pattern1.xml")) },
                { "XmasBall/pattern3", OpenRawFile(BuildRawAssetPath("XmasBall/pattern3.xml")) },
                { "XmasBall/pattern4", OpenRawFile(BuildRawAssetPath("XmasBall/pattern4.xml")) },

                // Xmas Bell
                { "XmasBell/pattern1", OpenRawFile(BuildRawAssetPath("XmasBell/pattern1.xml")) },
                { "XmasBell/pattern2", OpenRawFile(BuildRawAssetPath("XmasBell/pattern2.xml")) },

                // Xmas Candy

                // Xmas Snowflake
                { "XmasSnowflake/pattern1", OpenRawFile(BuildRawAssetPath("XmasSnowflake/pattern1.xml")) },
                { "XmasSnowflake/pattern2", OpenRawFile(BuildRawAssetPath("XmasSnowflake/pattern2.xml")) },
                { "XmasSnowflake/pattern3", OpenRawFile(BuildRawAssetPath("XmasSnowflake/pattern3.xml")) },
                { "XmasSnowflake/pattern4", OpenRawFile(BuildRawAssetPath("XmasSnowflake/pattern4.xml")) },

                // Xmas Gift

                // Xmas Log

                // Xmas Tree
            };

            // Load musics
            _musics = new List<Song>()
            {
                content.Load<Song>("Audio/BGM/boss-theme-intro"),
                content.Load<Song>("Audio/BGM/boss-theme-main")
            };

            // Load custom shaders
            _effects = new List<Effect>()
            {
                content.Load<Effect>("Graphics/Shaders/BasicTint"),
                content.Load<Effect>("Graphics/Shaders/AnimatedGradient"),
                content.Load<Effect>("Graphics/Shaders/Theter"),
                content.Load<Effect>("Graphics/Shaders/StarBackground"),
                content.Load<Effect>("Graphics/Shaders/SnowRainBackground"),
                content.Load<Effect>("Graphics/Shaders/Snowflake"),
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

        public static Effect GetShader(string shaderName)
        {
            return _effects.Find(s => s.Name == shaderName);
        }
    }
}