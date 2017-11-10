using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Text;

namespace XmasHell.Extensions
{
    public static class StringExtension
    {
        public static List<String> FormatBoxString(String text, int width, BitmapFont font)
        {
            var strings = new List<String>();
            var currentString = "";

            foreach (var letter in text)
            {
                if (letter.Equals(' '))
                {
                    if (font.MeasureString(currentString).Width >= width)
                    {
                        strings.Add(currentString);
                        currentString = "";
                        continue;
                    }
                }

                currentString += letter;
            }

            if (currentString.Length > 0)
                strings.Add(currentString);

            return strings;
        }
    }
}
