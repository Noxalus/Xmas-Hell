using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using XmasHell.Performance;
using System.Globalization;
using XmasHell.Extensions;
using XmasHell.Background;
using XmasHell.Entities.Bosses;

namespace XmasHell
{
    public static class GameConfig
    {
        // Graphics
        public static Point VirtualResolution = new Point(1080, 1920);

        // Game
        public static int RandomSeed = 42;

        public static float EndGameCameraZoomTime = 0.25f;
        public static float EndGameExplosionTime = 0.3f;

        // Player
        public static float PlayerSpeed = 1500f;
        public static readonly TimeSpan PlayerShootFrequency = TimeSpan.FromMilliseconds(100);
        public static float PlayerMoveSensitivity = 1f;
        public static float PlayerBulletSpeed = 2000f;
        public static float PlayerHitboxRadius = 5f;
        public static float PlayerEntranceAnimationTime = 0.5f; // seconds

        // Boss
        public static int BossDefaultBehaviourLife = 400;
        public static float BossDefaultSpeed = 200f;
        public static Vector2 BossDefaultPosition = new Vector2(VirtualResolution.X / 2f, VirtualResolution.Y * 0.15f);
        public static float BossEntranceAnimationTime = 1; // seconds
        public static BossType[] EnabledBosses = new BossType[]
        {
            BossType.Debug,
            BossType.XmasBall,
            BossType.XmasBell,
            BossType.XmasCandy,
            BossType.XmasSnowflake,
            BossType.XmasTree
        };

        // Bullet manager
        public static int MaximumBullets = 2500;
        public static Rectangle BulletArea = new Rectangle(-500, -500, 500, 500);

        public static Color[] BossHPBarColors = new Color[]
        {
            Color.Green,
            Color.Orange,
            Color.OrangeRed,
            Color.Red,
            Color.DarkRed
        };

        public static Dictionary<BackgroundLevel, Tuple<Color, Color>> BackgroundGradients = new Dictionary<BackgroundLevel, Tuple<Color, Color>>
        {
            { BackgroundLevel.Level1, new Tuple<Color, Color>(ColorExtension.FromHex("#D2EFF7"), ColorExtension.FromHex("#B7E5F2")) },
            { BackgroundLevel.Level2, new Tuple<Color, Color>(ColorExtension.FromHex("#91D3EC"), ColorExtension.FromHex("#61BCDE")) },
            { BackgroundLevel.Level3, new Tuple<Color, Color>(ColorExtension.FromHex("#419CBF"), ColorExtension.FromHex("#1C8DB8")) },
            { BackgroundLevel.Level4, new Tuple<Color, Color>(ColorExtension.FromHex("#066B91"), ColorExtension.FromHex("#045878")) },
            { BackgroundLevel.Level5, new Tuple<Color, Color>(ColorExtension.FromHex("#01364B"), ColorExtension.FromHex("#00212D")) }
        };

        // Physics
        public static readonly Vector2 DefaultGravity = Vector2.UnitY * 9.82f;

        // Debug
        public static bool GodMode = false;
        public static bool DebugPhysics = false;
        public static bool DisableCollision = false;
        public static bool DisableParticles = false;
        public static bool DebugScreen = false;
        public static bool EnableBloom = true;
        public static bool EnableBloomPulsation = false;
        public static bool ShowPerformanceInfo = false;
        public static bool ShowPerformanceGraph = false;
        public static float PerformanceInfoTextScale = 1.5f;
        public static int PerformanceGraphMaxSample = 500;
        public static readonly List<PerformanceStopwatchType> DisabledGraph = new List<PerformanceStopwatchType>()
        {
            PerformanceStopwatchType.GlobalFrame,
            //PerformanceStopwatchType.GlobalUpdate,
            PerformanceStopwatchType.BackgroundUpdate,
            PerformanceStopwatchType.ParticleUpdate,
            PerformanceStopwatchType.GlobalCollisionUpdate,
            PerformanceStopwatchType.PlayerHitboxBossBulletsCollisionUpdate,
            PerformanceStopwatchType.PlayerHitboxBossHitboxesCollisionUpdate,
            PerformanceStopwatchType.PlayerBulletsBossHitboxesCollisionUpdate,
            PerformanceStopwatchType.BossBulletUpdate,
            PerformanceStopwatchType.PlayerBulletUpdate,
            PerformanceStopwatchType.BossBehaviourUpdate,
            PerformanceStopwatchType.BackgroundSpriterAnimatorsUpdate,
            PerformanceStopwatchType.BackSpriterAnimatorsUpdate,
            PerformanceStopwatchType.FrontSpriterAnimatorsUpdate,
            PerformanceStopwatchType.UISpriterAnimatorsUpdate,
            PerformanceStopwatchType.UILabelsUpdate,
            PerformanceStopwatchType.PerformanceManagerUpdate,
            //PerformanceStopwatchType.GlobalDraw,
            PerformanceStopwatchType.ClearColorDraw,
            PerformanceStopwatchType.BackgroundDraw,
            PerformanceStopwatchType.CloudsDraw,
            PerformanceStopwatchType.SpriteBatchManagerDraw,
            PerformanceStopwatchType.BackgroundParticleDraw,
            PerformanceStopwatchType.BackgroundSpriterAnimatorDraw,
            PerformanceStopwatchType.GameParticleDraw,
            PerformanceStopwatchType.BossBulletDraw,
            PerformanceStopwatchType.PlayerBulletDraw,
            PerformanceStopwatchType.BloomDraw,
            PerformanceStopwatchType.BloomRenderTargetDraw,
            PerformanceStopwatchType.UIDraw,
            PerformanceStopwatchType.UISpritesDraw,
            PerformanceStopwatchType.UISpriterAnimatorDraw,
            PerformanceStopwatchType.UILabelsDraw,
            //PerformanceStopwatchType.PerformanceManagerDraw
        };
    }
}