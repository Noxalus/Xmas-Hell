using BulletML;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using XmasHell.BulletML;
using XmasHell.Spriter;

namespace XmasHell.Screens.Menu
{
    public class MenuScreen : Screen
    {
        protected Dictionary<string, CustomSpriterAnimator> SpriterFile;

        // Snow rain
        private string _patternFile = "MainMenu/snowflake";
        private TimeSpan _shootFrequency;

        public MenuScreen(XmasHell game) : base(game)
        {
        }

        public override void Initialize()
        {
            _shootFrequency = TimeSpan.Zero;
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

            Game.MusicManager.PlayMenuMusic();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            UpdateSnowRain(gameTime);
            Game.CloudManager.Update(gameTime);
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
    }
}
