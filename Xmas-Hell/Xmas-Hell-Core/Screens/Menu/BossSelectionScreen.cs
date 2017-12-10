using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using XmasHell.Controls;
using XmasHell.Entities.Bosses;
using XmasHell.GUI;
using XmasHell.Rendering;
using XmasHell.Spriter;

namespace XmasHell.Screens.Menu
{
    public class BossSelectionScreen : MenuScreen
    {
        #region Fields
        private BossType _selectedBoss;
        private Dictionary<string, SpriterSubstituteEntity> _bossGarlands = new Dictionary<string, SpriterSubstituteEntity>();
        private List<SpriterSubstituteEntity> _ballHooks = new List<SpriterSubstituteEntity>();

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

            SpriterFile = Assets.GetSpriterAnimators("Graphics/GUI/boss-selection");
            InitializeSpriterGui();
        }

        private void InitializeSpriterGui()
        {
            SpriterFile["Main"].AnimationFinished += BossSelectionScreen_AnimationFinished;
            SpriterFile["BossPanel"].AnimationFinished += BossPanel_AnimationFinished;

            SpriterFile["Garland"].ZIndex(5);
            SpriterFile["BallHook"].ZIndex(6);
            SpriterFile["Ball"].ZIndex(9);
            SpriterFile["BossPanel"].ZIndex(10);

            // Christmas tree's balls
            foreach (var bossName in _bossNames)
            {
                var ballAnimator = SpriterFile["Ball"].Clone();
                ballAnimator.Play("Balance");
                ballAnimator.Progress = (float)Game.GameManager.Random.NextDouble();
                ballAnimator.Speed = 0.5f + (float)Game.GameManager.Random.NextDouble();

                var hasRelation = _bossRelations.ContainsKey(bossName);

                for (int i = 0; i < 10; i++)
                    _ballHooks.Add(new SpriterSubstituteEntity("garland-forbidden-ball.png", SpriterFile["Main"], SpriterFile["BallHook"].Clone(), "garland-forbidden-ball_00" + i));

                if (hasRelation)
                {
                    _bossGarlands.Add(
                        _bossRelations[bossName].Item1 + "-" + bossName,
                        new SpriterSubstituteEntity(
                            "xmas-" + _bossRelations[bossName].Item1 + "-" + bossName + "-garland.png",
                            SpriterFile["Main"], SpriterFile["Garland"].Clone()
                        )
                    );

                    _bossGarlands.Add(
                         _bossRelations[bossName].Item2 + "-" + bossName,
                         new SpriterSubstituteEntity(
                            "xmas-" + _bossRelations[bossName].Item2 + "-" + bossName + "-garland.png",
                            SpriterFile["Main"], SpriterFile["Garland"].Clone()
                        )
                    );
                }

                var bossButton = new SpriterGuiButton(
                    Game.ViewportAdapter, bossName, "Graphics/GUI/BossSelection/xmas-" + bossName + "-available-button.png",
                    ballAnimator, SpriterFile["Main"], "Balance", null, "Audio/SE/select1"
                );

                bossButton.Action += OnBossButtonAction;
                _bossButtons.Add(BossFactory.StringToBossType(bossName), bossButton);
            }

            // Boss panel buttons
            var closeBossPanelButton = new SpriterGuiButton(
                Game.ViewportAdapter, "CloseBossPanel", "Graphics/GUI/BossSelection/boss-panel-close-button.png",
                SpriterFile["CloseButton"], SpriterFile["BossPanel"], "Idle", null, "Audio/SE/select4"
            );

            var startBattleBossPanelButton = new SpriterGuiButton(
                Game.ViewportAdapter, "StartBattleBossPanel", "Graphics/GUI/BossSelection/boss-panel-battle-button.png",
                SpriterFile["BattleButton"], SpriterFile["BossPanel"], "Idle", null, "Audio/SE/select2"
            );

            closeBossPanelButton.Action += BossPanelCloseButtonAction;
            startBattleBossPanelButton.Action += BossPanelStartBattleButtonAction;

            closeBossPanelButton.Animator().ZIndex(11);
            startBattleBossPanelButton.Animator().ZIndex(11);

            _bossPanelButtons.Add(closeBossPanelButton);
            _bossPanelButtons.Add(startBattleBossPanelButton);

            // Labels
            _bossNameLabel = new SpriterGuiLabel("Unknown", Assets.GetFont("Graphics/Fonts/ui-title"), "boss-panel-title-label.png", SpriterFile["BossPanel"], Vector2.Zero, true);
            _bestTimeLabel = new SpriterGuiLabel("", Assets.GetFont("Graphics/Fonts/ui"), "boss-panel-best-time-label.png", SpriterFile["BossPanel"], Vector2.Zero);
            _playTimeLabel = new SpriterGuiLabel("", Assets.GetFont("Graphics/Fonts/ui"), "boss-panel-play-time-label.png", SpriterFile["BossPanel"], Vector2.Zero);
            _playerDeathLabel = new SpriterGuiLabel("", Assets.GetFont("Graphics/Fonts/ui"), "boss-panel-player-deaths-label.png", SpriterFile["BossPanel"], Vector2.Zero);
            _bossDeathLabel = new SpriterGuiLabel("", Assets.GetFont("Graphics/Fonts/ui"), "boss-panel-boss-deaths-label.png", SpriterFile["BossPanel"], Vector2.Zero);

            _bossPanelLabels.Add(_bossNameLabel);
            _bossPanelLabels.Add(_bestTimeLabel);
            _bossPanelLabels.Add(_playTimeLabel);
            _bossPanelLabels.Add(_playerDeathLabel);
            _bossPanelLabels.Add(_bossDeathLabel);

            ResetUI();
        }

