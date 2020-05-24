using System;
using System.IO;

namespace MornaMapEditor
{
    public class ObjectInfo
    {
        private int unknownInt;
        private byte unknownByte;
        private byte movementDirections;
        private int height;
        private int[] indices;
        /// <value>
        /// The height of this object slice, in tiles.
        /// </value>
        public int Height
        {
            get { return height; }
        }
        public int[] Indices
        {
            get { return indices; }
        }
        public static ObjectInfo[] ReadCollection(string path)
        {
            Stream stream = new FileStream(path, FileMode.Open);

            BinaryReader binaryReader = new BinaryReader(stream);
            var count = binaryReader.ReadUInt32();
            ObjectInfo[] infoCollection = new ObjectInfo[count];
            ObjectInfo info = infoCollection[0] = new ObjectInfo();
            info.height = binaryReader.ReadByte();
            info.indices = new int[info.height];
            info.indices[0] = binaryReader.ReadByte();
            for (int index = 1; index < count; ++index)
            {
                info = infoCollection[index] = new ObjectInfo();
                info.unknownInt = binaryReader.ReadInt32();
                info.unknownByte = binaryReader.ReadByte();
                info.movementDirections = binaryReader.ReadByte();
                info.height = binaryReader.ReadByte();
                info.indices = new int[info.height];
                for (int subIndex = 0; subIndex < info.height; subIndex++)
                    info.indices[subIndex] = binaryReader.ReadUInt16();
                Array.Reverse(info.indices);
            }

            binaryReader.Close();
            stream.Dispose();
            return infoCollection;
        }
    }
}
