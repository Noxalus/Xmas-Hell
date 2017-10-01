using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tweening;
using XmasHell.Entities;
using XmasHell.Entities.Bosses;
using XmasHell.GUI;

namespace XmasHell.Screens
{
    public class BossSelectionScreen : Screen
    {
        private Sprite _bossSelectionGround;
        private Sprite _bossSelectionTree;
        private GuiButton _bossSelectionTreeStar;
        private GuiButton _xmasBallBossButton;
        private GuiButton _xmasBellBossButton;
        private GuiButton _xmasSnowflakeBossButton;
        private GuiButton _xmasCandyBossButton;
        private GuiButton _xmasLogBossButton;
        private GuiButton _xmasGiftBossButton;

        private bool _treeFlipped;

        public BossSelectionScreen(XmasHell game) : base(game)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            _treeFlipped = false;

            _bossSelectionGround.Position = new Vector2(_bossSelectionGround.BoundingRectangle.Width / 2f - 100, GameConfig.VirtualResolution.Y);
            _bossSelectionTree.Position = new Vector2(Game.ViewportAdapter.Center.X, _bossSelectionGround.Position.Y - _bossSelectionTree.BoundingRectangle.Height / 2f - 50);

            _bossSelectionTreeStar.Position = new Vector2(
                _bossSelectionTree.Position.X,
                _bossSelectionTree.Position.Y - _bossSelectionTree.BoundingRectangle.Height / 2f +
                _bossSelectionTreeStar.Sprite.BoundingRectangle.Height / 4f
            );

#if ANDROID
            _bossSelectionTreeStar.Tap += OnTreeStarAction;
#else
            _bossSelectionTreeStar.Click += OnTreeStarAction;
#endif

            InitializeBossButtons();
        }

        private void InitializeBossButtons()
        {
            // Relative to the tree position left top corner
            var originPosition = new Vector2(
                _bossSelectionTree.Position.X - _bossSelectionTree.BoundingRectangle.Width / 2f,
                _bossSelectionTree.Position.Y - _bossSelectionTree.BoundingRectangle.Height / 2f
            );

            _xmasBellBossButton.Position = new Vector2(originPosition.X + 180, originPosition.Y + 1400);
            _xmasCandyBossButton.Position = new Vector2(originPosition.X + 370, originPosition.Y + 1475);
            _xmasBallBossButton.Position = new Vector2(originPosition.X + 525, originPosition.Y + 1200);
            _xmasSnowflakeBossButton.Position = new Vector2(originPosition.X + 715, originPosition.Y + 1225);
            _xmasGiftBossButton.Position = new Vector2(originPosition.X + 265, originPosition.Y + 875);
            _xmasLogBossButton.Position = new Vector2(originPosition.X + 425, originPosition.Y + 900);


            // Initialize button action
#if ANDROID
            _xmasBallBossButton.Tap += (object sender, Point position) => OnBossButtonAction(BossType.XmasBall);
            _xmasBellBossButton.Tap += (object sender, Point position) => OnBossButtonAction(BossType.XmasBell);
            _xmasSnowflakeBossButton.Tap += (object sender, Point position) => OnBossButtonAction(BossType.XmasSnowflake);
            _xmasCandyBossButton.Tap += (object sender, Point position) => OnBossButtonAction(BossType.XmasCandy);
            _xmasLogBossButton.Tap += (object sender, Point position) => OnBossButtonAction(BossType.XmasLog);
            _xmasGiftBossButton.Tap += (object sender, Point position) => OnBossButtonAction(BossType.XmasGift);
#else
            _xmasBallBossButton.Click += (object sender, Point position) => OnBossButtonAction(BossType.XmasBall);
            _xmasBellBossButton.Click += (object sender, Point position) => OnBossButtonAction(BossType.XmasBell);
            _xmasSnowflakeBossButton.Click += (object sender, Point position) => OnBossButtonAction(BossType.XmasSnowflake);
            _xmasCandyBossButton.Click += (object sender, Point position) => OnBossButtonAction(BossType.XmasCandy);
            _xmasLogBossButton.Click += (object sender, Point position) => OnBossButtonAction(BossType.XmasLog);
            _xmasGiftBossButton.Click += (object sender, Point position) => OnBossButtonAction(BossType.XmasGift);
#endif
        }

