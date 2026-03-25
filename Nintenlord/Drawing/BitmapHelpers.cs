using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;

namespace Nintenlord.Drawing
{
    [SupportedOSPlatform("windows")]
    public static class BitmapHelpers
    {
        private static readonly IDictionary<string, ImageFormat> formats;

        static BitmapHelpers()
        {
            formats = new Dictionary<string, ImageFormat>(StringComparer.OrdinalIgnoreCase);
            formats[".bmp"] = ImageFormat.Bmp;
            formats[".png"] = ImageFormat.Png;
            formats[".gif"] = ImageFormat.Gif;
            //Add more at some point.
        }

        public static unsafe Rectangle FindMatch2D(byte* bigImage, Size bigSize, Rectangle findIn,
            byte* smallImage, Size smallSize, Rectangle findWhat)
        {
            var result = Rectangle.Empty;
            if (findIn.IsEmpty)
            {
                findIn = new Rectangle(Point.Empty, bigSize);
            }
            else if (!FitsIn(ref bigSize, ref findIn))
            {
                throw new ArgumentException("Rectangle to find in does not fit to the big image");
            }

            if (findWhat.IsEmpty)
            {
                findWhat = new Rectangle(Point.Empty, smallSize);
            }
            else if (!FitsIn(ref smallSize, ref findWhat))
            {
                throw new ArgumentException("Rectangle to find does not fit to the small image");
            }

            //add test for small rect not fitting to big rect here

            for (var y = findIn.Y; y < findIn.Bottom - findWhat.Height + 1; y++)
            {
                var rowPointer = bigImage + y * bigSize.Width + findIn.X;
                for (var x = findIn.X; x < findIn.Right - findWhat.Width + 1; x++)
                {
                    //inner loops, could use some optimation
                    for (var y2 = findWhat.Y; y2 < findWhat.Bottom; y2++)
                    {
                        for (var x2 = findWhat.X; x2 < findWhat.Right; x2++)
                        {
                            //do the comparing
                            if (rowPointer[y2 * bigSize.Width + x2]
                                != smallImage[y2 * smallSize.Width + x2])
                            {
                                goto noMatch;
                            }
                        }
                    }
                    //only way here is to pass the test
                    result.X = x;
                    result.Y = y;
                    result.Width = findWhat.Width;
                    result.Height = findWhat.Height;
                    goto matchFound;
noMatch:
                    rowPointer++;
                }
            }
matchFound:
            return result;
        }

        public static unsafe Rectangle FindMatch2D(Bitmap bigImage, Rectangle findIn,
            Bitmap smallImage, Rectangle findWhat)
        {
            if (bigImage.PixelFormat != smallImage.PixelFormat)
            {
                throw new ArgumentException("Bitmaps need to be in same format.");
            }

            if (findIn.IsEmpty)
            {
                findIn = new Rectangle(Point.Empty, bigImage.Size);
            }
            if (findWhat.IsEmpty)
            {
                findWhat = new Rectangle(Point.Empty, smallImage.Size);
            }

            var result = Rectangle.Empty;

            var bmpdBig = bigImage.LockBits(findIn, ImageLockMode.ReadOnly, bigImage.PixelFormat);
            var bmpdSmall = smallImage.LockBits(findWhat, ImageLockMode.ReadOnly, smallImage.PixelFormat);

            var pointerBig = (byte*)bmpdBig.Scan0;
            var pointerSmall = (byte*)bmpdSmall.Scan0;

            var bpp = Image.GetPixelFormatSize(bigImage.PixelFormat);
            var bigSize = bigImage.Size;
            bigSize.Width = bigSize.Width * bpp / 8;
            var smallSize = smallImage.Size;
            smallSize.Width = smallSize.Width * bpp / 8;

            findIn.X = findIn.X * bpp / 8;
            findIn.Width = findIn.Width * bpp / 8;
            findWhat.X = findWhat.X * bpp / 8;
            findWhat.Width = findWhat.Width * bpp / 8;

            result = FindMatch2D(pointerBig, bigSize, findIn, pointerSmall, smallSize, findWhat);

            result.X = result.X * 8 / bpp;
            result.Width = result.Width * 8 / bpp;

            return result;
        }

