using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BlurredBackgroundPoc
{
    public static class WriteableBitmapConvolutionExtensions
    {
        private static byte[] _tinyGaussianBlurKernel = 
            {16,26,16,
             26,41,26,
             16,26,16};

        private static byte[] _gaussianBlurKernel = 
            { 1, 4, 7, 4,1,
              4,16,26,16,4,
              7,26,41,26,7,
              4,16,26,16,4,
              1, 4, 7, 4,1};

        public static void GaussianBlurTiny(this WriteableBitmap bmp)
        {
            bmp.Convolute(_tinyGaussianBlurKernel, 3, 3);
        }

        public static void GaussianBlur(this WriteableBitmap bmp)
        {
            bmp.Convolute(_gaussianBlurKernel, 5, 5);
        }

        private static byte[] _1dgaussianBlurKernel = { 1, 4, 7, 4, 1, };
        private static byte[] _tiny1dGaussianBlurKernel = { 4, 7, 4 };

        public static void GaussianBlurFast(this WriteableBitmap bmp)
        {
            bmp.ConvoluteX(_tiny1dGaussianBlurKernel);
            bmp.ConvoluteY(_tiny1dGaussianBlurKernel);
        }

        public static void Convolute(this WriteableBitmap bmp, byte[] kernel, int kernelWidth, int kernelHeight)
        {
            if ((kernelWidth & 1) == 0)
            {
                throw new InvalidOperationException("Kernel width must be odd!");
            }
            if ((kernelHeight & 1) == 0)
            {
                throw new InvalidOperationException("Kernel height must be odd!");
            }
            if (kernel.Length != kernelWidth * kernelHeight)
            {
                throw new InvalidOperationException("Kernel size doesn't match width*height!");
            }

            int[] pixels = bmp.Pixels;
            int w = bmp.PixelWidth;
            int h = bmp.PixelHeight;
            int index = 0;
            int halfKernelWidth = kernelWidth / 2;
            int halfKernelHeight = kernelHeight / 2;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int kernelSum = 0;
                    int r = 0;
                    int g = 0;
                    int b = 0;

                    for (int kx = -halfKernelWidth; kx <= halfKernelWidth; kx++)
                    {
                        int px = kx + x;
                        if (px < 0 || px >= w)
                        {
                            continue;
                        }

                        for (int ky = -halfKernelHeight; ky <= halfKernelHeight; ky++)
                        {
                            int py = ky + y;
                            if (py < 0 || py >= h)
                            {
                                continue;
                            }

                            int kernelIndex = (ky + halfKernelHeight) * kernelWidth + (kx + halfKernelWidth);
                            byte kernelWeight = kernel[kernelIndex];
                            kernelSum += kernelWeight;

                            int innerIndex = py * w + px;
                            int col = pixels[innerIndex];
                            r += ((byte)(col >> 16)) * kernelWeight;
                            g += ((byte)(col >> 8)) * kernelWeight;
                            b += ((byte)col) * kernelWeight;
                        }
                    }

                    byte br = (byte)(r / kernelSum);
                    byte bg = (byte)(g / kernelSum);
                    byte bb = (byte)(b / kernelSum);

                    int color =
                        (255 << 24)
                        | (br << 16)
                        | (bg << 8)
                        | (bb);

                    pixels[index++] = color;
                }
            }
        }

        public static void ConvoluteX(this WriteableBitmap bmp, byte[] kernel)
        {
            int kernelWidth = kernel.Length;
            if ((kernelWidth & 1) == 0)
            {
                throw new InvalidOperationException("Kernel width must be odd!");
            }

            int[] pixels = bmp.Pixels;
            int w = bmp.PixelWidth;
            int h = bmp.PixelHeight;
            int index = 0;
            int halfKernelWidth = kernelWidth / 2;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int kernelSum = 0;
                    int r = 0;
                    int g = 0;
                    int b = 0;

                    for (int kx = -halfKernelWidth; kx <= halfKernelWidth; kx++)
                    {
                        int px = kx + x;
                        if (px < 0 || px >= w)
                        {
                            continue;
                        }

                        int kernelIndex = kx + halfKernelWidth;
                        byte kernelWeight = kernel[kernelIndex];
                        kernelSum += kernelWeight;

                        int innerIndex = y * w + px;
                        int col = pixels[innerIndex];
                        r += ((byte)(col >> 16)) * kernelWeight;
                        g += ((byte)(col >> 8)) * kernelWeight;
                        b += ((byte)col) * kernelWeight;

                    }

                    byte br = (byte)(r / kernelSum);
                    byte bg = (byte)(g / kernelSum);
                    byte bb = (byte)(b / kernelSum);

                    int color =
                        (255 << 24)
                        | (br << 16)
                        | (bg << 8)
                        | (bb);

                    pixels[index++] = color;
                }
            }
        }

        public static void ConvoluteY(this WriteableBitmap bmp, byte[] kernel)
        {
            int kernelHeight = kernel.Length;
            if ((kernelHeight & 1) == 0)
            {
                throw new InvalidOperationException("Kernel height must be odd!");
            }

            int[] pixels = bmp.Pixels;
            int w = bmp.PixelWidth;
            int h = bmp.PixelHeight;
            int index = 0;
            int halfKernelHeight = kernelHeight / 2;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int kernelSum = 0;
                    int r = 0;
                    int g = 0;
                    int b = 0;

                    for (int ky = -halfKernelHeight; ky <= halfKernelHeight; ky++)
                    {
                        int py = ky + y;
                        if (py < 0 || py >= h)
                        {
                            continue;
                        }

                        int kernelIndex = (ky + halfKernelHeight);
                        byte kernelWeight = kernel[kernelIndex];
                        kernelSum += kernelWeight;

                        int innerIndex = py * w + x;
                        int col = pixels[innerIndex];
                        r += ((byte)(col >> 16)) * kernelWeight;
                        g += ((byte)(col >> 8)) * kernelWeight;
                        b += ((byte)col) * kernelWeight;
                    }

                    byte br = (byte)(r / kernelSum);
                    byte bg = (byte)(g / kernelSum);
                    byte bb = (byte)(b / kernelSum);

                    int color =
                        (255 << 24)
                        | (br << 16)
                        | (bg << 8)
                        | (bb);

                    pixels[index++] = color;
                }
            }
        }

        public static void BoxBlur(this WriteableBitmap bmp, int range)
        {
            if ((range & 1) == 0)
            {
                throw new InvalidOperationException("Range must be odd!");
            }

            bmp.BoxBlurHorizontal(range);
            bmp.BoxBlurVertical(range);
        }

        public static void BoxBlurHorizontal(this WriteableBitmap bmp, int range)
        {
            int[] pixels = bmp.Pixels;
            int w = bmp.PixelWidth;
            int h = bmp.PixelHeight;
            int halfRange = range / 2;
            int index = 0;
            int[] newColors = new int[w];

            for (int y = 0; y < h; y++)
            {
                int hits = 0;
                int r = 0;
                int g = 0;
                int b = 0;
                for (int x = -halfRange; x < w; x++)
                {
                    int oldPixel = x - halfRange - 1;
                    if (oldPixel >= 0)
                    {
                        int col = pixels[index + oldPixel];
                        if (col != 0)
                        {
                            r -= ((byte)(col >> 16));
                            g -= ((byte)(col >> 8));
                            b -= ((byte)col);
                        }
                        hits--;
                    }

                    int newPixel = x + halfRange;
                    if (newPixel < w)
                    {
                        int col = pixels[index + newPixel];
                        if (col != 0)
                        {
                            r += ((byte)(col >> 16));
                            g += ((byte)(col >> 8));
                            b += ((byte)col);
                        }
                        hits++;
                    }

                    if (x >= 0)
                    {
                        int color =
                            (255 << 24)
                            | ((byte)(r / hits) << 16)
                            | ((byte)(g / hits) << 8)
                            | ((byte)(b / hits));

                        newColors[x] = color;
                    }
                }

                for (int x = 0; x < w; x++)
                {
                    pixels[index + x] = newColors[x];
                }

                index += w;
            }
        }

        public static void BoxBlurVertical(this WriteableBitmap bmp, int range)
        {
            int[] pixels = bmp.Pixels;
            int w = bmp.PixelWidth;
            int h = bmp.PixelHeight;
            int halfRange = range / 2;

            int[] newColors = new int[h];
            int oldPixelOffset = -(halfRange + 1) * w;
            int newPixelOffset = (halfRange) * w;

            for (int x = 0; x < w; x++)
            {
                int hits = 0;
                int r = 0;
                int g = 0;
                int b = 0;
                int index = -halfRange * w + x;
                for (int y = -halfRange; y < h; y++)
                {
                    int oldPixel = y - halfRange - 1;
                    if (oldPixel >= 0)
                    {
                        int col = pixels[index + oldPixelOffset];
                        if (col != 0)
                        {
                            r -= ((byte)(col >> 16));
                            g -= ((byte)(col >> 8));
                            b -= ((byte)col);
                        }
                        hits--;
                    }

                    int newPixel = y + halfRange;
                    if (newPixel < h)
                    {
                        int col = pixels[index + newPixelOffset];
                        if (col != 0)
                        {
                            r += ((byte)(col >> 16));
                            g += ((byte)(col >> 8));
                            b += ((byte)col);
                        }
                        hits++;
                    }

                    if (y >= 0)
                    {
                        int color =
                            (255 << 24)
                            | ((byte)(r / hits) << 16)
                            | ((byte)(g / hits) << 8)
                            | ((byte)(b / hits));

                        newColors[y] = color;
                    }

                    index += w;
                }

                for (int y = 0; y < h; y++)
                {
                    pixels[y * w + x] = newColors[y];
                }
            }
        }
    }

}
