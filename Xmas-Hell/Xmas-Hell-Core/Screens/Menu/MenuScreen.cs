using BulletML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using XmasHell.BulletML;
using XmasHell.Rendering;
using XmasHell.Spriter;
using XnaMediaPlayer = Microsoft.Xna.Framework.Media.MediaPlayer;

namespace XmasHell.Screens.Menu
{
    public class MenuScreen : Screen
    {
        protected Dictionary<string, CustomSpriterAnimator> SpriterFile;

        // Snow rain
        private string _patternFile = "MainMenu/snowflake";
        private TimeSpan _shootFrequency;

        // Clouds
        private int _cloud1Speed;
        private int _cloud2Speed;
        private TimeSpan _cloud1Timer;
        private TimeSpan _cloud2Timer;

        public MenuScreen(XmasHell game) : base(game)
        {
        }

        public override void Initialize()
        {
            _shootFrequency = TimeSpan.Zero;
            _cloud1Timer = TimeSpan.Zero;
            _cloud2Timer = TimeSpan.Zero;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            if (Game.GameManager.MoverManager.FindPattern(_patternFile) == null)
            {
                var pattern = new BulletPattern();
                pattern.ParseStream(_patternFile, Assets.GetPattern(_patternFile));

                Game.GameManager.MoverManager.AddPattern(_patternFile, pattern);
            }
        }

        protected virtual void ResetUI()
        {
        }

        public override void Show(bool reset)
        {
            base.Show(reset);

            if (XnaMediaPlayer.State == MediaState.Stopped)
            {
                XnaMediaPlayer.Volume = 1f;
                XnaMediaPlayer.IsRepeating = true;
                XnaMediaPlayer.Play(Assets.GetMusic("main-menu"));
            }

            InitializeClouds();
        }

        private void InitializeClouds()
        {
            if (SpriterFile.ContainsKey("Cloud1") && SpriterFile.ContainsKey("Main"))
            {
                Game.SpriteBatchManager.AddSpriterAnimator(SpriterFile["Cloud1"], Layer.BACKGROUND);
                SpriterFile["Cloud1"].zIndex(-1);
                SpriterFile["Cloud1"].Position = Vector2.Zero;
                SpriterFile["Main"].AddHiddenTexture("Graphics/GUI/cloud1");
            }
            if (SpriterFile.ContainsKey("Cloud2") && SpriterFile.ContainsKey("Main"))
            {
                Game.SpriteBatchManager.AddSpriterAnimator(SpriterFile["Cloud2"], Layer.BACKGROUND);
                SpriterFile["Cloud2"].zIndex(-1);
                SpriterFile["Cloud2"].Position = Vector2.Zero;
                SpriterFile["Main"].AddHiddenTexture("Graphics/GUI/cloud2");
            }
        }

        public override void Hide()
        {
            base.Hide();

            Game.SpriteBatchManager.RemoveSpriterAnimator(SpriterFile["Cloud1"], Layer.BACKGROUND);
            Game.SpriteBatchManager.RemoveSpriterAnimator(SpriterFile["Cloud2"], Layer.BACKGROUND);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            UpdateSnowRain(gameTime);
            UpdateClouds(gameTime);
        }

        private void UpdateSnowRain(GameTime gameTime)
        {
            if (_shootFrequency.TotalMilliseconds < 0)
            {
                var randomX = Game.GameManager.Random.Next(0, Game.ViewportAdapter.VirtualWidth);
                var position = new Vector2(randomX, -500);

                Game.GameManager.MoverManager.TriggerPattern(_patternFile, BulletType.Type1, false, position);
                _shootFrequency = TimeSpan.FromSeconds(0.01);
            }
            else
            {
                _shootFrequency -= gameTime.ElapsedGameTime;
            }
        }

        private void UpdateClouds(GameTime gameTime)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (SpriterFile["Cloud1"] != null)
            {
                if (_cloud1Timer.TotalMilliseconds < 0)
                {
                    if (SpriterFile["Cloud1"].Position == Vector2.Zero || SpriterFile["Cloud1"].Position.X > GameConfig.VirtualResolution.X + 150)
                    {
                        _cloud1Speed = Game.GameManager.Random.Next(50, 150);
                        SpriterFile["Cloud1"].Position = new Vector2(-150, Game.GameManager.Random.Next(0, 300));
                        _cloud1Timer = TimeSpan.FromSeconds(Game.GameManager.Random.NextDouble() * 20f);
                    }

                    SpriterFile["Cloud1"].Position = new Vector2(
                        SpriterFile["Cloud1"].Position.X + (_cloud1Speed * dt),
                        SpriterFile["Cloud1"].Position.Y
                    );

                }
                else
                    _cloud1Timer -= gameTime.ElapsedGameTime;
            }

            if (SpriterFile["Cloud2"] != null)
            {
                if (_cloud2Timer.TotalMilliseconds < 0)
                {
                    if (SpriterFile["Cloud2"].Position == Vector2.Zero || SpriterFile["Cloud2"].Position.X > GameConfig.VirtualResolution.X + 150)
                    {
                        _cloud2Speed = Game.GameManager.Random.Next(50, 150);
                        SpriterFile["Cloud2"].Position = new Vector2(-150, Game.GameManager.Random.Next(0, 300));
                        _cloud2Timer = TimeSpan.FromSeconds(Game.GameManager.Random.NextDouble() * 20f);
                    }

                    SpriterFile["Cloud2"].Position = new Vector2(
                        SpriterFile["Cloud2"].Position.X + (_cloud2Speed * dt),
                        SpriterFile["Cloud2"].Position.Y
                    );
                }
                else
                    _cloud2Timer -= gameTime.ElapsedGameTime;
            }
        }
    }
}
