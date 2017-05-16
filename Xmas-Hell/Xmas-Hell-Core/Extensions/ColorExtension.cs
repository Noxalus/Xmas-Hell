using System;
using Microsoft.Xna.Framework;

namespace XmasHell.Extensions
{
    public static class ColorExtension
    {
        public static Color FromHex(String hex)
        {
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            if (hex.Length != 6) throw new Exception("Color not valid");

            return new Color(
                int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber)
            );
        }
    }
}