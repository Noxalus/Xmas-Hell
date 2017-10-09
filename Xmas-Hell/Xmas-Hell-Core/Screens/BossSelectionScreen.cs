using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Xmas_Hell_Core.Controls;
using XmasHell.Entities.Bosses;
using XmasHell.GUI;
using XmasHell.Rendering;
using XmasHell.Spriter;

namespace XmasHell.Screens
{
    public class BossSelectionScreen : Screen
    {
        #region Fields
        private BossType _selectedBoss;
        private List<SpriterSubstituteEntity> _bossGarlands = new List<SpriterSubstituteEntity>();
        private bool _uiReseted = false;

        private readonly List<string> _bossNames = new List<string>()
        {
            "ball", "bell", "snowflake", "candy", "gift", "log", "tree", "reindeer", "snowman", "santa"
        };

        private readonly Dictionary<string, Tuple<string, string>> _bossRelations = new Dictionary<string, Tuple<string, string>>
        {
            { "santa", new Tuple<string, string>("reindeer", "snowman") },
            { "reindeer", new Tuple<string, string>("gift", "log") },
            { "snowman", new Tuple<string, string>("log", "tree") },
            { "gift", new Tuple<string, string>("bell", "candy") },
            { "log", new Tuple<string, string>("candy", "ball") },
            { "tree", new Tuple<string, string>("ball", "snowflake") }
        };

        // GUI
        private Dictionary<BossType, SpriterGuiButton> _bossButtons = new Dictionary<BossType, SpriterGuiButton>();
        private List<SpriterGuiButton> _bossPanelButtons = new List<SpriterGuiButton>();
        private SpriterGuiButton _closeBossPanelButton;
        private SpriterGuiButton _startBattleBossPanelButton;

        // Labels
        private List<SpriterGuiLabel> _bossPanelLabels = new List<SpriterGuiLabel>();
        private SpriterGuiLabel _bossNameLabel;
        private SpriterGuiLabel _bestTimeLabel;
        private SpriterGuiLabel _playTimeLabel;
        private SpriterGuiLabel _playerDeathLabel;
        private SpriterGuiLabel _bossDeathLabel;

        #endregion

        public BossSelectionScreen(XmasHell game) : base(game)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            LoadSpriterSprite("Graphics/GUI/boss-selection");
        }

        protected override void InitializeSpriterGui()
        {
            Animators["BossSelection"].AnimationFinished += BossSelectionScreen_AnimationFinished;
            Animators["BossPanel"].AnimationFinished += BossPanel_AnimationFinished;

            Animators["Ball"].zIndex(9);
            Animators["Garland"].zIndex(5);
            Animators["BossPanel"].zIndex(10);

            // Christmas tree's balls
            foreach (var bossName in _bossNames)
            {
                var ballAnimator = Animators["Ball"].Clone();
                ballAnimator.Play("Balance");
                ballAnimator.Progress = (float)Game.GameManager.Random.NextDouble();
                ballAnimator.Speed = 0.5f + (float)Game.GameManager.Random.NextDouble();

                var hasRelation = _bossRelations.ContainsKey(bossName);

                if (hasRelation)
                {
                    _bossGarlands.Add(new SpriterSubstituteEntity(
                        "xmas-" + _bossRelations[bossName].Item1 + "-" + bossName + "-garland.png",
                        Animators["BossSelection"], Animators["Garland"].Clone()
                    ));

                    _bossGarlands.Add(new SpriterSubstituteEntity(
                        "xmas-" + _bossRelations[bossName].Item2 + "-" + bossName + "-garland.png",
                        Animators["BossSelection"], Animators["Garland"].Clone()
                    ));
                }

                var bossButton = new SpriterGuiButton(
                    Game.ViewportAdapter, bossName, "Graphics/GUI/BossSelection/xmas-" + bossName  + "-available-button.png",
                    ballAnimator, Animators["BossSelection"]
                );

                bossButton.Action += OnBossButtonAction;
                _bossButtons.Add(BossFactory.StringToBossType(bossName), bossButton);
            }

            // Boss panel buttons
            _closeBossPanelButton = new SpriterGuiButton(
                Game.ViewportAdapter, "CloseBossPanel", "Graphics/GUI/BossSelection/boss-panel-close-button.png",
                Animators["CloseButton"], Animators["BossPanel"]
            );

            _startBattleBossPanelButton = new SpriterGuiButton(
                Game.ViewportAdapter, "StartBattleBossPanel", "Graphics/GUI/BossSelection/boss-panel-battle-button.png",
                Animators["BattleButton"], Animators["BossPanel"]
            );

            _closeBossPanelButton.Action += BossPanelCloseButtonAction;
            _startBattleBossPanelButton.Action += BossPanelStartBattleButtonAction;

            _closeBossPanelButton.Animator().zIndex(11);
            _startBattleBossPanelButton.Animator().zIndex(11);

            _bossPanelButtons.Add(_closeBossPanelButton);
            _bossPanelButtons.Add(_startBattleBossPanelButton);

            // Labels
            _bossNameLabel = new SpriterGuiLabel("Unknown", "boss-panel-title-label.png", Animators["BossPanel"]);
            //_bestTimeLabel = new SpriterGuiLabel("Best time: ", Vector2.Zero, Color.Black);
            //_playTimeLabel = new SpriterGuiLabel("Play time: ", Vector2.Zero, Color.Black);
            //_playerDeathLabel = new SpriterGuiLabel("Player deaths: ", Vector2.Zero, Color.Black);
            //_bossDeathLabel = new SpriterGuiLabel("Boss deaths: ", Vector2.Zero, Color.Black);

            _bossPanelLabels.Add(_bossNameLabel);
            //_bossPanelLabels.Add(_bestTimeLabel);
            //_bossPanelLabels.Add(_playTimeLabel);
            //_bossPanelLabels.Add(_playerDeathLabel);
            //_bossPanelLabels.Add(_bossDeathLabel);

            base.InitializeSpriterGui();

            ResetUI();
        }

