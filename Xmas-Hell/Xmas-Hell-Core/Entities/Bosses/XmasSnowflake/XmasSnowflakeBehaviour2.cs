using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using SpriterDotNet;
using XmasHell.Spriter;

namespace XmasHell.Entities.Bosses.XmasSnowflake
{
    class XmasSnowflakeBehaviour2 : AbstractBossBehaviour
    {
        private bool _initialized;
        private SpriterFile _branch1File;
        private SpriterFile _branch2File;

        private List<XmasSnowflakeBranch> _branches;

        public XmasSnowflakeBehaviour2(Boss boss) : base(boss)
        {
        }

        public override void Start()
        {
            base.Start();

            _initialized = false;

            Boss.MoveTo(new Vector2(GameConfig.VirtualResolution.X / 2f, GameConfig.VirtualResolution.Y / 5f), 0.5f, true);

            Boss.CurrentAnimator.Play("Idle");
            Boss.CurrentAnimator.AnimationFinished += AnimationFinishedHandler;

            _branch1File = SpriterUtils.GetSpriterFile("branch1.png", Boss.CurrentAnimator);
            _branch2File = SpriterUtils.GetSpriterFile("branch2.png", Boss.CurrentAnimator);

            _branches = new List<XmasSnowflakeBranch>(8);
        }

        public override void Stop()
        {
            base.Stop();

            foreach (var branch in _branches)
            {
                branch.Dispose();
            }
        }

        private void AnimationFinishedHandler(string animationName)
        {
            switch (animationName)
            {
                case "Attack1":
                    ReplaceBranches();
                    Boss.CurrentAnimator.Play("WithoutBranches");
                    _initialized = true;
                    break;
                case "RespawnBranches":
                    _initialized = false;
                    break;
            }
        }

        private void ReplaceBranches()
        {
            // Start by retrieving the position of all branches from Spriter object
            var spriteData = Boss.CurrentAnimator.FrameData.SpriteData;

            var branch1Sprites = spriteData.FindAll(so => so.FileId == _branch1File.Id);
            var branch2Sprites = spriteData.FindAll(so => so.FileId == _branch2File.Id);

            foreach (var branch1Sprite in branch1Sprites)
            {
                var branchTexture = Assets.GetTexture2D("Graphics/Sprites/Bosses/XmasSnowflake/branch1");
                var worldPosition = SpriterUtils.GetSpriterWorldPosition(branch1Sprite, Boss.CurrentAnimator);
                var angle = MathHelper.ToRadians(branch1Sprite.Angle);

                var sprite = new Sprite(branchTexture)
                {
                    Position = worldPosition,
                    Rotation = angle,
                    Origin = new Vector2(branch1Sprite.PivotX * branchTexture.Width, branch1Sprite.PivotY * branchTexture.Height),
                    Scale = new Vector2(branch1Sprite.ScaleX, branch1Sprite.ScaleY)
                };

                var branch = new XmasSnowflakeBranch(Boss, sprite);

                _branches.Add(branch);
            }

            foreach (var branch2Sprite in branch2Sprites)
            {
                var branchTexture = Assets.GetTexture2D("Graphics/Sprites/Bosses/XmasSnowflake/branch2");
                var worldPosition = SpriterUtils.GetSpriterWorldPosition(branch2Sprite, Boss.CurrentAnimator);
                var angle = MathHelper.ToRadians(branch2Sprite.Angle);

                var sprite = new Sprite(branchTexture)
                {
                    Position = worldPosition,
                    Rotation = angle,
                    Origin = new Vector2(branch2Sprite.PivotX * branchTexture.Width, branch2Sprite.PivotY * branchTexture.Height),
                    Scale = new Vector2(branch2Sprite.ScaleX, branch2Sprite.ScaleY)
                };

                var branch = new XmasSnowflakeBranch(Boss, sprite);

                _branches.Add(branch);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!_initialized && !Boss.TargetingPosition && Boss.CurrentAnimator.CurrentAnimation.Name != "Attack1")
            {
                Boss.CurrentAnimator.Play("Attack1");
            }

            for (int i = 0; i < _branches.Count; i++)
            {
                var branch = _branches[i];

                branch.Update(gameTime);

                if (!branch.Alive())
                {
                    branch.Dispose();
                    _branches.Remove(branch);
                }
            }

            if (_initialized && _branches.Count == 0 && Boss.CurrentAnimator.CurrentAnimation.Name != "RespawnBranches")
            {
                Boss.CurrentAnimator.Play("RespawnBranches");
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var branch in _branches)
                branch.Draw(spriteBatch);
        }
    }
}