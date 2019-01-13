using Entities.Tile;
using UnityEngine;

namespace Entities {
    public class MovingEntity : BaseEntity {
        public TileScript CurrentTile => Pathfinder.AllTiles[Position];

        public void MoveTo(TileScript tile) {
            Position = tile.Position;
            AlignToGrid();
        }
    }
}
