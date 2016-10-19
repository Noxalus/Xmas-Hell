using System;
using Microsoft.Xna.Framework;

namespace Xmas_Hell
{
    public static class Config
    {
        public static Point VirtualResolution = new Point(720, 1280);
        public static float PlayerSpeed = 50f; // Pixel / second
        public static readonly TimeSpan PlayerShootFrequency = TimeSpan.FromMilliseconds(100);
    }
}