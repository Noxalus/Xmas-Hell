using System;
using Microsoft.Xna.Framework;

namespace Xmas_Hell
{
    public static class GameConfig
    {
        public static Point VirtualResolution = new Point(720, 1280);
        public static readonly TimeSpan PlayerShootFrequency = TimeSpan.FromMilliseconds(100);
        public static float PlayerMoveSensitivity = 1f;
        public static float PlayerBulletSpeed = 2000f;
        public static int BossDefaultLife = 1000;
        public static bool DisplayCollisionBoxes = false;
    }
}