using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Aesir5
{
    public class ImageRenderer : IDisposable
    {
        bool isDisposed;
        public static int SizeModifier = 36;
        private static readonly ConcurrentDictionary<int, Bitmap> CachedTiles = new ConcurrentDictionary<int, Bitmap>();
        private static readonly ConcurrentDictionary<int, Bitmap> CachedObjects = new ConcurrentDictionary<int, Bitmap>();
        Bitmap bitmap;
        Bitmap resizeBitmap;

        #region IDisposable Methods
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool isDisposing)
        {
            if (isDisposed) return;

            if (isDisposing)
            {
                ClearTileCache();
                ClearObjectCache();
            }

            isDisposed = true;
        }

        ~ImageRenderer()
        {
            Dispose(false);   
        }
        #endregion

        public Bitmap GetTileBitmap(int tile)
        {
            if (CachedTiles.ContainsKey(tile))
                return CachedTiles[tile];

            bitmap = new Bitmap(48, 48, PixelFormat.Format8bppIndexed);
            ColorPalette palette = bitmap.Palette;
            palette.Entries[0] = Color.Transparent;

            for (int i = 1; i < 256; i++)
            {
                Color color = TileManager.TilePal[TileManager.TileTBL[tile]][i];
                palette.Entries[i] = color;
            }

            bitmap.Palette = palette;
            BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, 48, 48), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            long top = TileManager.Epf[0].frames[tile].Top;
            long left = TileManager.Epf[0].frames[tile].Left;
            long bottom = TileManager.Epf[0].frames[tile].Bottom;
            long right = TileManager.Epf[0].frames[tile].Right;
            if (left < 0)
                MessageBox.Show(@"left < 0");

            if (top < 0)
                MessageBox.Show(@"top < 0");

            byte[] pixelData = new byte[bitmapdata.Stride * bitmap.Height];
            Marshal.Copy(bitmapdata.Scan0, pixelData, 0, pixelData.Length);

            for (int i = (int)top; i < (int)bottom; i++)
            {
                int index = i * bitmapdata.Stride;
                //byte* numPtr = (byte*)bitmapdata.Scan0 + (i * bitmapdata.Stride);
                for (int j = (int)left; j < (int)right; j++)
                {
                    int num3 = TileManager.Epf[0].frames[tile][(int)(i - top), (int)(j - left)];
                    //if (num3 == 0)
                    //    continue;
                    //Color color = TileManager.TilePal[TileManager.TileTBL[tile]][num3];
                    long a = j;

                    pixelData[index + a] = (byte)num3;
                    //pixelData[index + (a * 4) + 1] = color.G;
                    //pixelData[index + (a * 4) + 2] = color.R;
                    //pixelData[index + (a * 4) + 3] = color.A;
                    //numPtr[(a * 4)] = color.B;
                    //numPtr[(a * 4) + 1] = color.G;
                    //numPtr[(a * 4) + 2] = color.R;
                    //numPtr[(a * 4) + 3] = color.A;
                }
            }

            Marshal.Copy(pixelData, 0, bitmapdata.Scan0, pixelData.Length);
            bitmap.UnlockBits(bitmapdata);
            resizeBitmap = new Bitmap(bitmap, SizeModifier, SizeModifier);
            //bitmap.RotateFlip(RotateFlipType.Rotate90FlipX);

            CachedTiles[tile] = resizeBitmap;
            return resizeBitmap;
        }

        public Bitmap GetObjectBitmap(int tile)
        {
            if (CachedObjects.ContainsKey(tile))
                return CachedObjects[tile];

            bitmap = new Bitmap(48, 48, PixelFormat.Format8bppIndexed);
            ColorPalette palette = bitmap.Palette;
            palette.Entries[0] = Color.Transparent;

            for (int i = 1; i < 256; i++)
            {
                Color color = TileManager.TileCPal[TileManager.TileCTBL[tile]][i];
                palette.Entries[i] = color;
            }

            bitmap.Palette = palette;
            BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, 48, 48), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            long top = TileManager.Epf[1].frames[tile].Top;
            long left = TileManager.Epf[1].frames[tile].Left;
            long bottom = TileManager.Epf[1].frames[tile].Bottom;
            long right = TileManager.Epf[1].frames[tile].Right;
            if (left < 0)
                MessageBox.Show(@"left < 0");

            if (top < 0)
                MessageBox.Show(@"top < 0");

            byte[] pixelData = new byte[bitmapdata.Stride * bitmap.Height];
            Marshal.Copy(bitmapdata.Scan0, pixelData, 0, pixelData.Length);

            for (int i = (int)top; i < (int)bottom; i++)
            {
                int index = i * bitmapdata.Stride;
                //byte* numPtr = (byte*)bitmapdata.Scan0 + (i * bitmapdata.Stride);
                for (int j = (int)left; j < (int)right; j++)
                {
                    int num3 = TileManager.Epf[1].frames[tile][(int)(i - top), (int)(j - left)];
                    //if (num3 == 0)
                    //    continue;
                    //Color color = TileManager.TileCPal[TileManager.TileCTBL[tile]][num3];
                    long a = j;

                    pixelData[index + a] = (byte)num3;
                    //pixelData[index + (a * 4) + 1] = color.G;
                    //pixelData[index + (a * 4) + 2] = color.R;
                    //pixelData[index + (a * 4) + 3] = color.A;
                    //numPtr[(a * 4)] = color.B;
                    //numPtr[(a * 4) + 1] = color.G;
                    //numPtr[(a * 4) + 2] = color.R;
                    //numPtr[(a * 4) + 3] = color.A;

                }
            }

            Marshal.Copy(pixelData, 0, bitmapdata.Scan0, pixelData.Length);
            bitmap.UnlockBits(bitmapdata);
            resizeBitmap = new Bitmap(bitmap, SizeModifier, SizeModifier);
            //bitmap.RotateFlip(RotateFlipType.Rotate90FlipX);

            CachedObjects[tile] = resizeBitmap;
            return resizeBitmap;
        }

        public static void ClearTileCache()
        {
            foreach (Bitmap bitmap in CachedTiles.Values)
                bitmap.Dispose();

            CachedTiles.Clear();
        }

        public static void ClearObjectCache()
        {
            foreach (Bitmap bitmap in CachedObjects.Values)
                bitmap.Dispose();

            CachedObjects.Clear();
        }
    }
}
