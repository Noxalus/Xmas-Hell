using System;
using Microsoft.Xna.Framework;
using XmasHell.Controls;
using System.Collections.Generic;
using XmasHell.Spriter;
using XmasHell.Rendering;
using XmasHell.GUI;

namespace XmasHell.Screens
{
    public class GameScreen : Screen
    {
        private bool _endGamePopupOpened = false;

        // GUI
        private Dictionary<string, CustomSpriterAnimator> _spriterFile;
        private List<SpriterGuiButton> _endGamePanelButtons = new List<SpriterGuiButton>();
        private AbstractGuiLabel _timerLabel;

        // Labels
        private List<SpriterGuiLabel> _endGamePanelLabels = new List<SpriterGuiLabel>();
        private SpriterGuiLabel _endGameTitleLabel;
        private SpriterGuiLabel _endGameDeathCounterLabel;
        private SpriterGuiLabel _endGameTauntLabel;

        private static List<String> _tauntStrings = new List<string>()
        {
            "You suck!",
            "Your goal is to avoid your opponent's\n bullet, not to collect them!",
            "Drag your finger over\n the screen to move",
            "Sorry, there is no in \napp-purchases to help you here"
        };

        private float GetRank()
        {
            return 1f;
        }

        public GameScreen(XmasHell game) : base(game)
        {
            ShouldBeStackInHistory = true;
            GameManager.GameDifficulty = GetRank;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            _spriterFile = Assets.GetSpriterAnimators("Graphics/GUI/game-screen");
            InitializeSpriterGui();

            _timerLabel = new AbstractGuiLabel("00:00:00", Assets.GetFont("Graphics/Fonts/ui-small"), new Vector2(Game.ViewportAdapter.VirtualWidth - 95, 30), Color.White, true);
        }

        private void InitializeSpriterGui()
        {
            _spriterFile["EndGamePanel"].AnimationFinished += EndGamePanel_AnimationFinished;

            // End game panel buttons
            var closePanelButton = new SpriterGuiButton(
                Game.ViewportAdapter, "CloseBossPanel", "Graphics/GUI/GameScreen/game-panel-close-button.png",
                _spriterFile["CloseButton"], _spriterFile["EndGamePanel"]
            );

            var retryPanelButton = new SpriterGuiButton(
                Game.ViewportAdapter, "StartBattleBossPanel", "Graphics/GUI/GameScreen/game-panel-retry-button.png",
                _spriterFile["RetryButton"], _spriterFile["EndGamePanel"]
            );

            closePanelButton.Action += EndGamePanelCloseButtonAction;
            retryPanelButton.Action += EndGamePanelRetryButtonAction;

            closePanelButton.Animator().zIndex(11);
            retryPanelButton.Animator().zIndex(11);

            _endGamePanelButtons.Add(closePanelButton);
            _endGamePanelButtons.Add(retryPanelButton);

            // GUI
            _endGameTitleLabel = new SpriterGuiLabel("You died!", Assets.GetFont("Graphics/Fonts/ui-title"), "end-game-panel-title-label.png", _spriterFile["EndGamePanel"], true);
            _endGameDeathCounterLabel = new SpriterGuiLabel("", Assets.GetFont("Graphics/Fonts/ui"), "end-game-panel-player-deaths-label.png", _spriterFile["EndGamePanel"], true);
            _endGameTauntLabel = new SpriterGuiLabel("", Assets.GetFont("Graphics/Fonts/ui"), "end-game-panel-taunt-label.png", _spriterFile["EndGamePanel"], true);

            _endGamePanelLabels.Add(_endGameTitleLabel);
            _endGamePanelLabels.Add(_endGameDeathCounterLabel);
            _endGamePanelLabels.Add(_endGameTauntLabel);
        }

        #region Animations finished
        private void EndGamePanel_AnimationFinished(string animationName)
        {
            if (animationName == "Show")
                _spriterFile["EndGamePanel"].Play("Idle");
            else if (animationName == "Hide")
                CloseEndGamePopup();
        }
        #endregion

        #region Button actions
        private void EndGamePanelCloseButtonAction(object button, Point e)
        {
            Game.ScreenManager.GoTo<BossSelectionScreen>();
        }

        private void EndGamePanelRetryButtonAction(object button, Point e)
        {
            // Reset game
            Game.GameManager.Reset();

            CloseEndGamePopup();
        }
        #endregion

        private void OpenEndGamePopup()
        {
            if (_endGamePopupOpened)
                return;

            _endGamePopupOpened = true;
            Game.SpriteBatchManager.AddSpriterAnimator(_spriterFile["EndGamePanel"], Layer.UI);
            _spriterFile["EndGamePanel"].Play("Show");

            // GUI
            _endGameDeathCounterLabel.Text = Game.PlayerData.BossAttempts(Game.GameManager.GetCurrentBoss().BossType) + " times already";
            _endGameTauntLabel.Text = GetRandomTauntString();

            foreach (var button in _endGamePanelButtons)
                Game.GuiManager.AddButton(button);

            foreach (var label in _endGamePanelLabels)
                Game.GuiManager.AddLabel(label);
        }

        private void CloseEndGamePopup()
        {
            _endGamePopupOpened = false;

            foreach (var button in _endGamePanelButtons)
                Game.GuiManager.RemoveButton(button);

            foreach (var label in _endGamePanelLabels)
                Game.GuiManager.RemoveLabel(label);

            _spriterFile["EndGamePanel"].Play("Hide");
            Game.SpriteBatchManager.RemoveSpriterAnimator(_spriterFile["EndGamePanel"], Layer.UI);
        }

        public override void Show(bool reset = false)
        {
            base.Show(reset);

            Game.GameManager.StartNewGame();

            Game.SpriteBatchManager.UILabels.Add(_timerLabel);

            // Should play music (doesn't seem to work for now...)
            //MediaPlayer.Volume = 1f;
            //MediaPlayer.IsRepeating = true;
            //MediaPlayer.Play(Assets.GetMusic("Audio/BGM/boss-theme"));
        }

        public override void Hide()
        {
            base.Hide();

            Game.GameManager.Dispose();
            CloseEndGamePopup();
            Game.SpriteBatchManager.UILabels.Remove(_timerLabel);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputManager.PressedCancel())
                Game.ScreenManager.GoTo<BossSelectionScreen>();

            if (Game.GameManager.GameIsFinished() && !_endGamePopupOpened)
                OpenEndGamePopup();

            if (!Game.GameManager.GameIsFinished())
                _timerLabel.Text = Game.GameManager.GetCurrentTime().ToString("mm\\:ss\\:ff");
        }

        private String GetRandomTauntString()
        {
            return _tauntStrings[Game.GameManager.Random.Next(_tauntStrings.Count)];
        }
    }
}