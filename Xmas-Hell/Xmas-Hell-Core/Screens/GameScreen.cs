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

            foreach (var button in _endGamePanelButtons)
                Game.GuiManager.AddButton(button);
        }

        private void CloseEndGamePopup()
        {
            _endGamePopupOpened = false;

            foreach (var button in _endGamePanelButtons)
                Game.GuiManager.RemoveButton(button);

            _spriterFile["EndGamePanel"].Play("Hide");
            Game.SpriteBatchManager.RemoveSpriterAnimator(_spriterFile["EndGamePanel"], Layer.UI);
        }

        // TODO: This should be handled by the ScreenManager
        public override void Show(bool reset = false)
        {
            base.Show(reset);

            Game.GameManager.StartNewGame();

            // Should play music (doesn't seem to work for now...)
            //MediaPlayer.Volume = 1f;
            //MediaPlayer.IsRepeating = true;
            //MediaPlayer.Play(Assets.GetMusic("Audio/BGM/boss-theme"));
        }

        public override void Hide()
        {
            base.Hide();

            Game.GameManager.Clear();
            CloseEndGamePopup();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputManager.PressedCancel())
                Game.ScreenManager.GoTo<BossSelectionScreen>();

            if (Game.GameManager.GameIsFinished() && !_endGamePopupOpened)
                OpenEndGamePopup();
        }
    }
}