        #region Button actions
        private void OnBossButtonAction(object button, Point position)
        {
            var spriterGuiButton = button as SpriterGuiButton;

            // Make sure the ball is entirely visible
            spriterGuiButton.SubstituteEntity.SubstituteAnimator.Color = 
                new Color(spriterGuiButton.SubstituteEntity.SubstituteAnimator.Color, 1f);

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
            Game.GameManager.LoadBoss(_selectedBoss);
            Game.ScreenManager.GoTo<GameScreen>();

            Game.CloudManager.Dispose();
        }
        #endregion

        #region Animations finished
        private void BossSelectionScreen_AnimationFinished(string animationName)
        {
            if (animationName == "Intro")
            {
                SpriterFile["Main"].Play("Idle");
            }
        }

        private void BossPanel_AnimationFinished(string animationName)
        {
            if (animationName == "Show")
                SpriterFile["BossPanel"].Play("Idle");
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
            spriterGuiButton.Animator().ZIndex(11);

            foreach (var bossPanelButton in _bossPanelButtons)
                Game.GuiManager.AddButton(bossPanelButton);

            foreach (var bossPanelLabel in _bossPanelLabels)
                Game.GuiManager.AddLabel(bossPanelLabel);

            Game.SpriteBatchManager.AddSpriterAnimator(SpriterFile["BossPanel"], Layer.UI);
            SpriterFile["BossPanel"].Play("Show");

            // Update labels
            var bossDeath = Game.PlayerData.BossBeatenCounter(bossType);
            _bossNameLabel.Text = "Xmas " + bossType.ToString().Substring(4);
            _bestTimeLabel.Text = "Best time: ";
            _bestTimeLabel.Text += (bossDeath > 0) ? Game.PlayerData.BossBestTime(bossType).ToString("mm\\:ss") : "--:--";
            _playTimeLabel.Text = "Play time: " + Game.PlayerData.BossPlayTime(bossType).ToString("mm\\:ss");
            _playerDeathLabel.Text = "Attempts: " + (Game.PlayerData.DeathCounter(bossType) + Game.PlayerData.BossBeatenCounter(bossType)).ToString("000");
            _bossDeathLabel.Text = "Boss deaths: " + bossDeath;

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

            Game.SpriteBatchManager.RemoveSpriterAnimator(SpriterFile["BossPanel"], Layer.UI);
            EnableBossButtons();
        }

