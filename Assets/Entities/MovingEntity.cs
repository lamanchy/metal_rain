using System.Collections;
using System.Collections.Generic;
using Entities.Tile;
using UnityEngine;

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

        public void EnqueueInteraction(List<TileEntity> path) {
            var target = path[0].standingEntity;
            if (target != null) {
                path.RemoveAt(0);
            }

            path.Reverse();
            PathQueue.AddRange(path);
            actionQueue.Enqueue(new MoveAction(this, path));

            if (target != null) {
                actionQueue.Enqueue(new InteractAction(this, target));
            }

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

        public override void Interact(BaseEntity otherEntity) {
            Debug.Log("Interacted");
        }
    }
}
