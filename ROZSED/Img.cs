using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Linq;
using System.Drawing.Drawing2D;
using System.IO;

namespace ROZSED.Std
{
    /// <summary>
    /// Perform actions on Bitmap, int[,] and double[,] arrays
    /// </summary>
    public static class Img
    {
        #region Kernels
        public static readonly int[,] SobelX = {{ 1, 0,-1},
                                                { 2, 0,-2},
                                                { 1, 0,-1}};
        public static readonly int[,] SobelY = {{ 1, 2, 1},
                                                { 0, 0, 0},
                                                {-1,-2,-1}};
        public static readonly double[,] Gauss5px16 = 
        {{0.0121461242019898,0.0261099442007322,0.0336973192407131,0.0261099442007322,0.0121461242019898},
         {0.0261099442007322,0.0561273024075996,0.0724375208467849,0.0561273024075996,0.0261099442007322},
         {0.0336973192407131,0.0724375208467849,0.0934873796057929,0.0724375208467849,0.0336973192407131},
         {0.0261099442007322,0.0561273024075996,0.0724375208467849,0.0561273024075996,0.0261099442007322},
         {0.0121461242019898,0.0261099442007322,0.0336973192407131,0.0261099442007322,0.0121461242019898}};
        #endregion
        public static double[,] GetGaussKernel(int kernelSize, double sigma)
        {
            if (kernelSize == 5 && sigma == 1.4) // Common used kernel
                return Gauss5px16;

            var gaussKernel = new double[kernelSize, kernelSize];
            int i, j,
                n = (kernelSize - 1) / 2;
            double D1, D2,
                pi = Math.PI,
                weight = 0;

            D2 = 1 / (2 * sigma * sigma);
            D1 = D2 / pi;

            for (i = -n; i <= n; i++)
            {
                for (j = -n; j <= n; j++)
                {
                    weight += gaussKernel[n + i, n + j] = D1 * Math.Exp(-(i * i + j * j) * D2);
                }
            }
            // Normalize
            for (i = -n; i <= n; i++)
            {
                for (j = -n; j <= n; j++)
                {
                    gaussKernel[n + i, n + j] /= weight;
                }
            }
            return gaussKernel;
        }

        // Basic manipulation =================================================================
        public static void Resize(string srcPath, string dstPath, int dstHeight)
        {
            var src = new Bitmap(srcPath);
            var dstWidth = src.Width * dstHeight / src.Height;
            var dstBitmap = new Bitmap(dstWidth, dstHeight);
            Graphics dstGraphics = Graphics.FromImage((Image)dstBitmap);
            dstGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            dstGraphics.DrawImage(src, 0, 0, dstWidth, dstHeight);
            dstGraphics.Dispose();
            dstBitmap.Save(dstPath, dstPath.ImgFormat());
        }
        public static ImageFormat ImgFormat(this string imgPath)
        {
            switch (imgPath.Substring(imgPath.LastIndexOf('.') + 1).ToLower())
            {
                case "bmp":
                    return ImageFormat.Bmp;
                case "emp":
                    return ImageFormat.Emf;
                case "gif":
                    return ImageFormat.Gif;
                case "jpg":
                case "jpeg":
                    return ImageFormat.Jpeg;
                case "png":
                    return ImageFormat.Png;
                case "tif":
                case "tiff":
                    return ImageFormat.Tiff;
                case "wmf":
                    return ImageFormat.Wmf;
                default:
                    throw new Exception("Can't recognize image file extension.");
            }
        }

