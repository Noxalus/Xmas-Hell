using System;
using BulletML;
using Microsoft.Xna.Framework;
using XmasHell.BulletML;
using XmasHell.GUI;
using XmasHell.Spriter;
using XmasHell.Rendering;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using XnaMediaPlayer = Microsoft.Xna.Framework.Media.MediaPlayer;

namespace XmasHell.Screens.Menu
{
    public class MainMenuScreen : Screen
    {
        private string _patternFile = "MainMenu/snowflake";
        private TimeSpan _shootFrequency;

        private Song _introSong;
        private Song _mainSong;
        private Song _menuSong;

        private Dictionary<string, CustomSpriterAnimator> _mainMenuSpriterFile;

        private List<SpriterGuiButton> _menuButtons = new List<SpriterGuiButton>();
        private SpriterGuiButton _playButton;
        private SpriterGuiButton _settingsButton;
        private SpriterGuiButton _achievementsButton;
        private SpriterGuiButton _leaderboardsButton;

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
            _mainMenuSpriterFile["MainMenu"].Play("Zoom");
            _mainMenuSpriterFile["MainMenu"].CurrentAnimation.Looping = false;

            if (_mainMenuSpriterFile["MainMenu"].Speed < 0f)
            _mainMenuSpriterFile["MainMenu"].Speed *= -1f;

            _playButton.Enable(false);
        }

        private void OnSettingsButtonAction(object s, Point e)
        {
        }

        private void OnAchievementsButtonAction(object s, Point e)
        {
#if ANDROID
            Game.AndroidActivity.ShowAchievements();
#endif
        }

        private void OnLeaderboardsButtonAction(object s, Point e)
        {
#if ANDROID
            Game.AndroidActivity.ShowLeaderboards();
#endif
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

            XnaMediaPlayer.IsRepeating = true;
            XnaMediaPlayer.Play(_menuSong);

            _mainMenuSpriterFile = Assets.GetSpriterAnimators("Graphics/GUI/main-menu");
            //LoadSpriterSprite("Graphics/GUI/main-menu");
            InitializeSpriterGui();
        }

        private void InitializeSpriterGui()
        {
            _mainMenuSpriterFile["MainMenu"].AnimationFinished += MainMenuScreen_AnimationFinished;

            _playButton = new SpriterGuiButton(Game.ViewportAdapter, "PlayButton", "Graphics/GUI/MainMenu/play-button.png", _mainMenuSpriterFile["PlayButton"], _mainMenuSpriterFile["MainMenu"]);
            _settingsButton = new SpriterGuiButton(Game.ViewportAdapter, "SettingsButton", "Graphics/GUI/MainMenu/settings-button.png", _mainMenuSpriterFile["SettingsButton"], _mainMenuSpriterFile["MainMenu"]);
            _achievementsButton = new SpriterGuiButton(Game.ViewportAdapter, "AchievementsButton", "Graphics/GUI/MainMenu/achievement-button.png", _mainMenuSpriterFile["AchievementsButton"], _mainMenuSpriterFile["MainMenu"]);
            _leaderboardsButton = new SpriterGuiButton(Game.ViewportAdapter, "LeaderboardsButton", "Graphics/GUI/MainMenu/leaderboard-button.png", _mainMenuSpriterFile["LeaderboardsButton"], _mainMenuSpriterFile["MainMenu"]);

            _playButton.Action += OnPlayButtonAction;
            _settingsButton.Action += OnSettingsButtonAction;
            _achievementsButton.Action += OnAchievementsButtonAction;
            _leaderboardsButton.Action += OnLeaderboardsButtonAction;

            _menuButtons.Add(_playButton);
            _menuButtons.Add(_settingsButton);
            _menuButtons.Add(_achievementsButton);
            _menuButtons.Add(_leaderboardsButton);

            _mainMenuSpriterFile["MainMenu"].AddHiddenTexture("Graphics/GUI/MainMenu/xmas-title");

            ResetUI();
        }

        private void ResetUI()
        {
            if (UIReseted)
                return;

            foreach (var button in _menuButtons)
            {
                button.Enable(true);
                Game.GuiManager.AddButton(button);
            }

            if (_mainMenuSpriterFile["MainMenu"] != null)
            {
                var previousScreen = Game.ScreenManager.GetPreviousScreen();

                if (previousScreen == null)
                {
                    _mainMenuSpriterFile["MainMenu"].Play("Idle");
                }
                else
                {
                    _mainMenuSpriterFile["MainMenu"].Play("Zoom");
                    _mainMenuSpriterFile["MainMenu"].Progress = 1f;
                    _mainMenuSpriterFile["MainMenu"].Speed *= -1;
                }

                Game.SpriteBatchManager.AddSpriterAnimator(_mainMenuSpriterFile["MainMenu"], Layer.BACKGROUND);
            }

            if (_mainMenuSpriterFile["XmasTitle"] != null)
                Game.SpriteBatchManager.AddSpriterAnimator(_mainMenuSpriterFile["XmasTitle"], Layer.BACKGROUND);

            UIReseted = true;
        }

        private void MainMenuScreen_AnimationFinished(string animationName)
        {
            if (animationName == "Zoom")
            {
                if (_mainMenuSpriterFile["MainMenu"].Progress == 1f)
                    Game.ScreenManager.GoTo<BossSelectionScreen>();
            }
        }

        public override void Show(bool reset = false)
        {
            base.Show(reset);

            // Should play music (doesn't seem to work for now...)
            XnaMediaPlayer.Volume = 1f;

            XnaMediaPlayer.MediaStateChanged += MediaPlayerOnMediaStateChanged;
            XnaMediaPlayer.ActiveSongChanged += MediaPlayerOnActiveSongChanged;

            ResetUI();
        }

        public override void Hide()
        {
            base.Hide();

            XnaMediaPlayer.MediaStateChanged -= MediaPlayerOnMediaStateChanged;
            XnaMediaPlayer.ActiveSongChanged -= MediaPlayerOnActiveSongChanged;

            // GUI
            Game.SpriteBatchManager.RemoveSpriterAnimator(_mainMenuSpriterFile["MainMenu"], Layer.BACKGROUND);
            Game.SpriteBatchManager.RemoveSpriterAnimator(_mainMenuSpriterFile["XmasTitle"], Layer.BACKGROUND);

            foreach (var button in _menuButtons)
            {
                button.Enable(false);
                Game.GuiManager.RemoveButton(button);
            }
        }

        private void MediaPlayerOnActiveSongChanged(object sender, EventArgs eventArgs)
        {
            //throw new NotImplementedException();
        }

        private void MediaPlayerOnMediaStateChanged(object sender, EventArgs eventArgs)
        {
            //if (XnaMediaPlayer.Queue.ActiveSong == null)
            //    return;

            //if (XnaMediaPlayer.Queue.ActiveSong.Name == _introSong.Name &&
            //    XnaMediaPlayer.Queue.ActiveSong.Position >= _introSong.Duration)
            //{
            //    XnaMediaPlayer.IsRepeating = true;
            //    XnaMediaPlayer.Play(_mainSong);
            //}
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

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

            var xmasTitleDummyPosition = SpriterUtils.GetSpriterFilePosition("xmas-title.png", _mainMenuSpriterFile["MainMenu"]);
            _mainMenuSpriterFile["XmasTitle"].Position = Game.ViewportAdapter.Center.ToVector2() + xmasTitleDummyPosition;
        }
    }
}