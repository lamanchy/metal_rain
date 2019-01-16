﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities.Tile;
using UnityEngine;

namespace Entities {
    public class MovingEntity : BaseEntity {
        private readonly Queue<IUnitAction> actionQueue = new Queue<IUnitAction>();

        private bool isExecutingActions;
        
        public readonly List<TileEntity> PathQueue = new List<TileEntity>();
        public readonly List<TileEntity> InteractionQueue = new List<TileEntity>();

        public TileEntity DestinationTile => PathQueue.Count == 0 ? CurrentTile : PathQueue[PathQueue.Count - 1];
        public TileEntity CurrentTile => Pathfinder.AllTiles[position];

        private void Start() {
            Pathfinder.AllTiles[position].standingEntity = this;
        }

        public void EnqueueInteraction(List<TileEntity> path) {
            if (path.Last().standingEntity != null) {
                var target = path.Last();
                path.Remove(target);
                PathQueue.AddRange(path);
                actionQueue.Enqueue(new MoveAction(this, path));
                InteractionQueue.Add(target);
                actionQueue.Enqueue(new InteractAction(this, target));
            } else {
                PathQueue.AddRange(path);
                actionQueue.Enqueue(new MoveAction(this, path));
            }

            if (!isExecutingActions) {
                StartCoroutine(DequeueCoroutine());
            }
        }

        private IEnumerator DequeueCoroutine() {
            isExecutingActions = true;
            while (actionQueue.Count != 0) {
                var action = actionQueue.Dequeue();
                yield return StartCoroutine(action.Execute());
                if (action.HasBeenInterrupted) {
                    actionQueue.Clear();
                    PathQueue.Clear();
                    InteractionQueue.Clear();
                    Pathfinder.RepaintHexColors();
                    break;
                }
            }
            isExecutingActions = false;
        }

        public override void Interact(BaseEntity otherEntity) {
            Debug.Log("Interacted");
        }
    }
}
