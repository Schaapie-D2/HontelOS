using Cosmos.System.Graphics;
using System;
using System.Drawing;

namespace HontelOS.System.Graphics
{
    public static class CanvasUtils
    {
        // From Szymekk44's Cosmos optimization kit with modifications
        public static void DrawFilledRoundedRectangle(this Canvas c, Color color, int x, int y, int width, int height, int radius)
        {
            c.DrawFilledRectangle(color, x + radius, y, width - 2 * radius, height, true);
            c.DrawFilledRectangle(color, x, y + radius, radius, height - 2 * radius, true);
            c.DrawFilledRectangle(color, x + width - radius, y + radius, radius, height - 2 * radius, true);
            c.DrawFilledCircle(color, x + radius, y + radius, radius);
            c.DrawFilledCircle(color, x + width - radius - 1, y + radius, radius);
            c.DrawFilledCircle(color, x + radius, y + height - radius - 1, radius);
            c.DrawFilledCircle(color, x + width - radius - 1, y + height - radius - 1, radius);
        }
        // From Szymekk44's Cosmos optimization kit with modifications
        public static void DrawRoundedRectangle(this Canvas c, Color color, int x, int y, int width, int height, int radius)
        {
            // Draw horizontal lines
            c.DrawLine(color, x + radius, y, x + width - radius, y); // Top horizontal line
            c.DrawLine(color, x + radius, y + height, x + width - radius, y + height); // Bottom horizontal line

            // Draw vertical lines
            c.DrawLine(color, x, y + radius, x, y + height - radius); // Left vertical line
            c.DrawLine(color, x + width, y + radius, x + width, y + height - radius); // Right vertical line

            // Draw the four corner arcs
            c.DrawArc(x + radius, y + radius, radius, radius, color, 180, 270); // Top-left corner
            c.DrawArc(x + width - radius, y + radius, radius, radius, color, 270, 360); // Top-right corner
            c.DrawArc(x + radius, y + height - radius, radius, radius, color, 90, 180); // Bottom-left corner
            c.DrawArc(x + width - radius, y + height - radius, radius, radius, color, 0, 90); // Bottom-right corner
        }
        // From Szymekk44's Cosmos optimization kit with modifications
        public static void DrawFilledTopRoundedRectangle(this Canvas c, Color color, int x, int y, int width, int height, int radius)
        {
            c.DrawFilledRectangle(color, x + radius, y, width - 2 * radius, height, true);
            c.DrawFilledRectangle(color, x, y + radius, width, height - radius, true);
            c.DrawFilledCircle(color, x + radius, y + radius, radius);
            c.DrawFilledCircle(color, x + width - radius - 1, y + radius, radius);
        }

        public static void DrawFilledBottomRoundedRectangle(this Canvas c, Color color, int x, int y, int width, int height, int radius)
        {
            c.DrawFilledRectangle(color, x + radius, y, width - 2 * radius, height, true);
            c.DrawFilledRectangle(color, x, y, width, height - radius, true);
            c.DrawFilledCircle(color, x + radius, y + height - radius - 1, radius);
            c.DrawFilledCircle(color, x + width - radius - 1, y + height - radius - 1, radius);
        }

        public static Bitmap ScaleImage(Bitmap image, int width, int height)
        {
            if (image.Width == width && image.Height == height)
                return image;

            int[] pixels = image.RawData;
            int w1 = (int)image.Width;
            int h1 = (int)image.Height;
            int[] temp = new int[width * height];
            int xRatio = (int)((w1 << 16) / width) + 1;
            int yRatio = (int)((h1 << 16) / height) + 1;
            int x2, y2;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    x2 = (j * xRatio) >> 16;
                    y2 = (i * yRatio) >> 16;
                    temp[(i * width) + j] = pixels[(y2 * w1) + x2];
                }
            }

