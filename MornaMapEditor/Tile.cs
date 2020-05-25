using System.Drawing;

namespace MornaMapEditor
{
    public class Tile
    {
        public static Tile DefaultTile { get; }

        static Tile()
        {
            DefaultTile = new Tile(0, true, 0);
        }
        
        public Tile(int tileNumber, bool passable, int objectNumber)
        {
            TileNumber = tileNumber;
            Passable = passable;
            ObjectNumber = objectNumber;
        }

        public int TileNumber { get; set; }
        public bool Passable { get; set; }
        public int ObjectNumber { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Tile)) return false;
            return Equals((Tile) obj);
        }

        private bool Equals(Tile other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.TileNumber == TileNumber && other.Passable.Equals(Passable) && other.ObjectNumber == ObjectNumber;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = TileNumber;
                result = (result*397) ^ Passable.GetHashCode();
                result = (result*397) ^ ObjectNumber;
                return result;
            }
        }

        public Bitmap RenderTile()
        {
            if (TileNumber <= 0) return null;
            if (TileNumber >= TileManager.Epf[0].max) return null;
            return (Bitmap)ImageRenderer.Singleton.GetTileBitmap(TileNumber).Clone();
        }

        public Bitmap RenderObjects(Tile[] tilesWithPossibleObjects)
        {
            int sizeModifier = ImageRenderer.Singleton.sizeModifier;
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
                            graphics.DrawImage(ImageRenderer.Singleton.GetObjectBitmap(objTileNumber), 0, 0);
                        }
                    }
                }
            }
            graphics.Dispose();
            return bitmap;
        }
    }
}