        [Obsolete("Use Image.GetPixelFormatSize instead.", true)]
        public static int BitsPerPixel(PixelFormat pixelFormat) => Image.GetPixelFormatSize(pixelFormat);

        public static void Transpose(ref Rectangle rect)
        {
            var temp = rect.Y;
            rect.Y = rect.X;
            rect.X = temp;

            temp = rect.Width;
            rect.Height = rect.Width;
            rect.Width = temp;
        }

        public static void Transpose(ref Point point)
        {
            var temp = point.Y;
            point.Y = point.X;
            point.X = temp;
        }

        public static void Transpose(ref RectangleF rect)
        {
            var temp = rect.Y;
            rect.Y = rect.X;
            rect.X = temp;

            temp = rect.Width;
            rect.Height = rect.Width;
            rect.Width = temp;
        }

        public static void Transpose(ref PointF point)
        {
            var temp = point.Y;
            point.Y = point.X;
            point.X = temp;
        }

        public static bool FitsIn(ref Size size, ref Rectangle rect) =>
            size.Height >= rect.Bottom &&
            size.Width >= rect.Right &&
            rect.X >= 0 &&
            rect.Y >= 0;

        public static bool IsBigger(ref Size big, ref Size small) => big.Width >= small.Width && big.Height >= small.Height;

        public static bool IsSmaller(ref Size big, ref Size small) => big.Width <= small.Width && big.Height <= small.Height;

        public static HashSet<Color> GetColors(this Bitmap bitmap)
        {
            var palette = new HashSet<Color>();

            if ((bitmap.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
            {
                foreach (var item in bitmap.Palette.Entries)
                {
                    palette.Add(item);
                }
            }
            else
            {
                for (var i = 0; i < bitmap.Width; i++)
                {
                    for (var j = 0; j < bitmap.Height; j++)
                    {
                        var color = bitmap.GetPixel(i, j);
                        if (!palette.Contains(color))
                        {
                            palette.Add(color);
                        }
                    }
                }
            }
            return palette;
        }

        public static ImageFormat GetFormat(string file) =>
            formats.SingleOrDefault(
                x => x.Key.Equals(Path.GetExtension(file))
            ).Value;

        public static ColorPalette InsertColors(this ColorPalette original, Color[] palette)
        {
            if (palette == null)
                throw new ArgumentNullException();

            for (var i = 0; i < palette.Length && i < original.Entries.Length; i++)
            {
                original.Entries[i] = palette[i];
            }
            for (var i = palette.Length; i < original.Entries.Length; i++)
            {
                original.Entries[i] = Color.FromArgb(0, 0, 0);
            }
            return original;
        }

        public static void InsertColorsToPalette(this Bitmap bitmap, Color[] palette) => bitmap.Palette = bitmap.Palette.InsertColors(palette);

        [Obsolete("Removed a file this method uses :D")]
        public static unsafe Bitmap Quantazase(Bitmap bitmap)
        {
            if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
                return bitmap;

            var result = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format8bppIndexed);
            Bitmap trueColorBitmap;

            if (bitmap is Bitmap && bitmap.PixelFormat == PixelFormat.Format32bppArgb)
            {
                trueColorBitmap = bitmap;
            }
            else
            {
                trueColorBitmap = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);

                using (var g = Graphics.FromImage(trueColorBitmap))
                {
                    g.PageUnit = GraphicsUnit.Pixel;
                    g.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
                }
            }

            //Octree<List<Color>> colors = new Octree<List<Color>>(5, 5);
            //BitmapData bmpData = trueColorBitmap.LockBits(new Rectangle(new Point(), trueColorBitmap.Size), ImageLockMode.ReadOnly, trueColorBitmap.PixelFormat);
            //int* pointer = (int*)bmpData.Scan0.ToPointer();
            //for (int y = 0; y < bitmap.Height; y++)
            //{
            //    for (int x = 0; x < trueColorBitmap.Width; x++)
            //    {
            //        Color color = Color.FromArgb(pointer[x]);
            //        int[] position = new int[5];
            //        for (int i = 0; i < position.Length; i++)
            //        {
            //            position[i] = ((color.R >> (8 - i)) & 1);
            //            position[i] += ((color.G >> (8 - i)) & 1) * 2;
            //            position[i] += ((color.B >> (8 - i)) & 1) * 4;
            //        }
            //        colors.GetItem(position).Add(color);
            //    }
            //    pointer += bmpData.Stride;
            //}



            if (trueColorBitmap != bitmap)
                trueColorBitmap.Dispose();

            return result;
        }