        #region Button actions
        private void OnBossButtonAction(object button, Point position)
        {
            var spriterGuiButton = button as SpriterGuiButton;
            var bossType = BossFactory.StringToBossType(spriterGuiButton.Name);

            if (bossType != BossType.Unknown)
                OpenBossPanel(bossType);
        }

        private void BossPanelCloseButtonAction(object button, Point e)
        {
            CloseBossPanel();
        }

        private void BossPanelStartBattleButtonAction(object button, Point e)
        {
            Game.ScreenManager.GetScreen<GameScreen>().LoadBoss(_selectedBoss);
            Game.ScreenManager.GoTo<GameScreen>();
        }
        #endregion

        #region Animations finished
        private void BossSelectionScreen_AnimationFinished(string animationName)
        {
            if (animationName == "Intro")
            {
                Animators["BossSelection"].Play("Idle");
            }
        }

        private void BossPanel_AnimationFinished(string animationName)
        {
            if (animationName == "Show")
                Animators["BossPanel"].Play("Idle");
            else if (animationName == "Hide")
                DoCloseBossPanel();
        }
        #endregion

        private void OpenBossPanel(BossType bossType)
        {
            _selectedBoss = bossType;

            var spriterGuiButton = _bossButtons[bossType];
            spriterGuiButton.SubstituteEntity.EnableSynchronization(false);
            var ballPosition = Game.ViewportAdapter.Center.ToVector2();
            ballPosition.Y -= 575;
            spriterGuiButton.Animator().Position = ballPosition;
            spriterGuiButton.Animator().Scale = new Vector2(2f);
            spriterGuiButton.Animator().zIndex(11);

            foreach (var bossPanelButton in _bossPanelButtons)
                Game.GuiManager.AddButton(bossPanelButton);

            foreach (var bossPanelLabel in _bossPanelLabels)
                Game.GuiManager.AddLabel(bossPanelLabel);

            Game.SpriteBatchManager.AddSpriterAnimator(Animators["BossPanel"], Layer.UI);
            Animators["BossPanel"].Play("Show");

            // Update labels
            _bossNameLabel.Text = "Xmas " + bossType.ToString().Substring(4);

            DoOpenBossPanel();
        }

        // Performed when animation is finished
        private void DoOpenBossPanel()
        {
            DisableBossButtons();
        }

        // Performed when animation is finished
        private void DoCloseBossPanel()
        {
            foreach (var bossPanelButton in _bossPanelButtons)
                Game.GuiManager.RemoveButton(bossPanelButton);

            foreach (var bossPanelLabel in _bossPanelLabels)
                Game.GuiManager.RemoveLabel(bossPanelLabel);

            Game.SpriteBatchManager.RemoveSpriterAnimator(Animators["BossPanel"], Layer.UI);
            EnableBossButtons();
        }