        // Extensions methods =================================================================
        public static Bitmap ToGreyBmp(this int[,] GreyArray)
        {
            int w, h,
                W = GreyArray.GetLength(0),
                H = GreyArray.GetLength(1);
            Bitmap dst = new Bitmap(W, H, PixelFormat.Format8bppIndexed);

            // Set pallete to grayscale
            ColorPalette pal = dst.Palette;
            for (int i = 0; i < pal.Entries.Length; i++)
                pal.Entries[i] = Color.FromArgb(i, i, i);
            dst.Palette = pal;

            BitmapData dstData = dst.LockBits(new Rectangle(0, 0, W, H),
                                     ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            int imageSize = dstData.Stride * dstData.Height;
            var dstBuffer = new byte[imageSize];
            int destinationIndex;

            for (h = 0; h < H; h++)
            {
                destinationIndex = h * dstData.Stride;
                for (w = 0; w < W; w++)
                {
                    dstBuffer[destinationIndex++] = (byte)GreyArray[w, h];
                }
            }

            Marshal.Copy(dstBuffer, 0, dstData.Scan0, imageSize);
            dst.UnlockBits(dstData);

            return dst;
        }
        public static Bitmap ToGreyBmp(this double[,] GreyArray)
        {
            int w, h,
                W = GreyArray.GetLength(0),
                H = GreyArray.GetLength(1);
            Bitmap bmp = new Bitmap(W, H, PixelFormat.Format8bppIndexed);

            // Set pallete to grayscale
            ColorPalette pal = bmp.Palette;
            for (int i = 0; i < pal.Entries.Length; i++)
                pal.Entries[i] = Color.FromArgb(i, i, i);
            bmp.Palette = pal;

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, W, H),
                                     ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            int imageSize = bmpData.Stride * bmpData.Height;
            var dstBuffer = new byte[imageSize];
            int dstIndex;

            for (h = 0; h < H; h++)
            {
                dstIndex = h * bmpData.Stride;
                for (w = 0; w < W; w++)
                {
                    dstBuffer[dstIndex++] = (byte)GreyArray[w, h];
                }
            }

            Marshal.Copy(dstBuffer, 0, bmpData.Scan0, imageSize);
            bmp.UnlockBits(bmpData);

            return bmp;
        }
        /// <summary>
        /// <para>greyArray[Width,Height]</para>
        /// <para>if(value &gt; threshold) value = whiteColor;</para>
        /// </summary>
        public static Bitmap ToBWBmp(this int[,] greyArray, int threshold)
        {
            int w, h,
                W = greyArray.GetLength(0),
                H = greyArray.GetLength(1);
            Bitmap dst = new Bitmap(W, H, PixelFormat.Format1bppIndexed);
            BitmapData dstData = dst.LockBits(new Rectangle(0, 0, W, H),
                                     ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);

            int imageSize = dstData.Stride * dstData.Height;
            var dstBuffer = new byte[imageSize];
            int dstIndex = 0;
            byte dstValue = 0;
            int pixelValue = 128;

            for (h = 0; h < H; h++)
            {
                dstIndex = h * dstData.Stride;
                dstValue = 0;
                pixelValue = 128;
                for (w = 0; w < W; w++)
                {
                    if (greyArray[w, h] > threshold)
                    {
                        dstValue += (byte)pixelValue;
                    }
                    if (pixelValue == 1)
                    {
                        dstBuffer[dstIndex] = dstValue;
                        dstIndex++;
                        dstValue = 0;
                        pixelValue = 128;
                    }
                    else
                    {
                        pixelValue >>= 1;
                    }
                }
                if (pixelValue != 128)
                {
                    dstBuffer[dstIndex] = dstValue;
                }
            }

            Marshal.Copy(dstBuffer, 0, dstData.Scan0, imageSize);
            dst.UnlockBits(dstData);

            return dst;
        }
        /// <summary>
        /// <para>greyArray[Width,Height]</para>
        /// <para>greyValue = (30*Red + 59*Green + 11*Blue) /100;</para>
        /// </summary>
        public static int[,] ToGreyArray(this Bitmap bmp)
        {
            int w, h,
                W = bmp.Width,
                H = bmp.Height;
            int[,] greyArray = new int[W, H];

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, W, H),
                                     ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int imageSize = bmpData.Stride * bmpData.Height;
            var dstBuffer = new byte[imageSize];
            Marshal.Copy(bmpData.Scan0, dstBuffer, 0, imageSize);
            bmp.UnlockBits(bmpData);