        public static unsafe Bitmap GetArea(this Bitmap bitmap, Rectangle rect)
        {
            if (rect.X < 0 || rect.Y < 0 ||
                rect.Right > bitmap.Width || rect.Bottom > bitmap.Height)
            {
                throw new ArgumentException("Out of bitmap's area", "rect");
            }
            var subBitmap = new Bitmap(rect.Width, rect.Height, bitmap.PixelFormat);
            bitmap.Copy(subBitmap, rect, Point.Empty);

            return subBitmap;
        }

        public static unsafe void Copy(this Bitmap source, Bitmap dest, Rectangle sourceRect, Point destPos)
        {
            if (source.PixelFormat != dest.PixelFormat)
            {
                throw new ArgumentException("Source and destination pixel formats are different.");
            }
            if (sourceRect.X < 0 || sourceRect.Y < 0 || sourceRect.Right > source.Width || sourceRect.Bottom > source.Height)
            {
                throw new ArgumentException("Out of bitmaps area", "sourceRect");
            }
            if (destPos.X < 0 || destPos.Y < 0 ||
                destPos.X + sourceRect.Width > dest.Width ||
                destPos.Y + sourceRect.Height > dest.Height)
            {
                throw new ArgumentException("Out of bitmaps area", "destPos");
            }

            using (var sourceLock = new BitmapLocker(source, ImageLockMode.ReadOnly))
            {
                using (var destLock = new BitmapLocker(dest, ImageLockMode.WriteOnly))
                {
                    var copyLength = sourceLock.PixelSize * sourceRect.Width;

                    for (var i = 0; i < sourceRect.Height; i++)
                    {
                        MemCopy(
                            sourceLock[sourceRect.X, sourceRect.Y + i],
                            destLock[destPos.X, destPos.Y + i],
                            copyLength);
                    }
                }
            }
        }


        private static unsafe void MemCopy(IntPtr source, IntPtr destination, int length)
        {
            var intSource = (int*)source.ToPointer();
            var intDest = (int*)destination.ToPointer();
            while (length >= 4)
            {
                intDest[0] = intSource[0];
                intSource++;
                intDest++;
                length -= 4;
            }
            var byteSource = (byte*)intSource;
            var byteDest = (byte*)intDest;
            while (length > 0)
            {
                byteDest[0] = byteSource[0];
                byteDest++;
                byteSource++;
                length--;
            }
        }



        public static IEnumerable<Color> GetPixelEnumerator(this Bitmap bitmap) => bitmap.GetPixelEnumerator(new Rectangle(Point.Empty, bitmap.Size));

        public static IEnumerable<Color> GetPixelEnumerator(this Bitmap bitmap, Rectangle area)
        {
            if (area.X < 0 || area.Y < 0 ||
                area.Right > bitmap.Width || area.Bottom > bitmap.Height)
            {
                throw new ArgumentException("Out of bitmap's area", "area");
            }

            for (var y = area.Top; y < area.Bottom; y++)
            {
                for (var x = area.Left; x < area.Right; x++)
                {
                    yield return bitmap.GetPixel(x, y);
                }
            }
        }

        public static IEnumerable<Color> GetPixelEnumerator(this Bitmap bitmap, Rectangle area, Size tileSize)
        {
            if (area.X < 0 || area.Y < 0 ||
                area.Right * tileSize.Width > bitmap.Width ||
                area.Bottom * tileSize.Height > bitmap.Height)
            {
                throw new ArgumentException("Out of bitmap's area", "area");
            }

            for (var rectY = area.Top; rectY < area.Bottom; rectY++)
            {
                for (var rectX = area.Left; rectX < area.Right; rectX++)
                {
                    var sx = rectX * tileSize.Width;
                    var sy = rectY * tileSize.Height;
                    for (var y = 0; y < tileSize.Height; y++)
                    {
                        for (var x = 0; x < tileSize.Width; x++)
                        {
                            yield return bitmap.GetPixel(sx + x, sy + y);
                        }
                    }
                }
            }
        }


