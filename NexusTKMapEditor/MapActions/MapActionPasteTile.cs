﻿using System.Drawing;

namespace NexusTKMapEditor.MapActions
{
    public class MapActionPasteTile : IMapAction
    {
        public Point Tile { get; set; }
        private readonly int oldTileNumber, newTileNumber;

        public MapActionPasteTile(Point tile, int oldTileNumber, int newTileNumber)
        {
            Tile = tile;
            this.oldTileNumber = oldTileNumber;
            this.newTileNumber = newTileNumber;
        }

        public void Undo(Map map)
        {
            map[Tile.X, Tile.Y] = map[Tile.X, Tile.Y] ?? NexusTKMapEditor.Tile.DefaultTile;
            map[Tile.X, Tile.Y].TileNumber = oldTileNumber;
        }

        public void Redo(Map map)
        {
            map[Tile.X, Tile.Y] = map[Tile.X, Tile.Y] ?? NexusTKMapEditor.Tile.DefaultTile;
            map[Tile.X, Tile.Y].TileNumber = newTileNumber;
        }
    }
}
