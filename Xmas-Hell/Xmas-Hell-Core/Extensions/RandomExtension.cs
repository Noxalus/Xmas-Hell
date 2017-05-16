using System;
using Microsoft.Xna.Framework;

namespace XmasHell.Extensions
{
    public static class RandomExtension
    {
        public static double NextDouble(this Random rand, double min, double max)
        {
            return rand.NextDouble() * (max - min) + min;
        }

        public static float NextFloat(this Random rand, float min, float max)
        {
            return (float)rand.NextDouble() * (max - min) + min;
        }

        public static Color NextColor(this Random rand)
        {
            return new Color(
                (byte) rand.Next(0, 255),
                (byte) rand.Next(0, 255),
                (byte) rand.Next(0, 255)
            );
        }
    }
}