        private void OnBossButtonAction(BossType bossType)
        {
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

            _bossSelectionGround = new Sprite(Assets.GetTexture2D("Graphics/GUI/BossSelection/boss-selection-ground"));
            _bossSelectionTree = new Sprite(Assets.GetTexture2D("Graphics/GUI/BossSelection/boss-selection-tree"));
            _bossSelectionTreeStar = new GuiButton(Game.ViewportAdapter, "boss-selection-tree-star", new Sprite(Assets.GetTexture2D("Graphics/GUI/BossSelection/boss-selection-tree-star")));

            // Bosses
            _xmasBallBossButton = new GuiButton(Game.ViewportAdapter, "xmas-ball-boss-button", new Sprite(Assets.GetTexture2D("Graphics/GUI/BossSelection/unknown-boss-button")));
            _xmasBellBossButton = new GuiButton(Game.ViewportAdapter, "xmas-bell-boss-button", new Sprite(Assets.GetTexture2D("Graphics/GUI/BossSelection/unknown-boss-button")));
            _xmasSnowflakeBossButton = new GuiButton(Game.ViewportAdapter, "xmas-snowflake-boss-button", new Sprite(Assets.GetTexture2D("Graphics/GUI/BossSelection/unknown-boss-button")));
            _xmasCandyBossButton = new GuiButton(Game.ViewportAdapter, "xmas-candy-boss-button", new Sprite(Assets.GetTexture2D("Graphics/GUI/BossSelection/unknown-boss-button")));
            _xmasLogBossButton = new GuiButton(Game.ViewportAdapter, "xmas-log-boss-button", new Sprite(Assets.GetTexture2D("Graphics/GUI/BossSelection/unknown-boss-button")));
            _xmasGiftBossButton = new GuiButton(Game.ViewportAdapter, "xmas-gift-boss-button", new Sprite(Assets.GetTexture2D("Graphics/GUI/BossSelection/unknown-boss-button")));
        }

        public override void Show(bool reset = false)
        {
            base.Show(reset);

            Game.SpriteBatchManager.UISprites.Add(_bossSelectionTree);
            Game.GuiManager.AddButton(_bossSelectionTreeStar);

            Game.GuiManager.AddButton(_xmasBallBossButton);
            Game.GuiManager.AddButton(_xmasBellBossButton);
            Game.GuiManager.AddButton(_xmasSnowflakeBossButton);
            Game.GuiManager.AddButton(_xmasCandyBossButton);
            Game.GuiManager.AddButton(_xmasLogBossButton);
            Game.GuiManager.AddButton(_xmasGiftBossButton);

            //Game.SpriteBatchManager.UISprites.Add(_bossSelectionGround);
        }

        public override void Hide()
        {
            base.Hide();

            Game.SpriteBatchManager.UISprites.Remove(_bossSelectionGround);
            Game.SpriteBatchManager.UISprites.Remove(_bossSelectionTree);
            Game.GuiManager.RemoveButton(_bossSelectionTreeStar);

            Game.GuiManager.RemoveButton(_xmasBallBossButton);
            Game.GuiManager.RemoveButton(_xmasBellBossButton);
            Game.GuiManager.RemoveButton(_xmasSnowflakeBossButton);
            Game.GuiManager.RemoveButton(_xmasCandyBossButton);
            Game.GuiManager.RemoveButton(_xmasLogBossButton);
            Game.GuiManager.RemoveButton(_xmasGiftBossButton);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void FlipTree()
        {
            if (_treeFlipped)
                _bossSelectionTree.CreateTweenGroup().ScaleTo(new Vector2(1f, 1f), 1.0f, EasingFunctions.Linear);
            else
                _bossSelectionTree.CreateTweenGroup().ScaleTo(new Vector2(-1f, 1f), 1.0f, EasingFunctions.Linear);

            _treeFlipped = !_treeFlipped;
        }
    }
}