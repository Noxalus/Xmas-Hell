using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.BitmapFonts;
using Stream = System.IO.Stream;
using Microsoft.Xna.Framework.Audio;
using XmasHell.Spriter;
using SpriterDotNet;
using SpriterDotNet.Providers;
using SpriterDotNet.MonoGame;
using SpriterDotNet.MonoGame.Content;
using System;

#if ANDROID
using Android.App;
#else
using System.IO;
#endif

namespace XmasHell
{
    public static class Assets
    {
        private static List<BitmapFont> _fonts;
        private static List<Texture2D> _textures;
        private static Dictionary<string, Stream> _patternSteams;
        private static List<SoundEffect> _musics; // Don't use Song type to avoid delay when looping with the MediaManager
        private static List<SoundEffect> _soundEffects;
        private static List<Effect> _effects;

        // Spriter
        private static readonly Config DefaultAnimatorConfig = new Config
        {
            MetadataEnabled = true,
            EventsEnabled = true,
            PoolingEnabled = true,
            TagsEnabled = false,
            VarsEnabled = false,
            SoundsEnabled = false
        };
        private static Dictionary<string, Dictionary<string, CustomSpriterAnimator>> _spriterAnimators;

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

        private static Dictionary<string, CustomSpriterAnimator> LoadSpriterFile(ContentManager content, string filename)
        {
            var factory = new DefaultProviderFactory<ISprite, SoundEffect>(DefaultAnimatorConfig, true);

            var loader = new SpriterContentLoader(content, filename);
            loader.Fill(factory);

            var animators = new Dictionary<string, CustomSpriterAnimator>();

            foreach (var entity in loader.Spriter.Entities)
            {
                var animator = new CustomSpriterAnimator(entity, factory);

                // Center the animator
                animator.Position = XmasHell.Instance().ViewportAdapter.Center.ToVector2();

                animators.Add(entity.Name, animator);
            }

            return animators;
        }

