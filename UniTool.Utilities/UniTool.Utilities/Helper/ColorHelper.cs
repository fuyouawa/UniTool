using System;
using UnityEngine;

namespace UniTool.Utilities
{
    public static class ColorHelper
    {
        public static string ToHex(this Color color)
        {
            Color32 c = color;
            return ToHex(c);
        }

        public static string ToHex(this Color32 color)
        {
            return $"#{color.r:X2}{color.g:X2}{color.b:X2}{color.a:X2}";
        }

        public static Color32 FromHex(string hex)
        {
            if (hex.IsNullOrEmpty())
                throw new ArgumentException("Hex string cannot be null or empty.");

            hex = hex.TrimStart('#');

            if (hex.Length != 6 && hex.Length != 8)
                throw new ArgumentException("Hex string must be 6 or 8 characters long.");

            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 4), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 6), System.Globalization.NumberStyles.HexNumber);
            byte a = 255; // Default alpha value (fully opaque)

            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(6, 8), System.Globalization.NumberStyles.HexNumber);
            }

            return new Color32(r, g, b, a);
        }
    }
}