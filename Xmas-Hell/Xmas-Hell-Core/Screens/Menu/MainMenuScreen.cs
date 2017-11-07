using Microsoft.Xna.Framework;
using XmasHell.GUI;
using XmasHell.Spriter;
using XmasHell.Rendering;
using System.Collections.Generic;

namespace XmasHell.Screens.Menu
{
    public class MainMenuScreen : MenuScreen
    {
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
            base.Initialize();
        }

        private void OnPlayButtonAction(object button, Point position)
        {
            SpriterFile["Main"].Play("Zoom");
            SpriterFile["Main"].CurrentAnimation.Looping = false;

            if (SpriterFile["Main"].Speed < 0f)
            SpriterFile["Main"].Speed *= -1f;
        }

        private void OnSettingsButtonAction(object button, Point position)
        {
            // TODO: Add a SettingsScreen
        }

        private void OnAchievementsButtonAction(object button, Point position)
        {
#if ANDROID
            Game.AndroidActivity.ShowAchievements();
#endif
        }

        private void OnLeaderboardsButtonAction(object button, Point position)
        {
#if ANDROID
            Game.AndroidActivity.ShowLeaderboards();
#endif
        }

        public override void LoadContent()
        {
            base.LoadContent();

            SpriterFile = Assets.GetSpriterAnimators("Graphics/GUI/main-menu");
            //LoadSpriterSprite("Graphics/GUI/main-menu");
            InitializeSpriterGui();
        }

        private void InitializeSpriterGui()
        {
            SpriterFile["Main"].AnimationFinished += MainMenuScreen_AnimationFinished;

            _playButton = new SpriterGuiButton(Game.ViewportAdapter, "PlayButton", "Graphics/GUI/MainMenu/play-button.png", SpriterFile["PlayButton"], SpriterFile["Main"]);
            _settingsButton = new SpriterGuiButton(Game.ViewportAdapter, "SettingsButton", "Graphics/GUI/MainMenu/settings-button.png", SpriterFile["SettingsButton"], SpriterFile["Main"]);
            _achievementsButton = new SpriterGuiButton(Game.ViewportAdapter, "AchievementsButton", "Graphics/GUI/MainMenu/achievement-button.png", SpriterFile["AchievementsButton"], SpriterFile["Main"]);
            _leaderboardsButton = new SpriterGuiButton(Game.ViewportAdapter, "LeaderboardsButton", "Graphics/GUI/MainMenu/leaderboard-button.png", SpriterFile["LeaderboardsButton"], SpriterFile["Main"]);

            _playButton.Action += OnPlayButtonAction;
            _settingsButton.Action += OnSettingsButtonAction;
            _achievementsButton.Action += OnAchievementsButtonAction;
            _leaderboardsButton.Action += OnLeaderboardsButtonAction;

            _menuButtons.Add(_playButton);
            _menuButtons.Add(_settingsButton);
            _menuButtons.Add(_achievementsButton);
            _menuButtons.Add(_leaderboardsButton);

            SpriterFile["Main"].AddHiddenTexture("Graphics/GUI/MainMenu/xmas-title");

            ResetUI();
        }

        protected override void ResetUI()
        {
            base.ResetUI();

            if (UIReseted)
                return;

            foreach (var button in _menuButtons)
            {
                button.Enable(true);
                Game.GuiManager.AddButton(button);
            }

            if (SpriterFile["Main"] != null)
            {
                var previousScreen = Game.ScreenManager.GetPreviousScreen();

                if (previousScreen == null)
                {
                    SpriterFile["Main"].Play("Idle");
                }
                else
                {
                    SpriterFile["Main"].Play("Zoom");
                    SpriterFile["Main"].Progress = 1f;
                    SpriterFile["Main"].Speed *= -1;
                }

                Game.SpriteBatchManager.AddSpriterAnimator(SpriterFile["Main"], Layer.BACKGROUND);
            }

            if (SpriterFile["XmasTitle"] != null)
                Game.SpriteBatchManager.AddSpriterAnimator(SpriterFile["XmasTitle"], Layer.BACKGROUND);

            UIReseted = true;
        }

        private void MainMenuScreen_AnimationFinished(string animationName)
        {
            if (animationName == "Zoom")
            {
                if (SpriterFile["Main"].Progress == 1f)
                    Game.ScreenManager.GoTo<BossSelectionScreen>();
            }
        }

        public override void Show(bool reset = false)
        {
            base.Show(reset);

            ResetUI();
        }

        public override void Hide()
        {
            base.Hide();

            // GUI
            Game.SpriteBatchManager.RemoveSpriterAnimator(SpriterFile["Main"], Layer.BACKGROUND);
            Game.SpriteBatchManager.RemoveSpriterAnimator(SpriterFile["XmasTitle"], Layer.BACKGROUND);

            foreach (var button in _menuButtons)
            {
                button.Enable(false);
                Game.GuiManager.RemoveButton(button);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var xmasTitleDummyPosition = SpriterUtils.GetSpriterFilePosition("xmas-title.png", SpriterFile["Main"]);
            SpriterFile["XmasTitle"].Position = Game.ViewportAdapter.Center.ToVector2() + xmasTitleDummyPosition;
        }
    }
}