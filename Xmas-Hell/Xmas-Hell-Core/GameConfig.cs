using System;
using Microsoft.Xna.Framework;

namespace XmasHell
{
    public static class GameConfig
    {
        // Graphics
        public static Point VirtualResolution = new Point(720, 1280);

        // Game
        public static int RandomSeed = 42;

        // Player
        public static readonly TimeSpan PlayerShootFrequency = TimeSpan.FromMilliseconds(100);
        public static float PlayerMoveSensitivity = 1f;
        public static float PlayerBulletSpeed = 2000f;
        public static float PlayerHitboxRadius = 5f;

        // Boss
        public static int BossDefaultLife = 1000;
        public static float BossDefaultSpeed = 200f;

        public static Color[] BossHPBarColors = new Color[]
        {
            Color.Green,
            Color.Orange,
            Color.OrangeRed,
            Color.Red
        };

        // Debug
        public static bool DisplayCollisionBoxes = true;
        public static bool EnableBloom = true;
        public static bool ShowDebugInfo = true;
    }
}