            int dstIndex = 0;
            for (h = 0; h < H; h++)
            {
                for (w = 0; w < W; w++)
                {
                    greyArray[w, h] = ((21 * dstBuffer[dstIndex++] + 72 * dstBuffer[dstIndex++] + 7 * dstBuffer[dstIndex++]) / 100);
                    dstIndex++;
                }
            }
            return greyArray;
        }
        public static int[,] Convolution(this int[,] raster, int[,] kernel)
        {
            int w, h, wK, hK,
                n = (kernel.GetLength(0) - 1) / 2,
                WLim = raster.GetLength(0) - n,
                HLim = raster.GetLength(1) - n;

            double sum = 0;
            var filtered = (int[,])raster.Clone();

            for (h = n; h < HLim; h++)
            {
                for (w = n; w < WLim; w++)
                {
                    sum = 0;

                    for (wK = -n; wK <= n; wK++)
                        for (hK = -n; hK <= n; hK++)
                            sum += raster[w + wK, h + hK] * kernel[n + wK, n + hK];

                    filtered[w, h] = (int)Math.Round(sum);
                }
            }
            return filtered;
        }
        public static int[,] Convolution(this int[,] raster, double[,] kernel)
        {
            int w, h, wK, hK,
                n = (kernel.GetLength(0) - 1) / 2,
                WLim = raster.GetLength(0) - n,
                HLim = raster.GetLength(1) - n;

            double sum = 0;
            var filtered = (int[,])raster.Clone();

            for (h = n; h < HLim; h++)
            {
                for (w = n; w < WLim; w++)
                {
                    sum = 0;

                    for (wK = -n; wK <= n; wK++)
                        for (hK = -n; hK <= n; hK++)
                            sum += raster[w + wK, h + hK] * kernel[n + wK, n + hK];

                    filtered[w, h] = (int)Math.Round(sum);
                }
            }
            return filtered;
        }
        public static int[,] NonMaxSup(this double[,] gradient, int[,] diffX, int[,] diffY)
        {
            double tangent, gradVal;
            int w, h,
                W = gradient.GetLength(0),
                H = gradient.GetLength(1),
                WLim = W - 2,
                HLim = H - 2;
            var localMax = new int[W, H];

            for (h = 2; h < HLim; h++)
            {
                for (w = 2; w < WLim; w++)
                {
                    gradVal = gradient[w, h];

                    if (diffX[w, h] == 0)
                        tangent = 3; // 3 > 2.4142
                    else
                        tangent = diffY[w, h] / diffX[w, h];

                    // Horizontal Edge
                    if (-0.4142 <= tangent && tangent < 0.4142)
                    {
                        if ((gradVal < gradient[w, h + 1]) || (gradVal < gradient[w, h - 1]))
                            localMax[w, h] = 0;
                        else
                            localMax[w, h] = (int)gradVal;
                    }

                    // Vertical Edge
                    else if (tangent < -2.4142 || 2.4142 <= tangent)
                    {
                        if ((gradVal < gradient[w + 1, h]) || (gradVal < gradient[w - 1, h]))
                            localMax[w, h] = 0;
                        else
                            localMax[w, h] = (int)gradVal;
                    }

                    // +45 Degree Edge
                    else if (-2.4142 <= tangent && tangent < -0.4142)
                    {
                        if ((gradVal < gradient[w + 1, h - 1]) || (gradVal < gradient[w - 1, h + 1]))
                            localMax[w, h] = 0;
                        else
                            localMax[w, h] = (int)gradVal;
                    }

                    // -45 Degree Edge
                    else if (2.4142 > tangent && tangent >= 0.4142)
                    {
                        if ((gradVal < gradient[w + 1, h + 1]) || (gradVal < gradient[w - 1, h - 1]))
                            localMax[w, h] = 0;
                        else
                            localMax[w, h] = (int)gradVal;
                    }
                }
            }
            return localMax;
        }
        public static void Thin(this int[,] raster)
        {
            int x, y,
                W = raster.GetLength(0) - 1,
                H = raster.GetLength(1) - 1;

            for (y = 1; y < H; y++)
                for (x = 1; x < W; x++)
                    if (raster.IsExcess(x, y, 255))
                        raster[x, y] = 0;
        }
        public static bool IsNeighbour(this int[,] raster, int X, int Y, int value)
        {
            if (raster[X - 1, Y - 1] == value ||
               raster[X, Y - 1] == value ||
               raster[X + 1, Y - 1] == value ||
               raster[X - 1, Y] == value ||
               raster[X + 1, Y] == value ||
               raster[X - 1, Y + 1] == value ||
               raster[X, Y + 1] == value ||
               raster[X + 1, Y + 1] == value)
                return true;
            else
                return false;
        }
        public static bool IsExcess(this int[,] raster, int X, int Y, int value)
        {
            if (raster[X, Y] == value &&
                raster[X - 1, Y - 1] == value &&
                raster[X, Y - 1] == value &&
                raster[X - 1, Y] == value)
                return true;
            else
                return false;
        }
        public static Bitmap CannyEdge(this Bitmap source, double th, double tl, int gaussKernelSize, double gaussKernelSigma)
        {
            var canny = new Canny(source, th, tl, gaussKernelSize, gaussKernelSigma);
            return canny.Edges.ToGreyBmp();
        }
        public static int[,] CannyEdge(this int[,] source, double th, double tl, int gaussKernelSize, double gaussKernelSigma)
        {
            var canny = new Canny(source, th, tl, gaussKernelSize, gaussKernelSigma);
            return canny.Edges;
        }
    }

    class Canny
    {
        public double HightThresh, LowThresh, KernelSigma;
        public int W, H, KernelSize; // H, W: Width and Height of input and output images

        // Images
        public int[,] GreyImage, FilteredImage, DiffX, DiffY, LocalMax, Edges;
        public double[,] Gradient;

        public Canny(int[,] input, double th, double tl, int gaussKernelSize, double gaussKernelSigma)
        {
            HightThresh = th;
            LowThresh = tl;
            KernelSize = gaussKernelSize;
            KernelSigma = gaussKernelSigma;
            W = input.GetLength(0);
            H = input.GetLength(1);

            GreyImage = input;
            DetectCannyEdges();
        }
        public Canny(Bitmap input, double th, double tl, int gaussKernelSize, double gaussKernelSigma)
        {
            HightThresh = th;
            LowThresh = tl;
            KernelSize = gaussKernelSize;
            KernelSigma = gaussKernelSigma;
            W = input.Width;
            H = input.Height;

            GreyImage = input.ToGreyArray();
            DetectCannyEdges();
        }
        public Canny(Bitmap input, double th, double tl) : this(input, th, tl, 5, 1.4) { }
        public Canny(Bitmap input) : this(input, 40, 10) { }

        private void DetectCannyEdges()
        {
            Gradient = new double[W, H];

            // Gaussian Filter Input Image 
            FilteredImage = GreyImage.Convolution(Img.GetGaussKernel(KernelSize, KernelSigma));

            // Compute difference and gradient magnitute
            DiffX = FilteredImage.Convolution(Img.SobelX);
            DiffY = FilteredImage.Convolution(Img.SobelY);
            Gradient.ForEach(DiffX, DiffY, (x, y) => Math.Sqrt((x * x) + (y * y)));

            // Perform Non maximum suppression:
            LocalMax = Gradient.NonMaxSup(DiffX, DiffY);

            // Select local maximum by thresholds
            Thresholding();

            // Remove pixel, which make loops in converted Polyline
            Edges.Thin();
        }
        private void Thresholding()
        {
            Edges = new int[W, H];
            int i, j,
                limit = (KernelSize - 1) / 2,
                WLim = W - limit,
                HLim = H - limit;

            for (i = limit; i < WLim; i++)
                for (j = limit; j < HLim; j++)
                    if (LocalMax[i, j] >= LowThresh)
                        if (LocalMax[i, j] >= HightThresh)
                            Edges[i, j] = 255; // edge
                        else
                            Edges[i, j] = 128; // posible edge

            for (i = limit; i < WLim; i++)
                for (j = limit; j < HLim; j++)
                    if (Edges[i, j] == 128)
                        if (Edges.IsNeighbour(i, j, 255))
                            Edges[i, j] = 255;
                        else
                            Edges[i, j] = 0;
        }
    }
}
