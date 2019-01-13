using Entities.Tile;
using UnityEngine;

namespace Entities {
    public class MovingEntity : BaseEntity {
        public TileEntity CurrentTile => Pathfinder.AllTiles[Position];

        private void Start() {
            Pathfinder.AllTiles[Position].standingEntity = this;
        }

        public void MoveTo(TileEntity tile) {
            Pathfinder.AllTiles[Position].standingEntity = null;
            Position = tile.Position;
            tile.standingEntity = this;
            AlignToGrid();
        }
    }
}