            var nBMP = new Bitmap((uint)width, (uint)height, ColorDepth.ColorDepth32);
            nBMP.RawData = temp;
            return nBMP;
        }

        // From Szymekk44's Waterfall-Core github repository
        public static Bitmap Blend(this Bitmap background, Bitmap image, byte alpha, bool blendImageAlpha)
        {
            // Create a new bitmap for the blended result with the size of the background
            Bitmap blendedBitmap = new Bitmap(background.Width, background.Height, ColorDepth.ColorDepth32);

            int[] blendedData = new int[background.Width * background.Height];

            byte alphaWeight = alpha;
            byte inverseAlphaWeight = (byte)(255 - alpha);

            int bgWidth = (int)background.Width;
            int bgHeight = (int)background.Height;
            int imgWidth = (int)image.Width;
            int imgHeight = (int)image.Height;

            // Precompute alpha ratios using bit shifts
            float alphaWeightRatio = alphaWeight / 255f;
            float inverseAlphaWeightRatio = inverseAlphaWeight / 255f;

            for (int y = 0; y < bgHeight; y++)
            {
                for (int x = 0; x < bgWidth; x++)
                {
                    int bgIndex = (y * bgWidth) + x;
                    int imgIndex = (y < imgHeight && x < imgWidth) ? (y * imgWidth) + x : -1;

                    int bgColor = background.RawData[bgIndex];
                    int imgColor = imgIndex >= 0 ? image.RawData[imgIndex] : bgColor;

                    byte bgAlpha = (byte)((bgColor >> 24) & 0xFF);
                    byte bgRed = (byte)((bgColor >> 16) & 0xFF);
                    byte bgGreen = (byte)((bgColor >> 8) & 0xFF);
                    byte bgBlue = (byte)(bgColor & 0xFF);

                    byte imgAlpha = (byte)((imgColor >> 24) & 0xFF);
                    byte imgRed = (byte)((imgColor >> 16) & 0xFF);
                    byte imgGreen = (byte)((imgColor >> 8) & 0xFF);
                    byte imgBlue = (byte)(imgColor & 0xFF);

                    byte resultAlpha;
                    byte resultRed;
                    byte resultGreen;
                    byte resultBlue;

                    if (blendImageAlpha)
                    {
                        if (imgAlpha == 0)
                        {
                            resultAlpha = bgAlpha;
                            resultRed = bgRed;
                            resultGreen = bgGreen;
                            resultBlue = bgBlue;
                        }
                        else
                        {
                            // Precompute normalized alphas
                            float imgAlphaNormalized = imgAlpha / 255f;
                            float bgAlphaNormalized = bgAlpha / 255f;

                            // Precompute the blending factors
                            float imgAlphaWeight = alphaWeightRatio * imgAlphaNormalized;
                            float bgAlphaWeight = inverseAlphaWeightRatio * bgAlphaNormalized;

                            resultAlpha = (byte)(Math.Min(1.0f, imgAlphaWeight + bgAlphaWeight) * 255);
                            resultRed = (byte)((((imgAlphaNormalized * imgRed) + ((1 - imgAlphaNormalized) * bgRed)) * alphaWeightRatio) + (bgRed * inverseAlphaWeightRatio));
                            resultGreen = (byte)((((imgAlphaNormalized * imgGreen) + ((1 - imgAlphaNormalized) * bgGreen)) * alphaWeightRatio) + (bgGreen * inverseAlphaWeightRatio));
                            resultBlue = (byte)((((imgAlphaNormalized * imgBlue) + ((1 - imgAlphaNormalized) * bgBlue)) * alphaWeightRatio) + (bgBlue * inverseAlphaWeightRatio));
                        }
                    }
                    else
                    {
                        resultAlpha = (byte)(((alphaWeight << 8) + (inverseAlphaWeight * bgAlpha)) >> 8);
                        resultRed = (byte)(((alphaWeight * imgRed) + (inverseAlphaWeight * bgRed)) >> 8);
                        resultGreen = (byte)(((alphaWeight * imgGreen) + (inverseAlphaWeight * bgGreen)) >> 8);
                        resultBlue = (byte)(((alphaWeight * imgBlue) + (inverseAlphaWeight * bgBlue)) >> 8);
                    }

                    blendedData[bgIndex] = (resultAlpha << 24) | (resultRed << 16) | (resultGreen << 8) | resultBlue;
                }
            }

            blendedBitmap.RawData = blendedData;

            return blendedBitmap;
        }
        // From Szymekk44's Waterfall-Core github repository with modifications
        public static Bitmap GetImage(this Bitmap bmp, int x, int y, int width, int height)
        {
            int widthCLeft = (int)(x + width - Kernel.screenWidth);
            int heightCLeft = (int)(y + height - Kernel.screenHeight);
            if (widthCLeft > 0)
                width -= widthCLeft;
            if (heightCLeft > 0)
                height -= heightCLeft;

            Bitmap extractedImage = new Bitmap((uint)width, (uint)height, ColorDepth.ColorDepth32);
            for (int i = 0; i < height; i++)
            {
                int bmpStartIndex = x + ((y + i) * (int)bmp.Width);
                int imageStartIndex = 0 + (i * (int)extractedImage.Width);
                Array.Copy(bmp.RawData, bmpStartIndex, extractedImage.RawData, imageStartIndex, width);
            }
            return extractedImage;
        }

        // From Szymekk44's Waterfall-Core github repository
        /// <summary>
        /// Modifies the colors of the pixels in the bitmap, shifting them towards a target color with a specified intensity.
        /// </summary>
        /// <param name="bitmap">The bitmap to modify.</param>
        /// <param name="targetColor">The target color to shift the pixels towards.</param>
        /// <param name="intensity">The intensity of the color shift, ranging from 0.0f (no change) to 1.0f (full change).</param>
        /// <returns>A new bitmap with the modified colors.</returns>
        public static void ModifyColor(this Bitmap bitmap, Color targetColor, float intensity)
        {
            if (intensity < 0.0f || intensity > 1.0f)
                return;

            int[] originalData = bitmap.RawData;

            int[] bmpData = new int[originalData.Length];
            Array.Copy(originalData, bmpData, originalData.Length);

            for (int i = 0; i < bmpData.Length; i++)
            {
                int color = bmpData[i];

                byte a = (byte)((color >> 24) & 0xFF);
                byte r = (byte)((color >> 16) & 0xFF);
                byte g = (byte)((color >> 8) & 0xFF);
                byte b = (byte)(color & 0xFF);

                int deltaR = targetColor.R - r;
                int deltaG = targetColor.G - g;
                int deltaB = targetColor.B - b;

                deltaR = (int)(deltaR * intensity);
                deltaG = (int)(deltaG * intensity);
                deltaB = (int)(deltaB * intensity);

                r = (byte)Math.Max(0, Math.Min(255, r + deltaR));
                g = (byte)Math.Max(0, Math.Min(255, g + deltaG));
                b = (byte)Math.Max(0, Math.Min(255, b + deltaB));

                bmpData[i] = (a << 24) | (r << 16) | (g << 8) | b;
            }
            bitmap.RawData = bmpData;
        }
    }
}