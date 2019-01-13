using Entities.Tile;
using UnityEngine;

namespace Entities {
    public class MovingEntity : BaseEntity {
        public TileScript CurrentTile => Pathfinder.AllTiles[Position];

        private void Start() {
            Pathfinder.AllTiles[Position].standingEntity = this;
        }

        public void MoveTo(TileScript tile) {
            Pathfinder.AllTiles[Position].standingEntity = null;
            Position = tile.Position;
            tile.standingEntity = this;
            AlignToGrid();
        }
    }
}
