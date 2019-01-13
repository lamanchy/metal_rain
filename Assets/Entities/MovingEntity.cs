using System.Collections;
using System.Collections.Generic;
using Entities.Tile;

namespace Entities {
    public class MovingEntity : BaseEntity {
        private readonly Queue<IUnitAction> actionQueue = new Queue<IUnitAction>();
        public readonly List<TileEntity> PathQueue = new List<TileEntity>();
        private bool isExecutingActions;

        public TileEntity DestinationTile => PathQueue.Count == 0 ? CurrentTile : PathQueue[PathQueue.Count - 1];
        public TileEntity CurrentTile => Pathfinder.AllTiles[Position];

        private void Start() {
            Pathfinder.AllTiles[Position].standingEntity = this;
        }

        public void MoveTo(List<TileEntity> path) {
            path.Reverse();
            PathQueue.AddRange(path);
            actionQueue.Enqueue(new MoveAction(this, path));
            if (!isExecutingActions) {
                StartCoroutine(DequeueCoroutine());
            }
        }

        private IEnumerator DequeueCoroutine() {
            isExecutingActions = true;
            while (actionQueue.Count != 0) {
                yield return StartCoroutine(actionQueue.Dequeue().Execute());
            }
            isExecutingActions = false;
        }
    }
}
