using Microsoft.Xna.Framework;
using System;
using XmasHell.Entities.Bosses;
using XmasHell.GUI;
using XmasHell.Rendering;
using XmasHell.Spriter;

namespace XmasHell.Screens
{
    public class BossSelectionScreen : Screen
    {
        private SpriterGuiButton _bossSelectionTreeStar;
        private SpriterGuiButton _xmasBallBossButton;
        private SpriterGuiButton _xmasBellBossButton;
        private SpriterGuiButton _xmasSnowflakeBossButton;
        private SpriterGuiButton _xmasCandyBossButton;
        private SpriterGuiButton _xmasLogBossButton;
        private SpriterGuiButton _xmasGiftBossButton;

        private bool _treeFlipped;

        public BossSelectionScreen(XmasHell game) : base(game)
        {
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

            var bossType = BossType.Debug;

            if (spriterGuiButton.Name == "XmasBall")
                bossType = BossType.XmasBall;

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

            //// Bosses
            //_xmasBallBossButton = new SpriterGuiButton(Game.ViewportAdapter, "xmas-ball-boss-button", new Sprite(Assets.GetTexture2D("Graphics/GUI/BossSelection/unknown-boss-button")));
            //_xmasBellBossButton = new SpriterGuiButton(Game.ViewportAdapter, "xmas-bell-boss-button", new Sprite(Assets.GetTexture2D("Graphics/GUI/BossSelection/unknown-boss-button")));
            //_xmasSnowflakeBossButton = new SpriterGuiButton(Game.ViewportAdapter, "xmas-snowflake-boss-button", new Sprite(Assets.GetTexture2D("Graphics/GUI/BossSelection/unknown-boss-button")));
            //_xmasCandyBossButton = new SpriterGuiButton(Game.ViewportAdapter, "xmas-candy-boss-button", new Sprite(Assets.GetTexture2D("Graphics/GUI/BossSelection/unknown-boss-button")));
            //_xmasLogBossButton = new SpriterGuiButton(Game.ViewportAdapter, "xmas-log-boss-button", new Sprite(Assets.GetTexture2D("Graphics/GUI/BossSelection/unknown-boss-button")));
            //_xmasGiftBossButton = new SpriterGuiButton(Game.ViewportAdapter, "xmas-gift-boss-button", new Sprite(Assets.GetTexture2D("Graphics/GUI/BossSelection/unknown-boss-button")));

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
            var spriterXmasBallDummyPosition = SpriterUtils.GetSpriterFilePosition("xmas-ball-dummy-boss-button.png", Animators["BossSelection"]);
            if (true) // available but not beaten
            {
                Animators["XmasBalls"].Position = Game.ViewportAdapter.Center.ToVector2() + spriterXmasBallDummyPosition;
                _xmasBallBossButton = new SpriterGuiButton(Game.ViewportAdapter, "XmasBall", "xmas-ball-dummy-boss-button.png", Animators["XmasBalls"]);
            }

            ResetUI();
        }

        private void ResetUI()
        {
            // Initialize button action
            if (_xmasBallBossButton != null)
            {
#if ANDROID
                _xmasBallBossButton.Tap += OnBossButtonAction;
#else
                _xmasBallBossButton.Click += OnBossButtonAction;
#endif
                Game.GuiManager.AddButton(_xmasBallBossButton);
            }

            if (Animators["BossSelection"] != null)
                Game.SpriteBatchManager.AddSpriterAnimator(Animators["BossSelection"], Layer.BACKGROUND);
        }

        public override void Show(bool reset = false)
        {
            base.Show(reset);

            //Game.GuiManager.AddButton(_bossSelectionTreeStar);

            //Game.GuiManager.AddButton(_xmasBallBossButton);
            //Game.GuiManager.AddButton(_xmasBellBossButton);
            //Game.GuiManager.AddButton(_xmasSnowflakeBossButton);
            //Game.GuiManager.AddButton(_xmasCandyBossButton);
            //Game.GuiManager.AddButton(_xmasLogBossButton);
            //Game.GuiManager.AddButton(_xmasGiftBossButton);

            ResetUI();
        }

        public override void Hide()
        {
            base.Hide();

#if ANDROID
            _xmasBallBossButton.Tap -= OnBossButtonAction;
#else
            _xmasBallBossButton.Click -= OnBossButtonAction;
#endif

            Game.GuiManager.RemoveButton(_xmasBallBossButton);
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