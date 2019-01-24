using System;
using Manager;
using UnityEngine;

namespace Entities {
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class BaseEntity : MonoBehaviour {
        public Vector3Int Position;
        
        [Header("Base stats")]
        public float Energy;
        public int MaxEnergy;
        public int EnergyPerSecond;

        public float EnergyPerTick => EnergyPerSecond / 60f;

        [HideInInspector]
        public bool IsPowered = true;

        private Pathfinder pathfinder;
        public Pathfinder Pathfinder => pathfinder ? pathfinder : pathfinder = FindObjectOfType<Pathfinder>();

        protected virtual void Start() {
            Pathfinder.AllTiles[Position].standingEntity = this;
            PowerUpCheck();
        }

        public void PowerDownCheck() {
            if (Math.Abs(Energy) < 0.1f) {
                IsPowered = false;
                PowerDown();
            }
        }

        public void PowerUpCheck() {
            if (Energy > 0.1f) {
                IsPowered = true;
                PowerUp();
            }
        }

        protected virtual void FixedUpdate() {
            if (!IsPowered) {
                return;
            }

            Energy = Mathf.Clamp(Energy + EnergyPerTick, 0, MaxEnergy);
            PowerDownCheck();
        }

        /// <summary>
        /// Transfers energy from this entity to target entity. Check both over/underflow and powering down.
        /// </summary>
        /// <param name="amount">Amount of energy to transfer. If negative, energy will be extracted instead.</param>
        /// <param name="target">Other entity to receive the energy.</param>
        protected void TransferEnergy(float amount, BaseEntity target) {
            if (amount >= 0) {
                // Giving energy
                if (amount > Energy) {
                    amount = Energy;
                }
                if (target.MaxEnergy - target.Energy < amount) {
                    amount = target.MaxEnergy - target.Energy;
                }
            } else {
                // Receiving energy
                if (Math.Abs(amount) > target.Energy) {
                    amount = -target.Energy;
                }
                if (MaxEnergy - Energy < Math.Abs(amount)) {
                    amount = Energy - MaxEnergy;
                }
            }
            target.Energy += amount;
            Energy -= amount;
            PowerDownCheck();
            target.PowerDownCheck();
        }

        protected virtual void PowerDown() {}
        protected virtual void PowerUp() {}

        public virtual void DestroySelf() {
            Pathfinder.AllTiles[Position].standingEntity = null;
            Destroy(gameObject);
        }

        [ContextMenu("Align to grid")]
        public void AlignToGrid() => transform.position = Pathfinder.GetWorldPosition(Position);
    }
}
