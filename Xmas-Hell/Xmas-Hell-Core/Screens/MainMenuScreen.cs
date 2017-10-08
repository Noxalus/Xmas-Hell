using System;
using BulletML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using XmasHell.BulletML;
using XmasHell.GUI;
using XmasHell.Spriter;
using XmasHell.Rendering;
using System.Collections.Generic;

namespace XmasHell.Screens
{
    public class MainMenuScreen : Screen
    {
        private string _patternFile = "MainMenu/snowflake";
        private TimeSpan _shootFrequency;
        private Song _introSong;
        private Song _mainSong;
        private Song _menuSong;

        private SpriterGuiButton _playButton;
        private SpriterGuiButton _settingsButton;

        public MainMenuScreen(XmasHell game) : base(game)
        {
        }

        public override void Initialize()
        {
            _shootFrequency = TimeSpan.Zero;

            base.Initialize();
        }

        private void OnPlayButtonAction(object button, Point position)
        {
            Animators["MainMenu"].Play("Zoom");
            Animators["MainMenu"].CurrentAnimation.Looping = false;

            if (Animators["MainMenu"].Speed < 0f)
            Animators["MainMenu"].Speed *= -1f;

            _playButton.Enable(false);
        }

        private void OnSettingsButtonAction(object s, Point e)
        {
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

            _introSong = Assets.GetMusic("boss-theme-intro");
            _mainSong = Assets.GetMusic("boss-theme-main");
            _menuSong = Assets.GetMusic("main-menu");

            MediaPlayer.IsRepeating = true;
            //MediaPlayer.Play(_menuSong);

            LoadSpriterSprite("Graphics/GUI/main-menu");
        }

        protected override void InitializeSpriterGui()
        {
            Animators["MainMenu"].AnimationFinished += MainMenuScreen_AnimationFinished;

            _playButton = new SpriterGuiButton(Game.ViewportAdapter, "PlayButton", "Graphics/GUI/MainMenu/play-button.png", Animators["PlayButton"], Animators["MainMenu"]);
            _settingsButton = new SpriterGuiButton(Game.ViewportAdapter, "SettingsButton", "Graphics/GUI/MainMenu/settings-button.png", Animators["SettingsButton"], Animators["MainMenu"]);

            _playButton.Action += OnPlayButtonAction;
            _settingsButton.Action += OnSettingsButtonAction;

            ResetUI();
        }

        private void ResetUI()
        {
            if (_playButton != null)
            {
                _playButton.Enable(true);
                Game.GuiManager.AddButton(_playButton);
            }

            if (_settingsButton != null)
            {
                _settingsButton.Enable(true);
                Game.GuiManager.AddButton(_settingsButton);
            }

            if (Animators["MainMenu"] != null)
            {
                var previousScreen = Game.ScreenManager.GetPreviousScreen();

                if (previousScreen == null)
                {
                    Animators["MainMenu"].Play("Idle");
                }
                else
                {
                    Animators["MainMenu"].Play("Zoom");
                    Animators["MainMenu"].Progress = 1f;
                    Animators["MainMenu"].Speed *= -1;
                }

                Game.SpriteBatchManager.AddSpriterAnimator(Animators["MainMenu"], Layer.BACKGROUND);
            }

            if (Animators["XmasTitle"] != null)
                Game.SpriteBatchManager.AddSpriterAnimator(Animators["XmasTitle"], Layer.BACKGROUND);
        }

        protected override void LoadSpriterSprite(String spriterFilename)
        {
            base.LoadSpriterSprite(spriterFilename);

            Animators["MainMenu"].SetHiddenTextures(new List<string>(new string[]
            {
                "Graphics/GUI/MainMenu/xmas-title",
            }));
        }

        private void MainMenuScreen_AnimationFinished(string animationName)
        {
            if (animationName == "Zoom")
            {
                if (Animators["MainMenu"].Progress == 1f)
                    Game.ScreenManager.GoTo<BossSelectionScreen>();
            }
        }

        public override void Show(bool reset = false)
        {
            base.Show(reset);

            // Should play music (doesn't seem to work for now...)
            MediaPlayer.Volume = 1f;

            MediaPlayer.MediaStateChanged += MediaPlayerOnMediaStateChanged;
            MediaPlayer.ActiveSongChanged += MediaPlayerOnActiveSongChanged;

            ResetUI();
        }

        public override void Hide()
        {
            base.Hide();

            MediaPlayer.MediaStateChanged -= MediaPlayerOnMediaStateChanged;
            MediaPlayer.ActiveSongChanged -= MediaPlayerOnActiveSongChanged;

            // GUI
            Game.GuiManager.RemoveButton(_settingsButton);
            Game.GuiManager.RemoveButton(_playButton);

            Game.SpriteBatchManager.RemoveSpriterAnimator(Animators["MainMenu"], Layer.BACKGROUND);
            Game.SpriteBatchManager.RemoveSpriterAnimator(Animators["XmasTitle"], Layer.BACKGROUND);
        }

        private void MediaPlayerOnActiveSongChanged(object sender, EventArgs eventArgs)
        {
            //throw new NotImplementedException();
        }

        private void MediaPlayerOnMediaStateChanged(object sender, EventArgs eventArgs)
        {
            if (MediaPlayer.Queue.ActiveSong == null)
                return;

            if (MediaPlayer.Queue.ActiveSong.Name == _introSong.Name &&
                MediaPlayer.Queue.ActiveSong.Position >= _introSong.Duration)
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(_mainSong);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_shootFrequency.TotalMilliseconds < 0)
            {
                Game.GameManager.MoverManager.TriggerPattern(_patternFile, BulletType.Type1, false);
                _shootFrequency = TimeSpan.FromSeconds(1);
            }
            else
            {
                _shootFrequency -= gameTime.ElapsedGameTime;
            }

            var xmasTitleDummyPosition = SpriterUtils.GetSpriterFilePosition("xmas-title.png", Animators["MainMenu"]);
            Animators["XmasTitle"].Position = Game.ViewportAdapter.Center.ToVector2() + xmasTitleDummyPosition;
        }
    }
}