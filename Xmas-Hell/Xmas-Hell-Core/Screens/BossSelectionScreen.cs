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
        private readonly List<string> _bossNames = new List<string>()
        {
            "ball", "bell", "snowflake", "candy", "gift", "log", "tree", "reindeer", "snowman", "santa"
        };

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

            LoadSpriterSprite("Graphics/GUI/boss-selection");
        }

        protected override void LoadSpriterSprite(String spriterFilename)
        {
            base.LoadSpriterSprite(spriterFilename);
        }

        protected override void InitializeSpriterGui()
        {
            // TODO: Choose the animator entity according to player state (from Android preferences)

            foreach(var bossName in _bossNames)
            {
                var ballAnimator = Animators["Ball"].Clone();
                ballAnimator.Play("Balance");
                ballAnimator.Progress = (float)Game.GameManager.Random.NextDouble();
                ballAnimator.Speed = 0.5f + (float)Game.GameManager.Random.NextDouble();

                var bossButton = new SpriterGuiButton(
                    Game.ViewportAdapter, bossName, "Graphics/GUI/BossSelection/xmas-" + bossName  + "-available-button.png",
                    ballAnimator, Animators["BossSelection"]
                );

                _bossButtons.Add(bossButton);
            }

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
                var beaten = Game.PlayerData.BossBeatenCounter(BossFactory.StringToBossType(bossButton.Name)) > 0;

                bossButton.Animator.AddTextureSwap(
                    "Graphics/GUI/BossSelection/unknown-boss-button",
                    Assets.GetTexture2D("Graphics/GUI/BossSelection/xmas-" + bossButton.Name + "-" + ((beaten) ? "beaten" : "available") + "-button")
                );

                Game.GuiManager.AddButton(bossButton);
            }

            if (Animators["BossSelection"] != null)
            {
                Game.SpriteBatchManager.AddSpriterAnimator(Animators["BossSelection"], Layer.BACKGROUND);
                Animators["BossSelection"].Play("Intro");
                Animators["BossSelection"].AnimationFinished += BossSelectionScreen_AnimationFinished;
            }
        }

        private void BossSelectionScreen_AnimationFinished(string animationName)
        {
            if (animationName == "Intro")
            {
                Animators["BossSelection"].Play("NoAnimation");
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

            foreach (var bossButton in _bossButtons)
            {
#if ANDROID
                bossButton.Tap -= OnBossButtonAction;
#else
                bossButton.Click -= OnBossButtonAction;
#endif

                Game.GuiManager.RemoveButton(bossButton);
            }

            Animators["BossSelection"].Play("Idle");
            Game.SpriteBatchManager.RemoveSpriterAnimator(Animators["BossSelection"], Layer.BACKGROUND);
            Animators["BossSelection"].AnimationFinished -= BossSelectionScreen_AnimationFinished;
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