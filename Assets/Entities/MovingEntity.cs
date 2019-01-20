using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DigitalRuby.LightningBolt;
using Entities.Tile;
using UnityEngine;

namespace Entities {
    public class MovingEntity : BaseEntity {
        private const float EnergyTransferPerTick = 1000;

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

        private void Start() {
            Pathfinder.AllTiles[Position].standingEntity = this;
        }

        private void EnqueueAction(IUnitAction action) {
            actionQueue.Add(action);
            OnActionEnqueue?.Invoke(action);
        }

        public void EnqueueInteraction(List<TileEntity> path) {
            if (path.Last().standingEntity != null) {
                var target = path.Last();
                path.Remove(target);
                if (path.Count > 0) {
                    EnqueueAction(new MoveAction(this, path));
                }
                EnqueueAction(new InteractAction(this, target));
            } else {
                EnqueueAction(new MoveAction(this, path));
            }

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

        public override IEnumerator Interact(BaseEntity otherEntity) {
            Debug.Log("Transfer started");
            var originalPosition = otherEntity.Position;
            var lightning = Instantiate(transferLightningPrefab).GetComponent<LightningBoltScript>();
            lightning.StartObject = gameObject;
            lightning.EndObject = otherEntity.gameObject;
            while (actionQueue.Count <= 1 && IsPowered && otherEntity.Position == originalPosition && !actionQueue.First().HasBeenInterrupted) {
                transferEnergy(EnergyTransferPerTick, otherEntity);
                yield return null;
            }
            otherEntity.PowerUpCheck();
            Destroy(lightning.gameObject);
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
    }
}
