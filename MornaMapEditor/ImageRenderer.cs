using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MornaMapEditor
{
    public class ImageRenderer : IDisposable
    {
        private object drawLock = new object();
        static readonly int CacheInitialCapacity = 40000;

        bool isDisposed;
        public int sizeModifier = 36;
        Dictionary<int, Bitmap> cachedTiles = new Dictionary<int, Bitmap>(CacheInitialCapacity);
        Dictionary<int, Bitmap> cachedObjects = new Dictionary<int, Bitmap>(CacheInitialCapacity);

        #region Singleton Member Variables
        // Disallow instance creation.
        private ImageRenderer()
        { }

        static readonly ImageRenderer singleton = new ImageRenderer();

        public static ImageRenderer Singleton
        {
            get { return singleton; }
        }
        #endregion

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
            if (cachedTiles.ContainsKey(tile))
            {
                lock (drawLock)
                {
                    return cachedTiles[tile].Clone() as Bitmap;   
                }
            }

            var bitmap = new Bitmap(48, 48, PixelFormat.Format8bppIndexed);
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
            var resizeBitmap = new Bitmap(bitmap, sizeModifier, sizeModifier);
            //bitmap.RotateFlip(RotateFlipType.Rotate90FlipX);

            cachedTiles[tile] = resizeBitmap;
            lock (drawLock)
            {
                return resizeBitmap.Clone() as Bitmap;
            }
        }

        public Bitmap GetObjectBitmap(int tile)
        {
            if (cachedObjects.ContainsKey(tile))
                return cachedObjects[tile];

            var bitmap = new Bitmap(48, 48, PixelFormat.Format8bppIndexed);
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
            var resizeBitmap = new Bitmap(bitmap, sizeModifier, sizeModifier);
            //bitmap.RotateFlip(RotateFlipType.Rotate90FlipX);

            cachedObjects[tile] = resizeBitmap;
            lock (drawLock)
            {
                return resizeBitmap.Clone() as Bitmap;
            }
        }
        
        public Bitmap RenderObjects(Tile[] tilesWithPossibleObjects)
        {
            Bitmap bitmap = new Bitmap(sizeModifier,sizeModifier);
            Graphics graphics = Graphics.FromImage(bitmap);
            for(int i = 0; i < 12; i++)
            {
                Tile tile = tilesWithPossibleObjects[i];
                if (tile != null && tile.ObjectNumber != 0)
                {
                    int objectNumber = tile.ObjectNumber;
                    if (objectNumber >= 0 && objectNumber < TileManager.ObjectInfos.Length)
                    {
                        int objectHeight = TileManager.ObjectInfos[objectNumber].Indices.Length;
                        if (objectHeight > i)
                        {
                            int objTileNumber = TileManager.ObjectInfos[objectNumber].Indices[objectHeight - i - 1];
                            lock (drawLock)
                            {
                                graphics.DrawImage(ImageRenderer.Singleton.GetObjectBitmap(objTileNumber), 0, 0);
                            }
                        }
                    }
                }
            } 
            
            graphics.Dispose(); 
            lock (drawLock) 
            {
                return bitmap.Clone() as Bitmap;
            }
        }

        public void ClearTileCache()
        {
            foreach (Bitmap bitmap in cachedTiles.Values)
                bitmap.Dispose();

            cachedTiles.Clear();
        }

        public void ClearObjectCache()
        {
            foreach (Bitmap bitmap in cachedObjects.Values)
                bitmap.Dispose();

            cachedObjects.Clear();
        }

        public Bitmap GetFilledObjectBitmap(Bitmap unfilledObjectBitmap, int xIndex, int yIndex)
        {
            var renderedBitmap = new Bitmap(sizeModifier, sizeModifier);
            var graphics = Graphics.FromImage(renderedBitmap);
            //If only showing objects, make sure we have a background of dark green first
            lock (drawLock)
            {
                graphics.FillRectangle(Brushes.DarkGreen, xIndex * sizeModifier, yIndex * sizeModifier, sizeModifier, sizeModifier);
                graphics.DrawImage(unfilledObjectBitmap, xIndex * sizeModifier, yIndex * sizeModifier); //, 36, 36)
                graphics.Dispose();
                return renderedBitmap.Clone() as Bitmap;
            }
        }

        public Bitmap GetCombinedBitmap(Bitmap tileBitmap, Bitmap objectBitmap)
        {
            var renderedBitmap = (Bitmap)tileBitmap.Clone();
            var tileGraphics = Graphics.FromImage(renderedBitmap);

            lock (drawLock)
            {
                tileGraphics.DrawImage(objectBitmap, 0, 0);
                tileGraphics.Dispose();
                return renderedBitmap.Clone() as Bitmap;
            }
        }

        public void WriteImageToFile(Bitmap image, string filename)
        {
            lock (drawLock)
            {
                image.Save(filename, ImageFormat.Png);
            }
        }
    }
}