        public static IEnumerable<Tuple<Point, Color>> GetPixelAndPosEnumerator(this Bitmap bitmap) => bitmap.GetPixelAndPosEnumerator(new Rectangle(Point.Empty, bitmap.Size));

        public static IEnumerable<Tuple<Point, Color>> GetPixelAndPosEnumerator(this Bitmap bitmap, Rectangle area)
        {
            if (area.X < 0 || area.Y < 0 ||
                area.Right > bitmap.Width || area.Bottom > bitmap.Height)
            {
                throw new ArgumentException("Out of bitmap's area", "area");
            }

            for (var y = area.Top; y < area.Bottom; y++)
            {
                for (var x = area.Left; x < area.Right; x++)
                {
                    yield return Tuple.Create(new Point(x, y), bitmap.GetPixel(x, y));
                }
            }
        }

        public static IEnumerable<Tuple<Point, Color>> GetPixelAndPosEnumerator(this Bitmap bitmap, Rectangle area, Size rectSize)
        {
            if (area.X < 0 || area.Y < 0 ||
                area.Right * rectSize.Width > bitmap.Width ||
                area.Bottom * rectSize.Height > bitmap.Height)
            {
                throw new ArgumentException("Out of bitmap's area", "area");
            }

            for (var rectY = area.Top; rectY < area.Bottom; rectY++)
            {
                for (var rectX = area.Left; rectX < area.Right; rectX++)
                {
                    var sx = rectX * rectSize.Width;
                    var sy = rectY * rectSize.Height;
                    for (var y = 0; y < rectSize.Height; y++)
                    {
                        for (var x = 0; x < rectSize.Width; x++)
                        {
                            yield return Tuple.Create(new Point(x, y), bitmap.GetPixel(x, y));
                        }
                    }
                }
            }
        }


        public static void MovePixels(Bitmap source, Bitmap destination, Func<Point, Point> move, Rectangle rect)
        {
            foreach (var item in source.GetPixelAndPosEnumerator(rect))
            {
                var point = move(item.Item1);
                destination.SetPixel(point.X, point.Y, item.Item2);
            }
        }
    }

    [SupportedOSPlatform("windows")]
    public sealed class BitmapLocker : IDisposable, IEnumerable<IntPtr>
    {
        public BitmapData BitmapData
        {
            get;
            private set;
        }
        public int PixelSize => Image.GetPixelFormatSize(BitmapData.PixelFormat) / 8;

        public int PixelByteWidth => PixelSize * BitmapData.Width;

        public IntPtr this[int x, int y] =>
            BitmapData.Scan0
            + BitmapData.Stride * y
            + PixelSize * x;

        private readonly Bitmap bitmap;
        private readonly ImageLockMode lockMode;
        private readonly Rectangle areaToLock;
        private readonly PixelFormat format;

        public BitmapLocker(Bitmap bitmapToLock, ImageLockMode lockMode)
            : this(bitmapToLock, lockMode, new Rectangle(Point.Empty, bitmapToLock.Size))
        {

        }

        private BitmapLocker(Bitmap bitmapToLock, ImageLockMode lockMode, Rectangle rect)
        {
            bitmap = bitmapToLock;
            this.lockMode = lockMode;
            areaToLock = rect;
            format = bitmap.PixelFormat;
            Lock();
        }

        private void Lock() =>
            BitmapData =
                bitmap.LockBits(
                    areaToLock,
                    lockMode,
                    format);

        #region IDisposable Members

        public void Dispose() => bitmap.UnlockBits(BitmapData);

        #endregion

        #region IEnumerable<IntPtr> Members

        public IEnumerator<IntPtr> GetEnumerator()
        {
            var currentLine = BitmapData.Scan0;
            for (var i = 0; i < bitmap.Height; i++)
            {
                yield return currentLine;
                currentLine += BitmapData.Stride;
            }
        }

        public IEnumerator<IntPtr> GetEnumerator(int y, int height)
        {
            var currentLine = BitmapData.Scan0 + y * BitmapData.Stride;
            for (var i = 0; i < height; i++)
            {
                yield return currentLine;
                currentLine += BitmapData.Stride;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}