        public static void Load(ContentManager content, GraphicsDevice graphicsDevice)
        {
            var pixel = new Texture2D(graphicsDevice, 1, 1) { Name = "pixel" };
            pixel.SetData(new[] { Color.White });

            // Load sprites
            _textures = new List<Texture2D>()
            {
                // Pictures
                pixel,
                content.Load<Texture2D>("Graphics/Pictures/snow"),

                // GUI

                // Main Menu

                // Boss Selection Menu
                content.Load<Texture2D>("Graphics/GUI/BossSelection/xmas-ball-available-button"),
                content.Load<Texture2D>("Graphics/GUI/BossSelection/xmas-ball-beaten-button"),
                content.Load<Texture2D>("Graphics/GUI/BossSelection/xmas-bell-available-button"),
                content.Load<Texture2D>("Graphics/GUI/BossSelection/xmas-bell-beaten-button"),
                content.Load<Texture2D>("Graphics/GUI/BossSelection/xmas-candy-available-button"),
                content.Load<Texture2D>("Graphics/GUI/BossSelection/xmas-candy-beaten-button"),
                content.Load<Texture2D>("Graphics/GUI/BossSelection/xmas-snowflake-available-button"),
                content.Load<Texture2D>("Graphics/GUI/BossSelection/xmas-snowflake-beaten-button"),
                content.Load<Texture2D>("Graphics/GUI/BossSelection/xmas-gift-available-button"),
                content.Load<Texture2D>("Graphics/GUI/BossSelection/xmas-gift-beaten-button"),
                content.Load<Texture2D>("Graphics/GUI/BossSelection/xmas-log-available-button"),
                content.Load<Texture2D>("Graphics/GUI/BossSelection/xmas-log-beaten-button"),
                content.Load<Texture2D>("Graphics/GUI/BossSelection/xmas-tree-available-button"),
                content.Load<Texture2D>("Graphics/GUI/BossSelection/xmas-tree-beaten-button"),
                content.Load<Texture2D>("Graphics/GUI/BossSelection/xmas-reindeer-available-button"),
                content.Load<Texture2D>("Graphics/GUI/BossSelection/xmas-reindeer-beaten-button"),
                content.Load<Texture2D>("Graphics/GUI/BossSelection/xmas-snowman-available-button"),
                content.Load<Texture2D>("Graphics/GUI/BossSelection/xmas-snowman-beaten-button"),
                content.Load<Texture2D>("Graphics/GUI/BossSelection/xmas-santa-available-button"),
                content.Load<Texture2D>("Graphics/GUI/BossSelection/xmas-santa-beaten-button"),

                content.Load<Texture2D>("Graphics/GUI/BossSelection/unknown-boss-button"),
                content.Load<Texture2D>("Graphics/GUI/BossSelection/hidden-boss-button"),

                // Sprites

                // Bullets
                content.Load<Texture2D>("Graphics/Sprites/Bullets/bullet1"),
                content.Load<Texture2D>("Graphics/Sprites/Bullets/bullet2"),
                content.Load<Texture2D>("Graphics/Sprites/Bullets/bullet3"),
                content.Load<Texture2D>("Graphics/Sprites/Bullets/bullet4"),
                content.Load<Texture2D>("Graphics/Sprites/Bullets/carrot"),

                content.Load<Texture2D>("Graphics/Sprites/Bullets/laser"),

                content.Load<Texture2D>("Graphics/Sprites/Player/hitbox"),

                // Snowflake
                content.Load<Texture2D>("Graphics/Sprites/Bosses/XmasSnowflake/branch1"),
                content.Load<Texture2D>("Graphics/Sprites/Bosses/XmasSnowflake/branch2"),

                // Candy bar
                content.Load<Texture2D>("Graphics/Sprites/Bosses/XmasCandy/candy-bar"),

                // Gift
                content.Load<Texture2D>("Graphics/Sprites/Bosses/XmasGift/body2"),
                content.Load<Texture2D>("Graphics/Sprites/Bosses/XmasGift/body3"),
                content.Load<Texture2D>("Graphics/Sprites/Bosses/XmasGift/body4"),
                content.Load<Texture2D>("Graphics/Sprites/Bosses/XmasGift/body5"),
                content.Load<Texture2D>("Graphics/Sprites/Bosses/XmasGift/body6"),
                content.Load<Texture2D>("Graphics/Sprites/Bosses/XmasGift/ribbon2"),
                content.Load<Texture2D>("Graphics/Sprites/Bosses/XmasGift/ribbon3"),
                content.Load<Texture2D>("Graphics/Sprites/Bosses/XmasGift/ribbon4"),
                content.Load<Texture2D>("Graphics/Sprites/Bosses/XmasGift/ribbon5"),
                content.Load<Texture2D>("Graphics/Sprites/Bosses/XmasGift/ribbon6"),

                // Snowman
                content.Load<Texture2D>("Graphics/Sprites/Bosses/XmasSnowman/snowball"),
                content.Load<Texture2D>("Graphics/Sprites/Bosses/XmasSnowman/big-arm"),

                // Clouds
                content.Load<Texture2D>("Graphics/GUI/cloud1"),
                content.Load<Texture2D>("Graphics/GUI/cloud2")
            };

            _spriterAnimators = new Dictionary<string, Dictionary<string, CustomSpriterAnimator>>()
            {
                { "Graphics/GUI/main-menu", LoadSpriterFile(content, "Graphics/GUI/main-menu") },
                { "Graphics/GUI/boss-selection", LoadSpriterFile(content, "Graphics/GUI/boss-selection") },
                { "Graphics/GUI/game-screen", LoadSpriterFile(content, "Graphics/GUI/game-screen") }
            };

            // Load fonts
            _fonts = new List<BitmapFont>()
            {
                content.Load<BitmapFont>("Graphics/Fonts/main"),
                content.Load<BitmapFont>("Graphics/Fonts/ui"),
                content.Load<BitmapFont>("Graphics/Fonts/ui-title"),
                content.Load<BitmapFont>("Graphics/Fonts/ui-small")
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
                { "XmasBall/pattern2", OpenRawFile(BuildRawAssetPath("XmasBall/pattern2.xml")) },
                { "XmasBall/pattern3", OpenRawFile(BuildRawAssetPath("XmasBall/pattern3.xml")) },
                { "XmasBall/pattern4", OpenRawFile(BuildRawAssetPath("XmasBall/pattern4.xml")) },

                // Xmas Bell
                { "XmasBell/pattern1", OpenRawFile(BuildRawAssetPath("XmasBell/pattern1.xml")) },
                { "XmasBell/pattern2", OpenRawFile(BuildRawAssetPath("XmasBell/pattern2.xml")) },
                { "XmasBell/pattern4", OpenRawFile(BuildRawAssetPath("XmasBell/pattern4.xml")) },
                { "XmasBell/pattern5", OpenRawFile(BuildRawAssetPath("XmasBell/pattern5.xml")) },

                // Xmas Candy
                { "XmasCandy/pattern1", OpenRawFile(BuildRawAssetPath("XmasCandy/pattern1.xml")) },
                { "XmasCandy/pattern2", OpenRawFile(BuildRawAssetPath("XmasCandy/pattern2.xml")) },
                { "XmasCandy/pattern3", OpenRawFile(BuildRawAssetPath("XmasCandy/pattern3.xml")) },
                { "XmasCandy/pattern4", OpenRawFile(BuildRawAssetPath("XmasCandy/pattern4.xml")) },

                // Xmas Snowflake
                { "XmasSnowflake/pattern1", OpenRawFile(BuildRawAssetPath("XmasSnowflake/pattern1.xml")) },
                { "XmasSnowflake/pattern2", OpenRawFile(BuildRawAssetPath("XmasSnowflake/pattern2.xml")) },
                { "XmasSnowflake/pattern3", OpenRawFile(BuildRawAssetPath("XmasSnowflake/pattern3.xml")) },
                { "XmasSnowflake/pattern4", OpenRawFile(BuildRawAssetPath("XmasSnowflake/pattern4.xml")) },

                // Xmas Gift
                { "XmasGift/pattern1", OpenRawFile(BuildRawAssetPath("XmasGift/pattern1.xml")) },

                // Xmas Log

                // Xmas Tree
                { "XmasTree/pattern1", OpenRawFile(BuildRawAssetPath("XmasTree/pattern1.xml")) },

                // Xmas Reindeer
                { "XmasReindeer/pattern1", OpenRawFile(BuildRawAssetPath("XmasReindeer/pattern1.xml")) },

                // Xmas Snowman
                { "XmasSnowman/pattern1", OpenRawFile(BuildRawAssetPath("XmasSnowman/pattern1.xml")) },
                { "XmasSnowman/pattern3_1", OpenRawFile(BuildRawAssetPath("XmasSnowman/pattern3_1.xml")) },
                { "XmasSnowman/pattern3_2", OpenRawFile(BuildRawAssetPath("XmasSnowman/pattern3_2.xml")) },

                // Xmas Santa
                { "XmasSanta/pattern1", OpenRawFile(BuildRawAssetPath("XmasSanta/pattern1.xml")) },
            };

            // Load musics
            _musics = new List<SoundEffect>
            {
                content.Load<SoundEffect>("Audio/BGM/menu-theme"),
                content.Load<SoundEffect>("Audio/BGM/boss-intro"),
                content.Load<SoundEffect>("Audio/BGM/boss-theme")
            };

            // Load sounds
            _soundEffects = new List<SoundEffect>
            {
                content.Load<SoundEffect>("Audio/SE/shoot1"),
                content.Load<SoundEffect>("Audio/SE/shoot2"),
                content.Load<SoundEffect>("Audio/SE/shoot3"),
                content.Load<SoundEffect>("Audio/SE/shoot4"),
                content.Load<SoundEffect>("Audio/SE/boss-hit1"),
                content.Load<SoundEffect>("Audio/SE/boss-hit2"),
                content.Load<SoundEffect>("Audio/SE/boss-hit3"),
                content.Load<SoundEffect>("Audio/SE/player-death"),
                content.Load<SoundEffect>("Audio/SE/shake")
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

        public static Dictionary<string, CustomSpriterAnimator> GetSpriterAnimators(string spriterFilename)
        {
            if (!_spriterAnimators.ContainsKey(spriterFilename))
                throw new Exception("This Spriter file doesn't exist");

            return _spriterAnimators[spriterFilename];
        }

        public static BitmapFont GetFont(string fontName)
        {
            return _fonts.Find(f => f.Name == fontName);
        }

        public static Stream GetPattern(string patternName)
        {
            return _patternSteams[patternName];
        }

        public static SoundEffect GetMusic(string musicName)
        {
            return _musics.Find(m => m.Name == musicName);
        }

        public static SoundEffect GetSound(string soundName)
        {
            return _soundEffects.Find(m => m.Name == soundName);
        }

        public static Effect GetShader(string shaderName)
        {
            return _effects.Find(s => s.Name == shaderName);
        }
    }
}