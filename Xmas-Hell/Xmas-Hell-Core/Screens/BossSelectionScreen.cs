using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using XmasHell.Entities.Bosses;
using XmasHell.GUI;
using XmasHell.Rendering;

namespace XmasHell.Screens
{
    public class BossSelectionScreen : Screen
    {
        private SpriterGuiButton _bossSelectionTreeStar;
        private List<SpriterGuiButton> _bossButtons;
        //private SpriterGuiButton _xmasBallBossButton;
        //private SpriterGuiButton _xmasBellBossButton;
        //private SpriterGuiButton _xmasSnowflakeBossButton;
        //private SpriterGuiButton _xmasCandyBossButton;
        //private SpriterGuiButton _xmasLogBossButton;
        //private SpriterGuiButton _xmasGiftBossButton;

        private bool _treeFlipped;

        public BossSelectionScreen(XmasHell game) : base(game)
        {
            _bossButtons = new List<SpriterGuiButton>();
        }

        public override void Initialize()
        {
            base.Initialize();

            _treeFlipped = false;

//#if ANDROID
//            _bossSelectionTreeStar.Tap += OnTreeStarAction;
//#else
//            _bossSelectionTreeStar.Click += OnTreeStarAction;
//#endif
        }

        private void OnBossButtonAction(object button, Point position)
        {
            var spriterGuiButton = button as SpriterGuiButton;

            var bossType = BossFactory.StringToBossType(spriterGuiButton.Name);

            Game.ScreenManager.GetScreen<GameScreen>().LoadBoss(bossType);
            Game.ScreenManager.GoTo<GameScreen>();
        }

        private void OnTreeStarAction(object sender, Point position)
        {
            FlipTree();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            //_bossSelectionTreeStar = new SpriterGuiButton(Game.ViewportAdapter, "boss-selection-tree-star", new Sprite(Assets.GetTexture2D("Graphics/GUI/BossSelection/boss-selection-tree-star")));

            LoadSpriterSprite("Graphics/GUI/BossSelection/boss-selection");
        }

        protected override void LoadSpriterSprite(String spriterFilename)
        {
            base.LoadSpriterSprite(spriterFilename);

            Animators["BossSelection"].Position = Game.ViewportAdapter.Center.ToVector2();
        }

        protected override void InitializeSpriterGui()
        {
            // TODO: Choose the animator entity according to player state (from Android preferences)

            // Xmas Ball
            var xmasBallAnimator = Animators["Ball"].Clone();
            xmasBallAnimator.Speed *= Game.GameManager.Random.Next(2, 5);

            xmasBallAnimator.AddTextureSwap(
                "Graphics/GUI/BossSelection/unknown-boss-button",
                Assets.GetTexture2D("Graphics/GUI/BossSelection/xmas-ball-available-button")
            );

            var xmasBallBossButton = new SpriterGuiButton(
                Game.ViewportAdapter, "XmasBall", "Graphics/GUI/BossSelection/xmas-ball-dummy-boss-button.png",
                xmasBallAnimator, Animators["BossSelection"]
            );

            _bossButtons.Add(xmasBallBossButton);

            // Xmas Bell
            var xmasBellAnimator = Animators["Ball"].Clone();

            xmasBellAnimator.AddTextureSwap(
                "Graphics/GUI/BossSelection/unknown-boss-button",
                Assets.GetTexture2D("Graphics/GUI/BossSelection/xmas-bell-available-button")
            );

            var xmasBellBossButton = new SpriterGuiButton(
                Game.ViewportAdapter, "XmasBell", "Graphics/GUI/BossSelection/xmas-bell-dummy-boss-button.png",
                xmasBellAnimator, Animators["BossSelection"]
            );

            _bossButtons.Add(xmasBellBossButton);

            // Xmas Snowflake
            var xmasSnowflakeAnimator = Animators["Ball"].Clone();

            xmasSnowflakeAnimator.AddTextureSwap(
                "Graphics/GUI/BossSelection/unknown-boss-button",
                Assets.GetTexture2D("Graphics/GUI/BossSelection/xmas-snowflake-available-button")
            );

            var xmasSnowflakeBossButton = new SpriterGuiButton(
                Game.ViewportAdapter, "XmasSnowflake", "Graphics/GUI/BossSelection/xmas-snowflake-dummy-boss-button.png",
                xmasSnowflakeAnimator, Animators["BossSelection"]
            );

            _bossButtons.Add(xmasSnowflakeBossButton);

            // Xmas Candy
            var xmasCandyAnimator = Animators["Ball"].Clone();

            xmasCandyAnimator.AddTextureSwap(
                "Graphics/GUI/BossSelection/unknown-boss-button",
                Assets.GetTexture2D("Graphics/GUI/BossSelection/xmas-candy-available-button")
            );

            var xmasCandyBossButton = new SpriterGuiButton(
                Game.ViewportAdapter, "XmasCandy", "Graphics/GUI/BossSelection/xmas-candy-dummy-boss-button.png",
                xmasCandyAnimator, Animators["BossSelection"]
            );

            _bossButtons.Add(xmasCandyBossButton);

            // Xmas Gift
            var xmasGiftAnimator = Animators["Ball"].Clone();

            xmasGiftAnimator.AddTextureSwap(
                "Graphics/GUI/BossSelection/unknown-boss-button",
                Assets.GetTexture2D("Graphics/GUI/BossSelection/xmas-gift-available-button")
            );

            var xmasGiftBossButton = new SpriterGuiButton(
                Game.ViewportAdapter, "XmasGift", "Graphics/GUI/BossSelection/xmas-gift-dummy-boss-button.png",
                xmasGiftAnimator, Animators["BossSelection"]
            );

            _bossButtons.Add(xmasGiftBossButton);

            // Xmas Log
            var xmasLogAnimator = Animators["Ball"].Clone();

            xmasLogAnimator.AddTextureSwap(
                "Graphics/GUI/BossSelection/unknown-boss-button",
                Assets.GetTexture2D("Graphics/GUI/BossSelection/xmas-log-available-button")
            );

            var xmasLogBossButton = new SpriterGuiButton(
                Game.ViewportAdapter, "XmasLog", "Graphics/GUI/BossSelection/xmas-log-dummy-boss-button.png",
                xmasLogAnimator, Animators["BossSelection"]
            );

            _bossButtons.Add(xmasLogBossButton);

            ResetUI();
        }

        private void ResetUI()
        {
            foreach (var bossButton in _bossButtons)
            {
#if ANDROID
                bossButton.Tap += OnBossButtonAction;
#else
                bossButton.Click += OnBossButtonAction;
#endif
                Game.GuiManager.AddButton(bossButton);
            }

            if (Animators["BossSelection"] != null)
                Game.SpriteBatchManager.AddSpriterAnimator(Animators["BossSelection"], Layer.BACKGROUND);
        }

        public override void Show(bool reset = false)
        {
            base.Show(reset);

            ResetUI();
        }

        public override void Hide()
        {
            base.Hide();

            foreach (var bossButton in _bossButtons)
            {
#if ANDROID
                bossButton.Tap -= OnBossButtonAction;
#else
                bossButton.Click -= OnBossButtonAction;
#endif

                Game.GuiManager.RemoveButton(bossButton);
            }

            Game.SpriteBatchManager.RemoveSpriterAnimator(Animators["BossSelection"], Layer.BACKGROUND);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void FlipTree()
        {
            // TODO: Start tree's flip animation

            _treeFlipped = !_treeFlipped;
        }
    }
}