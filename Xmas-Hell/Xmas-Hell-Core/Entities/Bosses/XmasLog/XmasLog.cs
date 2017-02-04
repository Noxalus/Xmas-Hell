using System.Collections.Generic;
using System.Linq;
using BulletML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using SpriterDotNet;
using SpriterDotNet.MonoGame;
using SpriterDotNet.Providers;
using XmasHell.Physics.Collision;
using XmasHell.Spriter;

namespace XmasHell.Entities.Bosses.XmasLog
{
    class XmasLog : Boss
    {
        private bool _pause = false;
        private KeyboardState _oldKeyboardState;

        public XmasLog(XmasHell game, PositionDelegate playerPositionDelegate) : base(game, playerPositionDelegate)
        {
            // BulletML
            BulletPatternFiles.Add("sample");

            SpriterFilename = "Graphics/Sprites/Bosses/XmasLog/xmas-log";
        }

        protected override void InitializePhysics()
        {
            base.InitializePhysics();

            Game.GameManager.CollisionWorld.AddBossHitBox(new SpriterCollisionConvexPolygon(this, "body.png", Vector2.Zero));
        }
    }
}