        private void CloseBossPanel(bool hardClose = false)
        {
            if (_bossButtons.ContainsKey(_selectedBoss))
            {
                _bossButtons[_selectedBoss].SubstituteEntity.EnableSynchronization(true);
                _bossButtons[_selectedBoss].Animator().zIndex(9);
            }

            if (hardClose)
            {
                DoCloseBossPanel();
            }
            else
            {
                Animators["BossPanel"].Play("Hide");
                Animators["BossPanel"].CurrentAnimation.Looping = false;
            }
        }

        public override void Show(bool reset = false)
        {
            base.Show(reset);

            ResetUI();
        }

        private void ShowBossButtons()
        {
            foreach (var bossButtonPair in _bossButtons)
            {
                var bossButton = bossButtonPair.Value;

                bossButton.SubstituteEntity.EnableSynchronization(true);

                var hasRelation = _bossRelations.ContainsKey(bossButton.Name);

                var available = !hasRelation ||
                    (Game.PlayerData.BossBeatenCounter(BossFactory.StringToBossType(_bossRelations[bossButton.Name].Item1)) > 0 &&
                    Game.PlayerData.BossBeatenCounter(BossFactory.StringToBossType(_bossRelations[bossButton.Name].Item2)) > 0);

                var hidden = Game.PlayerData.BossAttempts(BossFactory.StringToBossType(bossButton.Name)) == 0;

                if (!hasRelation || (available && !hidden))
                {
                    var beaten = Game.PlayerData.BossBeatenCounter(BossFactory.StringToBossType(bossButton.Name)) > 0;

                    bossButton.Animator().AddTextureSwap(
                        "Graphics/GUI/BossSelection/unknown-boss-button",
                        Assets.GetTexture2D("Graphics/GUI/BossSelection/xmas-" + bossButton.Name + "-" + ((beaten) ? "beaten" : "available") + "-button")
                    );
                }
                else
                {
                    bossButton.Name = "Unknown";
                }

                Game.GuiManager.AddButton(bossButton);
            }

            EnableBossButtons();
        }

        private void ResetUI()
        {
            if (!SpriterGuiInitialized || _uiReseted)
                return;

            if (Animators["BossSelection"] != null)
            {
                if (Game.ScreenManager.GetPreviousScreen().Name == "MainMenuScreen")
                {
                    Animators["BossSelection"].Play("Intro");
                }
                else if (Game.ScreenManager.GetPreviousScreen().Name == "GameScreen")
                {
                    Animators["BossSelection"].Play("Idle");
                }

                Game.SpriteBatchManager.AddSpriterAnimator(Animators["BossSelection"], Layer.BACKGROUND);
                Animators["BossSelection"].CurrentAnimation.Looping = false;
            }

            foreach (var garland in _bossGarlands)
                Game.SpriteBatchManager.AddSpriterAnimator(garland.SubstituteAnimator, Layer.UI);

            ShowBossButtons();
            _uiReseted = true;
        }

        private void EnableBossButtons()
        {
            foreach (var bossButtonPair in _bossButtons)
            {
                var bossButton = bossButtonPair.Value;
                bossButton.Enable(true);
            }
        }

        private void DisableBossButtons()
        {
            foreach (var bossButtonPair in _bossButtons)
            {
                var bossButton = bossButtonPair.Value;
                bossButton.Enable(false);
            }
        }

        private void HideBossButtons()
        {
            foreach (var bossButtonPair in _bossButtons)
            {
                var bossButton = bossButtonPair.Value;
                Game.GuiManager.RemoveButton(bossButton);
            }

            DisableBossButtons();
        }

        public override void Hide()
        {
            base.Hide();

            foreach (var garland in _bossGarlands)
                Game.SpriteBatchManager.RemoveSpriterAnimator(garland.SubstituteAnimator, Layer.UI);

            HideBossButtons();

            Animators["BossSelection"].Play("Idle");
            Game.SpriteBatchManager.RemoveSpriterAnimator(Animators["BossSelection"], Layer.BACKGROUND);

            CloseBossPanel(true);

            _uiReseted = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputManager.PressedCancel())
                Game.ScreenManager.GoTo<MainMenuScreen>();

            foreach (var garland in _bossGarlands)
                garland.Update(gameTime);
        }
    }
}