using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using XmasHell.Entities.Bosses;
using XmasHell.GUI;
using XmasHell.Rendering;
using XmasHell.Spriter;

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

        private readonly Dictionary<string, Tuple<string, string>> _bossRelations = new Dictionary<string, Tuple<string, string>>
        {
            { "santa", new Tuple<string, string>("reindeer", "snowman") },
            { "reindeer", new Tuple<string, string>("gift", "log") },
            { "snowman", new Tuple<string, string>("log", "tree") },
            { "gift", new Tuple<string, string>("bell", "candy") },
            { "log", new Tuple<string, string>("candy", "ball") },
            { "tree", new Tuple<string, string>("ball", "snowflake") }
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
                var noRelation = !_bossRelations.ContainsKey(bossButton.Name);

                var available = noRelation ||
                    (Game.PlayerData.BossBeatenCounter(BossFactory.StringToBossType(_bossRelations[bossButton.Name].Item1)) > 0 &&
                    Game.PlayerData.BossBeatenCounter(BossFactory.StringToBossType(_bossRelations[bossButton.Name].Item2)) > 0);

                var hidden = Game.PlayerData.BossAttempts(BossFactory.StringToBossType(bossButton.Name)) == 0;

                if (noRelation || (available && !hidden))
                {
                    var beaten = Game.PlayerData.BossBeatenCounter(BossFactory.StringToBossType(bossButton.Name)) > 0;

                    bossButton.Animator.AddTextureSwap(
                        "Graphics/GUI/BossSelection/unknown-boss-button",
                        Assets.GetTexture2D("Graphics/GUI/BossSelection/xmas-" + bossButton.Name + "-" + ((beaten) ? "beaten" : "available") + "-button")
                    );
                }

                Game.GuiManager.AddButton(bossButton);
            }

            if (Animators["BossSelection"] != null)
            {
                Game.SpriteBatchManager.AddSpriterAnimator(Animators["BossSelection"], Layer.BACKGROUND);
                Animators["BossSelection"].Play("Intro");
                Animators["BossSelection"].AnimationFinished += BossSelectionScreen_AnimationFinished;
            }

            if (Animators["Garland"] != null)
                Game.SpriteBatchManager.AddSpriterAnimator(Animators["Garland"], Layer.BACKGROUND);
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

            // Synchronize current GUI button animator with the related dummy element from the Spriter file
            var spriterDummyData = SpriterUtils.GetSpriterFileData("xmas-ball-log-garland.png", Animators["BossSelection"]);

            if (spriterDummyData != null)
            {
                var dummyPosition = new Vector2(spriterDummyData.X, -spriterDummyData.Y);
                var dummyScale = new Vector2(spriterDummyData.ScaleX, spriterDummyData.ScaleY);

                Animators["Garland"].Position = Animators["BossSelection"].Position + dummyPosition;
                Animators["Garland"].Rotation = -spriterDummyData.Angle;
                Animators["Garland"].Scale = dummyScale;
                Animators["Garland"].Color = new Color(Animators["Garland"].Color, spriterDummyData.Alpha);
            }
        }

        public void FlipTree()
        {
            // TODO: Start tree's flip animation

            _treeFlipped = !_treeFlipped;
        }
    }
}