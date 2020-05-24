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
    }
}