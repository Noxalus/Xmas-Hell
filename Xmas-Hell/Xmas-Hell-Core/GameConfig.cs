#define RELEASE_VERSION

using System;
using Microsoft.Xna.Framework;

namespace XmasHell
{
    public static class GameConfig
    {
        // Graphics
        public static Point VirtualResolution = new Point(1080, 1920);

        // Game
        public static int RandomSeed = 42;

        // Player
        public static float PlayerSpeed = 1125f;
        public static readonly TimeSpan PlayerShootFrequency = TimeSpan.FromMilliseconds(100);
        public static float PlayerMoveSensitivity = 1f;
        public static float PlayerBulletSpeed = 2000f;
        public static float PlayerHitboxRadius = 5f;

        // Boss
        public static int BossDefaultLife = 1000;
        public static float BossDefaultSpeed = 200f;

        // Bullet manager
        public static int MaximumBullets = 500;

        public static Color[] BossHPBarColors = new Color[]
        {
            Color.Green,
            Color.Orange,
            Color.OrangeRed,
            Color.Red
        };

        // Debug
        public static bool DebugScreen = true;
        public static bool GodMode = true;
        public static bool DisplayCollisionBoxes = false;
        public static bool EnableBloom = true;
        public static bool ShowPerformanceInfo = true;
        public static bool ShowPerformanceGraph = true;
    }
}