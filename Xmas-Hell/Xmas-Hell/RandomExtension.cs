using System;

namespace RandomExtension
{
    public static class RandomExtension
    {
        public static double NextDouble(this Random rand, double min, double max)
        {
            return rand.NextDouble() * (max - min) + min;
        }
    }
}