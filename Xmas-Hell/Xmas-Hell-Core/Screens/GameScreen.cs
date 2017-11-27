using System;
using Microsoft.Xna.Framework;
using XmasHell.Controls;
using System.Collections.Generic;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using XmasHell.Spriter;
using XmasHell.Rendering;
using XmasHell.GUI;
using XmasHell.Screens.Menu;
using XnaMediaPlayer = Microsoft.Xna.Framework.Media.MediaPlayer;
using XmasHell.Extensions;
using XmasHell.Audio;
using XmasHell.Background;
using XmasHell.Entities.Bosses;

namespace XmasHell.Screens
{
    public class GameScreen : Screen
    {
        private bool _endGamePopupOpened = false;

        // GUI
        private Dictionary<string, CustomSpriterAnimator> _spriterFile;
        private List<SpriterGuiButton> _endGamePanelButtons = new List<SpriterGuiButton>();
        private AbstractGuiLabel _timerLabel;
        private AbstractGuiLabel _timerLabelShadow;
        private Sprite _hpBar;

        // Labels
        private List<SpriterGuiLabel> _endGamePanelLabels = new List<SpriterGuiLabel>();
        private SpriterGuiLabel _endGameTitleLabel;
        private SpriterGuiLabel _endGameDeathCounterLabel;
        private List<SpriterGuiLabel> _endGameTauntLabels;

        private static List<String> _tauntStrings = new List<string>
        { 
            "I expected more from you...",
            "Protip: drag your finger around the screen to move.",
            "My granny could scrap better than that.",
            "Sorry, no in app-purchases to help you here!",
            "What about trying this one tomorrow?",
            "Collect your opponent's bullet won't make your score better...",
            "That was quick!",
            "It's easier to play using your finger.",
            "Please, stop hurting yourself."
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
            _timerLabelShadow = new AbstractGuiLabel("00:00:00", Assets.GetFont("Graphics/Fonts/ui-small"), new Vector2(_timerLabel.Position.X + 1, _timerLabel.Position.Y + 1), Color.Black, true);

            _hpBar = new Sprite(
                new TextureRegion2D(
                    Assets.GetTexture2D("pixel"),
                    0, 0, GameConfig.VirtualResolution.X, 10
                )
            )
            {
                Origin = Vector2.Zero,
                Color = Color.Red
            };
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
            _endGameTitleLabel = new SpriterGuiLabel("", Assets.GetFont("Graphics/Fonts/ui-title"), "end-game-panel-title-label.png", _spriterFile["EndGamePanel"], Vector2.Zero, true);
            _endGameDeathCounterLabel = new SpriterGuiLabel("", Assets.GetFont("Graphics/Fonts/ui"), "end-game-panel-player-deaths-label.png", _spriterFile["EndGamePanel"], Vector2.Zero, true);
            _endGameTauntLabels = new List<SpriterGuiLabel>();

            _endGamePanelLabels.Add(_endGameTitleLabel);
            _endGamePanelLabels.Add(_endGameDeathCounterLabel);
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

        private void OpenEndGamePopup(bool won)
        {
            if (_endGamePopupOpened)
                return;

            _endGamePopupOpened = true;
            Game.SpriteBatchManager.AddSpriterAnimator(_spriterFile["EndGamePanel"], Layer.UI);
            _spriterFile["EndGamePanel"].Play("Show");

            var font = Assets.GetFont("Graphics/Fonts/ui");

            // GUI
            if (won)
            {
                _endGameTitleLabel.Text = "You won!";
                _endGameDeathCounterLabel.Text = "Time: " + Game.GameManager.GetCurrentTime().ToString("mm\\:ss\\'fff");

                _endGameTauntLabels.Add(new SpriterGuiLabel("", font, "end-game-panel-taunt-label.png", _spriterFile["EndGamePanel"], Vector2.Zero));
            }
            else
            {
                _endGameTitleLabel.Text = "You died!";

                _endGameDeathCounterLabel.Text =
                    Game.PlayerData.DeathCounter(Game.GameManager.GetCurrentBoss().BossType) + " times already";

                var randomTaunt = GetRandomTauntString();
                var tauntStrings = StringExtension.FormatBoxString(randomTaunt, 260, font);

                for (var i = 0; i < tauntStrings.Count; i++)
                {
                    var relativePosition = new Vector2(0, i * font.MeasureString(tauntStrings[i]).Height);
                    var label = new SpriterGuiLabel(tauntStrings[i], font,
                        "end-game-panel-taunt-label.png", _spriterFile["EndGamePanel"], relativePosition, true);

                    _endGameTauntLabels.Add(label);
                    _endGamePanelLabels.Add(label);
                }
            }

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

            foreach (var endGameTauntLabel in _endGameTauntLabels)
                _endGamePanelLabels.Remove(endGameTauntLabel);

            _endGameTauntLabels.Clear();

            _spriterFile["EndGamePanel"].Play("Hide");
            Game.SpriteBatchManager.RemoveSpriterAnimator(_spriterFile["EndGamePanel"], Layer.UI);
        }

        public override void Show(bool reset = false)
        {
            base.Show(reset);

            Game.GameManager.StartNewGame();

            Game.SpriteBatchManager.UILabels.Add(_timerLabelShadow);
            Game.SpriteBatchManager.UILabels.Add(_timerLabel);
            Game.SpriteBatchManager.UISprites.Add(_hpBar);

            var level = BossFactory.BossTypeToBackgroundLevel(Game.GameManager.GetCurrentBoss().BossType);
            ((GradientBackground)Game.SpriteBatchManager.Background).ChangeGradientColors(GameConfig.BackgroundGradients[level].Item1, GameConfig.BackgroundGradients[level].Item2);

            Game.MusicManager.PlayGameMusic(true);
        }

        public override void Hide()
        {
            base.Hide();

            Game.GameManager.Dispose();
            CloseEndGamePopup();
            Game.SpriteBatchManager.UILabels.Remove(_timerLabel);
            Game.SpriteBatchManager.UILabels.Remove(_timerLabelShadow);
            Game.SpriteBatchManager.UISprites.Remove(_hpBar);

            var level = BackgroundLevel.Level1;
            ((GradientBackground)Game.SpriteBatchManager.Background).ChangeGradientColors(GameConfig.BackgroundGradients[level].Item1, GameConfig.BackgroundGradients[level].Item2);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputManager.PressedCancel())
                Game.ScreenManager.GoTo<BossSelectionScreen>();

            if (Game.GameManager.GameIsFinished() && !_endGamePopupOpened)
                OpenEndGamePopup(Game.GameManager.Won());

            if (!Game.GameManager.GameIsFinished())
            {
                _timerLabel.Text = Game.GameManager.GetCurrentTime().ToString("mm\\:ss\\:ff");
                _timerLabelShadow.Text = _timerLabel.Text;

                var currentBoss = Game.GameManager.GetCurrentBoss();

                if (currentBoss != null)
                {
                    _hpBar.Scale = new Vector2(currentBoss.GetLifePercentage(), 1);
                    _hpBar.Color = currentBoss.Tinted ? Color.White : GameConfig.BossHPBarColors[currentBoss.CurrentBehaviourIndex];
                }
            }
        }

        private String GetRandomTauntString()
        {
            return _tauntStrings[Game.GameManager.Random.Next(_tauntStrings.Count)];
        }
    }
}