        private void CloseBossPanel(bool hardClose = false)
        {
            if (_bossButtons.ContainsKey(_selectedBoss))
            {
                _bossButtons[_selectedBoss].SubstituteEntity.EnableSynchronization(true);
                _bossButtons[_selectedBoss].Animator().ZIndex(9);
            }

            if (hardClose)
            {
                DoCloseBossPanel();
            }
            else
            {
                SpriterFile["BossPanel"].Play("Hide");
                SpriterFile["BossPanel"].CurrentAnimation.Looping = false;
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

                // TO REMOVE: Make all boss available from start to test easily
                //available = true;

                // TO REMOVE: Only the first 4 bosses will be available
                available = !hasRelation;

                if (available)
                {
                    var bossType = BossFactory.StringToBossType(bossButton.Name);
                    var beaten = Game.PlayerData.BossBeatenCounter(bossType) > 0;
                    var hidden = Game.PlayerData.DeathCounter(bossType) == 0 && Game.PlayerData.BossBeatenCounter(bossType) == 0;
                    var buttonTextureSwapName = "xmas-" + bossButton.Name + "-" + ((beaten) ? "beaten" : "available") + "-button";

                    if (hasRelation && hidden)
                        buttonTextureSwapName = "hidden-boss-button";

                    if (hasRelation)
                    {
                        var relation1 = _bossRelations[bossButton.Name].Item1;
                        var relation2 = _bossRelations[bossButton.Name].Item2;
                        var animationName = (beaten) ? "FlashBeaten" : "FlashAvailable";

                        _bossGarlands[relation1 + "-" + bossButton.Name].SubstituteAnimator.Play(animationName);
                        _bossGarlands[relation2 + "-" + bossButton.Name].SubstituteAnimator.Play(animationName);
                    }

                    bossButton.Animator().AddTextureSwap(
                        "Graphics/GUI/BossSelection/unknown-boss-button",
                        Assets.GetTexture2D("Graphics/GUI/BossSelection/" + buttonTextureSwapName)
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

        protected override void ResetUI()
        {
            base.ResetUI();

            if (UIReseted)
                return;

            if (SpriterFile["Main"] != null)
            {
                if (Game.ScreenManager.GetPreviousScreen().Name == "MainMenuScreen")
                    SpriterFile["Main"].Play("Intro");
                else if (Game.ScreenManager.GetPreviousScreen().Name == "GameScreen")
                    SpriterFile["Main"].Play("Idle");

                Game.SpriteBatchManager.AddSpriterAnimator(SpriterFile["Main"], Layer.BACKGROUND);
                SpriterFile["Main"].CurrentAnimation.Looping = false;
            }

            foreach (var ballHook in _ballHooks)
            {
                ballHook.Reset();
                Game.SpriteBatchManager.AddSpriterAnimator(ballHook.SubstituteAnimator, Layer.UI);
            }

            foreach (var garland in _bossGarlands)
            {
                garland.Value.Reset();
                Game.SpriteBatchManager.AddSpriterAnimator(garland.Value.SubstituteAnimator, Layer.UI);
            }

            ShowBossButtons();
            UIReseted = true;
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

            foreach (var ballHook in _ballHooks)
                Game.SpriteBatchManager.RemoveSpriterAnimator(ballHook.SubstituteAnimator, Layer.UI);

            foreach (var garland in _bossGarlands)
                Game.SpriteBatchManager.RemoveSpriterAnimator(garland.Value.SubstituteAnimator, Layer.UI);

            HideBossButtons();

            SpriterFile["Main"].Play("Idle");
            Game.SpriteBatchManager.RemoveSpriterAnimator(SpriterFile["Main"], Layer.BACKGROUND);

            CloseBossPanel(true);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputManager.PressedCancel())
                Game.ScreenManager.GoTo<MainMenuScreen>();

            foreach (var ballHook in _ballHooks)
                ballHook.Update(gameTime);

            foreach (var garland in _bossGarlands)
                garland.Value.Update(gameTime);
        }
    }
}