using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace Aesir5
{
    public class Map
    {
        public string Name { get; set; }
        public Size Size { get; set; }
        public bool IsModified { get; set; }
        public bool IsEditable { get; set; }
        public Dictionary<Point, Tile> MapData { get; private set; }

        public class Tile
        {
            public int TileNumber { get; set; }
            public bool Passability { get; set; }
            public int ObjectNumber { get; set; }
            
            public Bitmap RenderedTile { get; set; }

            private async Task<Bitmap> RenderTile()
            {
                if (TileNumber >= TileManager.Epf[0].max) return null;

                try
                {
                    return await Task.FromResult(new ImageRenderer().GetTileBitmap(TileNumber));
                }
                catch (IndexOutOfRangeException e)
                {
                    Console.Out.WriteLine(e.ToString());
                    return null;
                }
            }

            private async void AsyncInit()
            {
                RenderedTile = await RenderTile();
            }

            public Tile(int tileNumber, bool passability, int objectNumber)
            {
                TileNumber = tileNumber;
                Passability = passability;
                ObjectNumber = objectNumber;
                AsyncInit();
            }

            public static Tile GetDefault()
            {
                return new Tile(0, true, 0);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (Tile)) return false;
                return Equals((Tile) obj);
            }

            public bool Equals(Tile other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return other.TileNumber == TileNumber && other.Passability.Equals(Passability) && other.ObjectNumber == ObjectNumber;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int result = TileNumber;
                    result = (result*397) ^ Passability.GetHashCode();
                    result = (result*397) ^ ObjectNumber;
                    return result;
                }
            }
        }

        public Map(string mapPath)
        {
            Name = Path.GetFileNameWithoutExtension(mapPath);
            IsEditable = false;
            
            bool encrypted = Path.GetExtension(mapPath).Equals(".mape");
            bool tileCompressed = Path.GetExtension(mapPath).Equals(".cmp");

            FileStream mapFileStream = File.Open(mapPath, FileMode.Open);
            Stream stream = encrypted ? LoadStream(mapPath) : mapFileStream;

            BinaryReader reader = new BinaryReader(stream);
            
            //CMP has an extra 'CMAP' header in the first 4 bytes
            if (tileCompressed)
            {
                string header = new string(reader.ReadChars(4));
                if (!header.Equals("CMAP"))
                {
                    reader.Close();
                    stream.Close();
                    throw new Exception("CMAP header missing, cannot parse cmp file");
                }
            }
            
            ushort sx = reader.ReadUInt16();
            ushort sy = reader.ReadUInt16();

            CreateEmptyMap(sx, sy);

            //If we are reading a CMP,change the stream under the reader to Deflate now
            if (tileCompressed)
            {
                reader = new BinaryReader(new InflaterInputStream(mapFileStream));
            }
            
            for (int y = 0; y < sy; y++)
            {
                for (int x = 0; x < sx; x++)
                {
                    ushort tileNumber = reader.ReadUInt16();
                    ushort passable = reader.ReadUInt16();
                    ushort objectNumber = reader.ReadUInt16();
                    MapData.Add(new Point(x, y), new Tile(tileNumber, Convert.ToBoolean(passable), objectNumber));
                }
            }

            reader.Close();
            stream.Close();
            IsModified = false;
        }

        public void Save(string mapPath)
        {
            bool encrypted = Path.GetExtension(mapPath).Equals(".mape");
            bool tileCompressed = Path.GetExtension(mapPath).Equals(".cmp");
            
            if (File.Exists(mapPath)) File.Delete(mapPath);

            FileStream mapFileStream = File.Create(mapPath);
            Stream mapStream = encrypted ? (Stream)new MemoryStream() : mapFileStream;
            BinaryWriter writer = new BinaryWriter(mapStream);

            //CMP has an extra 'CMAP' header in the first 4 bytes
            if (tileCompressed)
            {
                writer.Write("CMAP".ToCharArray());
            }
            
            writer.Write(Convert.ToInt16(Size.Width));
            writer.Write(Convert.ToInt16(Size.Height));

            //If we are writing a CMP, flush and change the stream under the writer to Deflate now
            if (tileCompressed)
            {
                writer.Flush();
                writer = new BinaryWriter(new DeflaterOutputStream(mapFileStream));
            }


            for (int y = 0; y < Size.Height; y++)
            {
                for (int x = 0; x < Size.Width; x++)
                {
                    writer.Write((short)((this[x, y] != null) ? this[x, y].TileNumber : 0));
                    writer.Write(Convert.ToInt16((this[x, y] == null) || this[x, y].Passability));
                    writer.Write((short)((this[x,y] != null) ? this[x,y].ObjectNumber : 0));
                }
            }

            if (encrypted) SaveStream((MemoryStream)mapStream, mapPath);
            writer.Close();
            mapStream.Close();
            Name = Path.GetFileNameWithoutExtension(mapPath);
            IsModified = false;
        }

        private static void SaveBool(BinaryWriter writer, bool boolValue)
        {
            writer.Write((byte)0);
            if (boolValue) writer.Write((byte) 1);
            else writer.Write((byte)0);
        }

        private static void SaveInt(BinaryWriter writer, int intValue)
        {
            byte byte1, byte2;
            IntTo2Bytes(intValue, out byte1, out byte2);
            writer.Write(byte1);
            writer.Write(byte2);
        }

        private static void IntTo2Bytes(int intValue, out byte byte1, out byte byte2)
        {
            byte1 = Convert.ToByte(intValue / 256);
            byte2 = Convert.ToByte(intValue - (intValue / 256) * 256);
        }

        public Tile this[int x, int y]
        {
            get
            {
                Point point = new Point(x, y);
                if (MapData.ContainsKey(point)) return MapData[point];
                return null;
            }
            set
            {
                if (!IsEditable) return;
                Point point = new Point(x, y);
                if (MapData.ContainsKey(point)) MapData.Remove(point);
                MapData.Add(point, value);
                IsModified = true;
            }
        }

        public Map(int width, int height)
        {
            CreateEmptyMap(width, height);
            IsEditable = true;
        }

        private void CreateEmptyMap(int width, int height)
        {
            Size = new Size(width, height);
            MapData = new Dictionary<Point, Tile>();
        }
        

        #region Compression/Encryption

        private static Stream LoadStream(string mapPath)
        {
            MemoryStream memoryStream = new MemoryStream();
            using (FileStream inFile = File.Open(mapPath, FileMode.Open))
            {
                using (GZipStream decompress = new GZipStream(inFile, CompressionMode.Decompress))
                {
                    byte[] buffer = new byte[4096];
                    int numRead;
                    while ((numRead = decompress.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        memoryStream.Write(buffer, 0, numRead);
                    }
                }
            }

            return EncryptionHelper.Decrypt(memoryStream);
        }

        private static void SaveStream(MemoryStream inStream, string mapPath)
        {
            using (Stream encryptedStream = EncryptionHelper.Encrypt(inStream))
            {
                using (FileStream outFile = File.Create(mapPath))
                {
                    using (GZipStream compress = new GZipStream(outFile, CompressionMode.Compress))
                    {
                        byte[] buffer = new byte[4096];
                        int numRead;
                        while ((numRead = encryptedStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            compress.Write(buffer, 0, numRead);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
