﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities.Tile;
using UnityEngine;

namespace Entities {
    public class MovingEntity : BaseEntity {
        private const float EnergyTransferPerTick = 1000;

        private readonly Queue<IUnitAction> actionQueue = new Queue<IUnitAction>();
        public IReadOnlyCollection<IUnitAction> ActionQueue => actionQueue;

        public event Action<IUnitAction> OnActionEnqueue;
        public event Action<IUnitAction> OnActionDequeue;

        private bool isExecutingActions;
        
        public float baseMoveSpeed = 3;
        // TODO Apply terrain modifiers
        public float MoveSpeedModifier => baseMoveSpeed;
        
        public readonly List<TileEntity> PathQueue = new List<TileEntity>();
        public readonly List<TileEntity> InteractionQueue = new List<TileEntity>();

        public TileEntity DestinationTile => PathQueue.Count == 0 ? CurrentTile : PathQueue[PathQueue.Count - 1];
        public TileEntity CurrentTile => Pathfinder.AllTiles[Position];

        private void Start() {
            Pathfinder.AllTiles[Position].standingEntity = this;
        }

        private void EnqueueAction(IUnitAction action) {
            actionQueue.Enqueue(action);
            OnActionEnqueue?.Invoke(action);
        }

        public void EnqueueInteraction(List<TileEntity> path) {
            if (path.Last().standingEntity != null) {
                var target = path.Last();
                path.Remove(target);
                if (path.Count > 0) {
                    PathQueue.AddRange(path);
                    EnqueueAction(new MoveAction(this, path));
                }
                InteractionQueue.Add(target);
                EnqueueAction(new InteractAction(this, target));
            } else {
                PathQueue.AddRange(path);
                EnqueueAction(new MoveAction(this, path));
            }

            if (!isExecutingActions) {
                StartCoroutine(DequeueCoroutine());
            }
        }

        private IEnumerator DequeueCoroutine() {
            isExecutingActions = true;
            while (actionQueue.Count != 0) {
                var action = actionQueue.Dequeue();
                Debug.Log(action);
                yield return StartCoroutine(action.Execute());
                OnActionDequeue?.Invoke(action);
                if (action.HasBeenInterrupted) {
                    for (var i = 0; i < actionQueue.Count; ++i) {
                        OnActionDequeue?.Invoke(null);
                    }
                    actionQueue.Clear();
                    PathQueue.Clear();
                    InteractionQueue.Clear();
                    Pathfinder.RepaintHexColors();
                    break;
                }
            }
            isExecutingActions = false;
        }

        public override IEnumerator Interact(BaseEntity otherEntity) {
            Debug.Log("Transfer started");
            var originalPosition = otherEntity.Position;
            while (actionQueue.Count == 0 && IsPowered && otherEntity.Position == originalPosition) {
                transferEnergy(EnergyTransferPerTick, otherEntity);
                yield return null;
            }
            otherEntity.PowerUpCheck();
            Debug.Log("Transfer ended");
        }
    }
}
