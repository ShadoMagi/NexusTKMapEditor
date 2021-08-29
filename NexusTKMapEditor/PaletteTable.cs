﻿using System.IO;

namespace NexusTKMapEditor
{
    public sealed class PaletteTable
    {
        string name;
        int[] paletteEntries;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Count
        {
            get { return paletteEntries.Length; }
        }

        public int this[int index]
        {
            get { return paletteEntries[index]; }
        }

        public PaletteTable(string file)
        :this(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read), leaveOpen: false)
        {
            this.name = Path.GetFileName(file);
        }

        public PaletteTable(Stream stream, bool leaveOpen = true)
        {
            ReadPaletteEntries(stream);

            if(!leaveOpen)
                stream.Dispose();
        }

        void ReadPaletteEntries(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);

            var entryCount = reader.ReadUInt32();
            paletteEntries = new int[entryCount];

            for(int i = 0; i < entryCount; i++)
            {
                byte lo = reader.ReadByte();
                byte hi = reader.ReadByte();

                int paletteIndex = ((hi & 0x7F) << 8) | lo;
                paletteEntries[i] = paletteIndex;
            }
            reader.Close();
        }

        public override string ToString()
        {
            return string.Format("{0}, Count = {1}",
                                name ?? "<null>",
                                paletteEntries.Length.ToString());
        }
        /*private int[] table;

        public static PaletteTable LoadTBL(string file)
        {
            PaletteTable nT = new PaletteTable();
            byte[] buffer = File.ReadAllBytes(file);
            MemoryStream input = new MemoryStream(buffer);
            BinaryReader reader = new BinaryReader(input);

            int count = reader.ReadUInt16();
            nT.table = new int[count];
            reader.BaseStream.Seek(4, SeekOrigin.Begin);
            for (int x = 0; x < count; x++)
            {
                int tbl = reader.ReadUInt16();
                //reader.BaseStream.Seek(1, SeekOrigin.Current);
//#pragma warning disable 168
                //int tbl2 = reader.ReadByte();
//#pragma warning restore 168
                nT.table[x] = tbl;
            }
            reader.Close();
            input.Dispose();
            return nT;
        }

        public int tbl(int frame)
        {
            return table[frame];
        }

        public byte tblb(int frame)
        {
            return (byte)table[frame];
        }*/
    }
}
