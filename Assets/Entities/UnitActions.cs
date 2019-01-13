using System.Collections;
using System.Collections.Generic;
using Entities.Tile;
using UnityEngine;

namespace Entities {
    public interface IUnitAction {
        IEnumerator Execute();
    }

    public class MoveAction : IUnitAction {
        private readonly MovingEntity movingEntity;
        private readonly List<TileEntity> path;

        public MoveAction(MovingEntity movingEntity, List<TileEntity> path) {
            this.movingEntity = movingEntity;
            this.path = path;
        }

        public IEnumerator Execute() {
            foreach (var tile in path) {
                tile.standingEntity = movingEntity;
                var startingPosition = movingEntity.transform.position;
                var destinationPosition = tile.transform.position;
                for (var i = 0f; i < 1f; i += 0.03f) {
                    movingEntity.transform.position = Vector3.Lerp(startingPosition, destinationPosition, i);
                    yield return null;
                }
                movingEntity.Pathfinder.AllTiles[movingEntity.Position].standingEntity = null;
                movingEntity.Position = tile.Position;
                movingEntity.PathQueue.Remove(tile);
            }
        }
    }

    public class InteractAction : IUnitAction {
        private readonly MovingEntity movingEntity;
        private readonly BaseEntity target;

        public InteractAction(MovingEntity movingEntity, BaseEntity target) {
            this.movingEntity = movingEntity;
            this.target = target;
        }

        public IEnumerator Execute() {
            target.Interact(movingEntity);
            yield return null;
        }
    }
}
