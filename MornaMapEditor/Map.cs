using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace MornaMapEditor
{
    public class Map
    {
        public string Name { get; set; }
        public Size Size { get; set; }
        public bool IsModified { get; set; }
        public bool IsEditable { get; set; }
        public Dictionary<Point, Tile> MapData { get; private set; }

        public Map(string mapPath)
        {
            Name = Path.GetFileNameWithoutExtension(mapPath);
            IsEditable = false;
            
            bool tileCompressed = Path.GetExtension(mapPath).Equals(".cmp");

            FileStream mapFileStream = File.Open(mapPath, FileMode.Open);

            BinaryReader reader = new BinaryReader(mapFileStream);
            
            //CMP has an extra 'CMAP' header in the first 4 bytes
            if (tileCompressed)
            {
                string header = new string(reader.ReadChars(4));
                if (!header.Equals("CMAP"))
                {
                    reader.Close();
                    mapFileStream.Close();
                    throw new Exception("CMAP header missing, cannot parse cmp file");
                }
            }
            
            short sx = reader.ReadInt16();
            short sy = reader.ReadInt16();

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
                    short tileNumber = reader.ReadInt16();
                    short passable = reader.ReadInt16();
                    short objectNumber = reader.ReadInt16();
                    MapData.Add(new Point(x, y), new Tile(tileNumber, Convert.ToBoolean(passable), objectNumber));
                }
            }

            reader.Close();
            mapFileStream.Close();
            IsModified = false;
        }

        public void Save(string mapPath)
        {
            bool tileCompressed = Path.GetExtension(mapPath).Equals(".cmp");
            
            if (File.Exists(mapPath)) File.Delete(mapPath);

            FileStream mapFileStream = File.Create(mapPath);
            BinaryWriter writer = new BinaryWriter(mapFileStream);

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
                    writer.Write(Convert.ToInt16((this[x, y] == null) || this[x, y].Passable));
                    writer.Write((short)((this[x,y] != null) ? this[x,y].ObjectNumber : 0));
                }
            }

            writer.Close();
            mapFileStream.Close();
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
    }
}
