using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Tile;
using UnityEngine;

namespace Entities {
    public interface IUnitAction {
        bool HasBeenInterrupted { get; }
        IEnumerator Execute();
    }

    public class MoveAction : IUnitAction {
        private readonly MovingEntity movingEntity;
        private readonly List<TileEntity> path;

        public bool HasBeenInterrupted { get; private set; }

        public MoveAction(MovingEntity movingEntity, List<TileEntity> path) {
            this.movingEntity = movingEntity;
            this.path = path;
        }

        public IEnumerator Execute() {
            foreach (var tile in path) {
                if (tile.standingEntity != null) {
                    Debug.Log("Path blocked;");
                    HasBeenInterrupted = true;
                    yield break;
                }
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
        private readonly TileEntity target;
        
        public bool HasBeenInterrupted { get; private set; }

        public InteractAction(MovingEntity movingEntity, TileEntity target) {
            this.movingEntity = movingEntity;
            this.target = target;
        }

        public IEnumerator Execute() {
            if (target.standingEntity == null) {
                Debug.Log("Target is gone.");
                HasBeenInterrupted = true;
                yield break;
            }
            target.standingEntity.Interact(movingEntity);
            yield return null;
        }
    }
}
