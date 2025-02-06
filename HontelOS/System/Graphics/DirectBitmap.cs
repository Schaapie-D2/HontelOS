/*
* PROJECT:          HontelOS
* CONTENT:          Direct bitmap (used for compositing)
* PROGRAMMERS:      Valentin Charbonnier <valentinbreiz@gmail.com>
* MODIFIED BY:      Jort van Dalen
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using Cosmos.Core;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;

namespace HontelOS.System.Graphics
{
    public unsafe class DirectBitmap
    {
        /// <summary>
        /// Stride.
        /// </summary>
        internal int Stride;

        /// <summary>
        /// Pitch.
        /// </summary>
        internal int Pitch;

        public Bitmap Bitmap { get; private set; }
        public int Height = 144;
        public int Width = 160;

        public DirectBitmap()
        {
            Bitmap = new Bitmap((uint)Width, (uint)Height, ColorDepth.ColorDepth32);
            Stride = (int)32 / 8;
            Pitch = (int)Width * Stride;
        }

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Bitmap = new Bitmap((uint)Width, (uint)Height, ColorDepth.ColorDepth32);
            Stride = (int)32 / 8;
            Pitch = (int)Width * Stride;
        }

        public void SetSize(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Width and height must be positive values.");

            Width = width;
            Height = height;
            Bitmap = new Bitmap((uint)Width, (uint)Height, ColorDepth.ColorDepth32);
            Stride = 32 / 8;
            Pitch = Width * Stride;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixel(int x, int y, Color color)
        {
            int index = x + y * Width;
            Bitmap.RawData[index] = (color.A << 24) | (color.R << 16) | (color.G << 8) | color.B;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixelAlpha(int x, int y, Color color)
        {
            int index = x + y * Width;

            if (index < Bitmap.RawData.Length)
            {
                if (color.A == 255) // Fully opaque
                {
                    Bitmap.RawData[index] = (color.A << 24) | (color.R << 16) | (color.G << 8) | color.B;
                    return;
                }

                int bgColor = Bitmap.RawData[index];
                int alpha = color.A;
                int invAlpha = 255 - alpha;
                int newRed = (color.R * alpha + ((bgColor >> 16) & 0xff) * invAlpha) >> 8;
                int newGreen = (color.G * alpha + ((bgColor >> 8) & 0xff) * invAlpha) >> 8;
                int newBlue = (color.B * alpha + (bgColor & 0xff) * invAlpha) >> 8;

                Bitmap.RawData[index] = (alpha << 24) | (newRed << 16) | (newGreen << 8) | newBlue;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Color GetPixel(int x, int y)
        {
            int index = x + y * Width;
            return Color.FromArgb(Bitmap.RawData[index]);
        }

        public void Clear(Color color)
        {
            int argb = color.ToArgb();
            fixed (int* destPtr = Bitmap.RawData)
            {
                MemoryOperations.Fill(destPtr, argb, Bitmap.RawData.Length);
            }
        }

        public void DrawString(string str, Font font, Color color, int x, int y)
        {
            int length = str.Length;
            byte width = font.Width;
            for (int i = 0; i < length; i++)
            {
                DrawChar(str[i], font, color, x, y);
                x += width;
            }
        }

        public void DrawChar(char c, Font font, Color color, int x, int y)
        {
            byte height = font.Height;
            byte width = font.Width;
            byte[] data = font.Data;
            int num = height * (byte)c;
            for (int i = 0; i < height; i++)
            {
                for (byte b = 0; b < width; b = (byte)(b + 1))
                {
                    if (font.ConvertByteToBitAddress(data[num + i], b + 1))
                    {
                        SetPixelAlpha((ushort)(x + b), (ushort)(y + i), color);
                    }
                }
            }
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="color">The color to draw with.</param>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        public virtual void DrawRectangle(Color color, int x, int y, int width, int height)
        {
            /*
             * we must draw four lines connecting any vertex of our rectangle to do this we first obtain the position of these
             * vertex (we call these vertexes A, B, C, D as for geometric convention)
             */

            /* The check of the validity of x and y are done in DrawLine() */

            /* The vertex A is where x,y are */
            int xa = x;
            int ya = y;

            /* The vertex B has the same y coordinate of A but x is moved of width pixels */
            int xb = x + width;
            int yb = y;

            /* The vertex C has the same x coordiate of A but this time is y that is moved of height pixels */
            int xc = x;
            int yc = y + height;

            /* The Vertex D has x moved of width pixels and y moved of height pixels */
            int xd = x + width;
            int yd = y + height;

            /* Draw a line betwen A and B */
            DrawLine(color, xa, ya, xb, yb);

            /* Draw a line between A and C */
            DrawLine(color, xa, ya, xc, yc);

            /* Draw a line between B and D */
            DrawLine(color, xb, yb, xd, yd);

            /* Draw a line between C and D */
            DrawLine(color, xc, yc, xd, yd);
        }

        public void DrawFilledRectangle(Color color, int xStart, int yStart, int width, int height)
        {
            if (height == -1)
            {
                height = width;
            }

            for (int i = yStart; i < yStart + height; i++)
            {
                DrawLine(color, xStart, i, xStart + width - 1, i);
            }
        }

        public void DrawLine(Color color, int x1, int y1, int x2, int y2)
        {
            TrimLine(ref x1, ref y1, ref x2, ref y2);
            int num = x2 - x1;
            int num2 = y2 - y1;
            if (num2 == 0)
            {
                DrawHorizontalLine(color, num, x1, y1);
            }
            else if (num == 0)
            {
                DrawVerticalLine(color, num2, x1, y1);
            }
            else
            {
                DrawDiagonalLine(color, num, num2, x1, y1);
            }
        }

        internal void DrawDiagonalLine(Color color, int dx, int dy, int x1, int y1)
        {
            int num = Math.Abs(dx);
            int num2 = Math.Abs(dy);
            int num3 = Math.Sign(dx);
            int num4 = Math.Sign(dy);
            int num5 = num2 >> 1;
            int num6 = num >> 1;
            int num7 = x1;
            int num8 = y1;
            if (num >= num2)
            {
                for (int i = 0; i < num; i++)
                {
                    num6 += num2;
                    if (num6 >= num)
                    {
                        num6 -= num;
                        num8 += num4;
                    }

                    num7 += num3;
                    SetPixelAlpha(num7, num8, color);
                }

                return;
            }

            for (int i = 0; i < num2; i++)
            {
                num5 += num;
                if (num5 >= num2)
                {
                    num5 -= num2;
                    num7 += num3;
                }

                num8 += num4;
                SetPixelAlpha(num7, num8, color);
            }
        }

        internal void DrawVerticalLine(Color color, int dy, int x1, int y1)
        {
            for (int i = 0; i < dy; i++)
            {
                SetPixelAlpha(x1, y1 + i, color);
            }
        }

        internal void DrawHorizontalLine(Color color, int dx, int x1, int y1)
        {
            for (int i = 0; i < dx; i++)
            {
                SetPixelAlpha(x1 + i, y1, color);
            }
        }

        protected void TrimLine(ref int x1, ref int y1, ref int x2, ref int y2)
        {
            if (x1 == x2)
            {
                x1 = Math.Min((int)(Width - 1), Math.Max(0, x1));
                x2 = x1;
                y1 = Math.Min((int)(Height - 1), Math.Max(0, y1));
                y2 = Math.Min((int)(Height - 1), Math.Max(0, y2));
                return;
            }

            float num = x1;
            float num2 = y1;
            float num3 = x2;
            float num4 = y2;
            float num5 = (num4 - num2) / (num3 - num);
            float num6 = num2 - num5 * num;
            if (num < 0f)
            {
                num = 0f;
                num2 = num6;
            }
            else if (num >= (float)Width)
            {
                num = Width - 1;
                num2 = (float)(Width - 1) * num5 + num6;
            }

            if (num3 < 0f)
            {
                num3 = 0f;
                num4 = num6;
            }
            else if (num3 >= (float)Width)
            {
                num3 = Width - 1;
                num4 = (float)(Width - 1) * num5 + num6;
            }

            if (num2 < 0f)
            {
                num = (0f - num6) / num5;
                num2 = 0f;
            }
            else if (num2 >= (float)Height)
            {
                num = ((float)(Height - 1) - num6) / num5;
                num2 = Height - 1;
            }

            if (num4 < 0f)
            {
                num3 = (0f - num6) / num5;
                num4 = 0f;
            }
            else if (num4 >= (float)Height)
            {
                num3 = ((float)(Height - 1) - num6) / num5;
                num4 = Height - 1;
            }

            if (num < 0f || num >= (float)Width || num2 < 0f || num2 >= (float)Height)
            {
                num = 0f;
                num3 = 0f;
                num2 = 0f;
                num4 = 0f;
            }

            if (num3 < 0f || num3 >= (float)Width || num4 < 0f || num4 >= (float)Height)
            {
                num = 0f;
                num3 = 0f;
                num2 = 0f;
                num4 = 0f;
            }

            x1 = (int)num;
            y1 = (int)num2;
            x2 = (int)num3;
            y2 = (int)num4;
        }

        public void DrawImage(Image image, int x, int y)
        {
            for (int yi = 0; yi < image.Height; yi++)
            {
                int destOffset = ((y + yi) * (int)Bitmap.Width + x);
                int srcOffset = yi * (int)image.Width;
                int count = (int)image.Width;

                MemoryOperations.Copy(Bitmap.RawData, destOffset, image.RawData, srcOffset, count);
            }
        }

        public void DrawImage(Image image, int x, int y, int w, int h)
        {
            Color color;

            int[] pixels = ScaleImage(image, w, h);
            var maxWidth = Math.Min(w, Width - x);
            var maxHeight = Math.Min(h, Height - y);
            for (int xi = 0; xi < maxWidth; xi++)
            {
                for (int yi = 0; yi < maxHeight; yi++)
                {
                    color = Color.FromArgb(pixels[xi + (yi * w)]);
                    SetPixelAlpha(x + xi, y + yi, color);
                }
            }
        }

        int[] ScaleImage(Image image, int newWidth, int newHeight)
        {
            int[] pixels = image.RawData;
            int w1 = (int)image.Width;
            int h1 = (int)image.Height;
            int[] temp = new int[newWidth * newHeight];
            int xRatio = (int)((w1 << 16) / newWidth) + 1;
            int yRatio = (int)((h1 << 16) / newHeight) + 1;
            int x2, y2;
            for (int i = 0; i < newHeight; i++)
            {
                for (int j = 0; j < newWidth; j++)
                {
                    x2 = (j * xRatio) >> 16;
                    y2 = (i * yRatio) >> 16;
                    temp[(i * newWidth) + j] = pixels[(y2 * w1) + x2];
                }
            }
            return temp;
        }

        public Bitmap ExtractImage(int srcX, int srcY, int width, int height)
        {
            Bitmap bmp = new((uint)width, (uint)height, ColorDepth.ColorDepth32);

            for (int yi = 0; yi < height; yi++)
            {
                int destOffset = yi * width;
                int srcOffset = ((srcY + yi) * (int)Bitmap.Width + srcX);
                int count = width;

                MemoryOperations.Copy(bmp.RawData, destOffset, Bitmap.RawData, srcOffset, count);
            }

            return bmp;
        }

        public static void AlphaBlendSSE(uint* dest, int dbpl, uint* src, int sbpl, int width, int height)
        {
            // PLUGGED
        }

        public static void AlphaBltSSE2(byte* dst, byte* src, int w, int h, int wmul4)
        {
        }

        public static void OpacitySSE(uint* pixelPtr, int w, int h, int bpl, uint a)
        {
            // PLUGGED
        }

        public void DrawImageAlpha(Bitmap image, int x, int y, byte alpha = 0xFF)
        {
            if (image.RawData.Length > Bitmap.RawData.Length)
            {
                return;
            }

            if ((y + image.Height > Bitmap.Height || y < 0) && (x + image.Width < Bitmap.Width || x > 0))
            {
                return;
            }

            Bitmap tmp = ExtractImage(x, y, (int)image.Width, (int)image.Height);
            if (tmp.Width == 0) return;

            fixed (int* bgBitmap = tmp.RawData)
            fixed (int* fgBitmap = image.RawData)
            {
                if (alpha < 0xFF)
                {
                    OpacitySSE((uint*)fgBitmap, (int)image.Width, (int)image.Height, (int)image.Width * 4, alpha);
                }

                // AlphaBltSSE2((byte*)bgBitmap, (byte*)fgBitmap, w, (int)image.Height, wmul4);
                AlphaBlendSSE((uint*)bgBitmap, (int)image.Width * 4, (uint*)fgBitmap, (int)image.Width * 4, (int)image.Width, (int)image.Height);
            }

            DrawImage(tmp, x, y);
        }

        public void DrawImageStretchAlpha(Image image, Rectangle sourceRect, Rectangle destRect)
        {
            float scaleX = (float)sourceRect.Width / destRect.Width;
            float scaleY = (float)sourceRect.Height / destRect.Height;

            for (int xi = 0; xi < destRect.Width; xi++)
            {
                for (int yi = 0; yi < destRect.Height; yi++)
                {
                    int srcX = (int)(xi * scaleX) + sourceRect.Left;
                    int srcY = (int)(yi * scaleY) + sourceRect.Top;

                    srcX = Math.Min(srcX, sourceRect.Right - 1);
                    srcY = Math.Min(srcY, sourceRect.Bottom - 1);

                    int color = image.RawData[srcX + srcY * image.Width];
                    SetPixelAlpha(destRect.Left + xi, destRect.Top + yi, Color.FromArgb(color));
                }
            }
        }

        /// <summary>
        /// Draws a filled circle at the given coordinates with the given radius.
        /// </summary>
        /// <param name="color">The color to draw with.</param>
        /// <param name="x0">The X center coordinate.</param>
        /// <param name="y0">The Y center coordinate.</param>
        /// <param name="radius">The radius of the circle to draw.</param>
        public void DrawFilledCircle(Color color, int x0, int y0, int radius)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    if (x * x + y * y <= radius * radius)
                    {
                        int drawX = x0 + x;
                        int drawY = y0 + y;
                        if (drawX >= 0 && drawX < Width && drawY >= 0 && drawY < Height)
                        {
                            SetPixelAlpha(drawX, drawY, color);
                        }
                    }
                }
            }
        }

        public void DrawFilledRoundedRectangle(Color color, int x, int y, int width, int height, int radius)
        {
            DrawFilledRectangle(color, x + radius, y, width - 2 * radius, height);
            DrawFilledRectangle(color, x, y + radius, radius + 1, height - 2 * radius);
            DrawFilledRectangle(color, x + width - radius - 1, y + radius, radius + 2, height - 2 * radius);
            DrawFilledCircle(color, x + radius, y + radius, radius);
            DrawFilledCircle(color, x + width - radius - 1, y + radius, radius);
            DrawFilledCircle(color, x + radius, y + height - radius - 1, radius);
            DrawFilledCircle(color, x + width - radius - 1, y + height - radius - 1, radius);
        }

        public void DrawRoundedRectangle(Color color, int x, int y, int width, int height, int radius)
        {
            // Draw horizontal lines
            DrawLine(color, x + radius, y, x + width - radius, y); // Top horizontal line
            DrawLine(color, x + radius, y + height, x + width - radius, y + height); // Bottom horizontal line

            // Draw vertical lines
            DrawLine(color, x, y + radius, x, y + height - radius); // Left vertical line
            DrawLine(color, x + width, y + radius, x + width, y + height - radius); // Right vertical line

            // Draw the four corner arcs
            DrawArc(color, x + radius, y + radius, radius, 180, 270); // Top-left corner
            DrawArc(color, x + width - radius, y + radius, radius, 270, 360); // Top-right corner
            DrawArc(color, x + radius, y + height - radius, radius, 90, 180); // Bottom-left corner
            DrawArc(color, x + width - radius, y + height - radius, radius, 0, 90); // Bottom-right corner
        }

        public void DrawFilledTopRoundedRectangle(Color color, int x, int y, int width, int height, int radius)
        {
            DrawFilledRectangle(color, x + radius, y, width - 2 * radius, height);
            DrawFilledRectangle(color, x, y + radius, width, height - radius);
            DrawFilledCircle(color, x + radius, y + radius, radius);
            DrawFilledCircle(color, x + width - radius - 1, y + radius, radius);
        }

        public void DrawFilledBottomRoundedRectangle(Color color, int x, int y, int width, int height, int radius)
        {
            DrawFilledRectangle(color, x + radius, y, width - 2 * radius, height);
            DrawFilledRectangle(color, x, y, width, height - radius);
            DrawFilledCircle(color, x + radius, y + height - radius - 1, radius);
            DrawFilledCircle(color, x + width - radius - 1, y + height - radius - 1, radius);
        }

        public void DrawArc(Color color, int centerX, int centerY, int radius, double startAngle, double endAngle)
        {
            // Convert angles from degrees to radians
            double startRad = startAngle * (Math.PI / 180);
            double endRad = endAngle * (Math.PI / 180);

            for (double angle = startRad; angle <= endRad; angle += 0.01) // Increment angle for smoothness
            {
                int x = centerX + (int)(radius * Math.Cos(angle));
                int y = centerY + (int)(radius * Math.Sin(angle));
                SetPixel(x, y, color);
            }
        }
    }
}