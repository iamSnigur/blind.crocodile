using UnityEngine;

namespace BlindCrocodile.Gameplay
{
    public static class ColorUtils
    {
        public static Vector3 RGBToHSV(this Color rgbColor)
        {
            float min = Mathf.Min(rgbColor.r, Mathf.Min(rgbColor.g, rgbColor.b));
            float max = Mathf.Max(rgbColor.r, Mathf.Max(rgbColor.g, rgbColor.b));
            float delta = max - min;

            float hue = 0;
            if (delta != 0)
            {
                if (max == rgbColor.r)
                    hue = (rgbColor.g - rgbColor.b) / delta;
                else if (max == rgbColor.g)
                    hue = 2f + (rgbColor.b - rgbColor.r) / delta;
                else
                    hue = 4f + (rgbColor.r - rgbColor.g) / delta;

                hue *= 60;
                if (hue < 0) hue += 360;
            }

            float saturation = (max == 0) ? 0 : delta / max;

            return new Vector3(hue, saturation, max);
        }

        public static Color HSVToRGB(this Vector3 hsv)
        {
            float C = hsv.z * hsv.y;
            float X = C * (1 - Mathf.Abs((hsv.x / 60) % 2 - 1));
            float m = hsv.z - C;

            float r, g, b;
            if (hsv.x >= 0 && hsv.x < 60)
            {
                r = C; g = X; b = 0;
            }
            else if (hsv.x >= 60 && hsv.x < 120)
            {
                r = X; g = C; b = 0;
            }
            else if (hsv.x >= 120 && hsv.x < 180)
            {
                r = 0; g = C; b = X;
            }
            else if (hsv.x >= 180 && hsv.x < 240)
            {
                r = 0; g = X; b = C;
            }
            else if (hsv.x >= 240 && hsv.x < 300)
            {
                r = X; g = 0; b = C;
            }
            else
            {
                r = C; g = 0; b = X;
            }

            return new Color(r + m, g + m, b + m);
        }
    }
}