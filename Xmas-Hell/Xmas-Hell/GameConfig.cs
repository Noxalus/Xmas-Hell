using System;
using Microsoft.Xna.Framework;

namespace Xmas_Hell
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

        // Boss
        public static int BossDefaultLife = 1000;
        public static float BossDefaultSpeed = 200f;

        // Debug
        public static bool DisplayCollisionBoxes = false;
        public static bool DisableBloom = false;
    }
}