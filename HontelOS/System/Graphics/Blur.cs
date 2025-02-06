/*
* PROJECT:          HontelOS
* CONTENT:          Blur effect generator
* PROGRAMMERS:      Jort van Dalen
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

using Cosmos.System.Graphics;
using System;

namespace HontelOS.System.Graphics
{
    public static class Blur
    {
        public static Bitmap GenerateFastBlur(Bitmap bmp, int radial)
        {
            var bm = bmp;
            bm.FastBlur(radial);
            return bm;
        }

        public static void FastBlur(this Bitmap bmp, int radial)
        {
            int width = (int)bmp.Width;
            int height = (int)bmp.Height;
            int[] rawData = bmp.RawData;

            if (radial < 1 || width < 2 * radial || height < 2 * radial) return;

            int[] newAlpha = new int[width * height];
            int[] newRed = new int[width * height];
            int[] newGreen = new int[width * height];
            int[] newBlue = new int[width * height];

            // Separate channels
            SeparateChannels(rawData, newAlpha, newRed, newGreen, newBlue);

            // Perform Gaussian blur on each channel
            int[] temp = new int[width * height];
            GaussBlur(newRed, temp, width, height, radial);
            GaussBlur(newGreen, temp, width, height, radial);
            GaussBlur(newBlue, temp, width, height, radial);

            // Combine channels back into bitmap
            CombineChannels(rawData, newAlpha, newRed, newGreen, newBlue);

            // Copy blurred data back to the bitmap
            for (int i = 0; i < rawData.Length; i++)
            {
                int a = 255;
                int r = Clamp(newRed[i], 0, 255);
                int g = Clamp(newGreen[i], 0, 255);
                int b = Clamp(newBlue[i], 0, 255);
                rawData[i] = (a << 24) | (r << 16) | (g << 8) | b;
            }
        }

        private static void SeparateChannels(int[] rawData, int[] alpha, int[] red, int[] green, int[] blue)
        {
            for (int i = 0; i < rawData.Length; i++)
            {
                alpha[i] = (int)((rawData[i] & 0xff000000) >> 24);
                red[i] = (rawData[i] & 0x00ff0000) >> 16;
                green[i] = (rawData[i] & 0x0000ff00) >> 8;
                blue[i] = rawData[i] & 0x000000ff;
            }
        }

        private static void CombineChannels(int[] rawData, int[] alpha, int[] red, int[] green, int[] blue)
        {
            for (int i = 0; i < rawData.Length; i++)
            {
                int a = Clamp(alpha[i], 0, 255);
                int r = Clamp(red[i], 0, 255);
                int g = Clamp(green[i], 0, 255);
                int b = Clamp(blue[i], 0, 255);
                rawData[i] = (a << 24) | (r << 16) | (g << 8) | b;
            }
        }
        private static void GaussBlur(int[] source, int[] temp, int width, int height, int radius)
        {
            int[] boxSizes = BoxesForGauss(radius, 3);
            for (int i = 0; i < 3; i++)
            {
                BoxBlur(source, temp, width, height, (boxSizes[i] - 1) / 2);
            }
        }
        private static void BoxBlur(int[] source, int[] dest, int width, int height, int r)
        {
            for (int i = 0; i < source.Length; i++) dest[i] = source[i];
            BoxBlurH(source, dest, width, height, r);
            BoxBlurV(dest, source, width, height, r);
        }
        private static void BoxBlurH(int[] source, int[] dest, int width, int height, int r)
        {
            double iar = 1.0 / (r + r + 1);
            for (int i = 0; i < height; i += 2) // Blur every second row
            {
                int ti = i * width, li = ti, ri = ti + r;
                int fv = source[ti], lv = source[ti + width - 1], val = (r + 1) * fv;
                for (int j = 0; j < r; j++) val += source[ti + j];
                for (int j = 0; j <= r; j++) { val += source[ri++] - fv; dest[ti++] = (int)Math.Round(val * iar); }
                for (int j = r + 1; j < width - r; j++) { val += source[ri++] - source[li++]; dest[ti++] = (int)Math.Round(val * iar); }
                for (int j = width - r; j < width; j++) { val += lv - source[li++]; dest[ti++] = (int)Math.Round(val * iar); }
            }

            // Copy blurred rows to the rows that were skipped
            for (int i = 1; i < height; i += 2)
            {
                int ti = i * width;
                int tiAbove = (i - 1) * width;
                Array.Copy(dest, tiAbove, dest, ti, width);
            }
        }
        private static void BoxBlurV(int[] source, int[] dest, int width, int height, int r)
        {
            double iar = 1.0 / (r + r + 1);
            for (int i = 0; i < width; i += 2) // Blur every second column
            {
                int ti = i, li = ti, ri = ti + (r * width);
                int fv = source[ti], lv = source[ti + (width * (height - 1))], val = (r + 1) * fv;
                for (int j = 0; j < r; j++) val += source[ti + (j * width)];
                for (int j = 0; j <= r; j++) { val += source[ri] - fv; dest[ti] = (int)Math.Round(val * iar); ri += width; ti += width; }
                for (int j = r + 1; j < height - r; j++) { val += source[ri] - source[li]; dest[ti] = (int)Math.Round(val * iar); li += width; ri += width; ti += width; }
                for (int j = height - r; j < height; j++) { val += lv - source[li]; dest[ti] = (int)Math.Round(val * iar); li += width; ti += width; }
            }

            // Copy blurred columns to the columns that were skipped
            for (int i = 1; i < width; i += 2)
            {
                for (int j = 0; j < height; j++)
                {
                    dest[(j * width) + i] = dest[(j * width) + i - 1];
                }
            }
        }
        private static int[] BoxesForGauss(int sigma, int n)
        {
            double wIdeal = Math.Sqrt((12 * sigma * sigma / n) + 1);
            int wl = (int)Math.Floor(wIdeal);
            if (wl % 2 == 0) wl--;
            int wu = wl + 2;

            double mIdeal = ((12 * sigma * sigma) - (n * wl * wl) - (4 * n * wl) - (3 * n)) / ((-4 * wl) - 4);
            int m = (int)Math.Round(mIdeal);

            int[] sizes = new int[n];
            for (int i = 0; i < n; i++) sizes[i] = i < m ? wl : wu;
            return sizes;
        }
        private static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}