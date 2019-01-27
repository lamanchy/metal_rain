using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities.Tile;
using UnityEngine;

namespace Entities {
    public class MovingEntity : BaseEntity {
        private const int MaxActionQueueSize = 9;
        private const float EnergyTransferPerTick = 100;

        private readonly List<IUnitAction> actionQueue = new List<IUnitAction>();
        public IReadOnlyCollection<IUnitAction> ActionQueue => actionQueue;

        public event Action<IUnitAction> OnActionEnqueue;
        public event Action<int> OnActionDequeue;

        private bool isExecutingActions;
        
        [Header("Moving entity stats")]
        public float baseMoveSpeed = 3;
        // TODO Apply terrain modifiers
        public float MoveSpeedModifier => baseMoveSpeed;

        public TileEntity DestinationTile => GetDestinationTile();
        public TileEntity CurrentTile => Pathfinder.AllTiles[Position];

        public static event Action<MovingEntity> OnMovingEntityDestroyed;

        private void EnqueueAction(IUnitAction action) {
            if (actionQueue.Count == MaxActionQueueSize) {
                return;
            }
            actionQueue.Add(action);
            OnActionEnqueue?.Invoke(action);
        }

        public void EnqueueInteraction(List<TileEntity> path, bool isPrimary) {
            if (path.Last().standingEntity != null) {
                var target = path.Last();
                path.Remove(target);
                if (path.Count > 0) {
                    EnqueueAction(new MoveAction(this, path));
                }
                EnqueueAction(new InteractAction(this, target, isPrimary));
            } else {
                EnqueueAction(new MoveAction(this, path));
            }

            if (!isExecutingActions) {
                StartCoroutine(DequeueCoroutine());
            }
        }

        public void EnqueueBuild(List<TileEntity> path, GameObject prefab) {
            if(path == null) { return; }

            if (path.Last().standingEntity != null) {
                Debug.Log("Can't build on occupied tile.");
                return;
            }
            
            var target = path.Last();
            path.Remove(target);
            if (path.Count > 0) {
                EnqueueAction(new MoveAction(this, path));
            }
            EnqueueAction(new BuildAction(this, target, prefab));

            if (!isExecutingActions) {
                StartCoroutine(DequeueCoroutine());
            }
        }

        private IEnumerator DequeueCoroutine() {
            isExecutingActions = true;
            while (actionQueue.Count != 0) {
                var action = actionQueue.First();
                Debug.Log(action);
                yield return action.Execute();
                actionQueue.RemoveAt(0);
                OnActionDequeue?.Invoke(0);
                if (action.HasBeenInterrupted) {
                    ClearActionQueue();
                    break;
                }
            }
            isExecutingActions = false;
        }

        public IEnumerator Interact(BaseEntity otherEntity, bool isPrimary) {
            Debug.Log("Transfer started");
            var originalPosition = otherEntity.Position;

            using (new EnergyTransferEffect(gameObject, otherEntity.gameObject)) {
                var i = 40;
                var shouldContinue = true;
                while (IsPowered 
                    && (isPrimary || otherEntity.IsPowered)
                    && otherEntity.Position == originalPosition 
                    && !actionQueue.First().HasBeenInterrupted
                    && (i > 0 || !isPrimary)  // limit only giving, not receiving
                    && shouldContinue) {
                    shouldContinue = TransferEnergy(isPrimary ? EnergyTransferPerTick : -EnergyTransferPerTick, otherEntity);
                    i--;
                    yield return null;
                }
            }
            
            otherEntity.PowerUpCheck();
            Debug.Log("Transfer ended");
        }

        public void CancelLastAction() {
            if (!isExecutingActions) {
                return;
            }
            if (actionQueue.Count == 1) {
                Interrupt();
                return;
            }
            actionQueue.RemoveAt(actionQueue.Count - 1);
            OnActionDequeue?.Invoke(actionQueue.Count - 1);

        }

        public void Interrupt() {
            if (!isExecutingActions) {
                return;
            }
            actionQueue.First().HasBeenInterrupted = true;
        }

        public void ClearActionQueue() {
            for (var i = actionQueue.Count - 1; i >= 0; --i) {
                OnActionDequeue?.Invoke(i);
            }
            actionQueue.Clear();
            Pathfinder.RepaintHexColors();
        }

        private TileEntity GetDestinationTile() {
            for (var i = actionQueue.Count - 1; i >= 0; i--) {
                if (actionQueue[i] is MoveAction) {
                    return ((MoveAction) actionQueue[i]).Path.Last();
                }
            }
            return CurrentTile;
        }

        public override void Explode() {
            base.Explode();
            OnMovingEntityDestroyed?.Invoke(this);
        }
    }
}
