using System;
using System.Collections;
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

        [Header("Base prefabs")]
        public GameObject transferLightningPrefab;

        public float EnergyPerTick => EnergyPerSecond / 60f;

        [HideInInspector]
        public bool IsPowered = true;

        private Pathfinder pathfinder;
        public Pathfinder Pathfinder => pathfinder ? pathfinder : pathfinder = FindObjectOfType<Pathfinder>();

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

        private void FixedUpdate() {
            if (!IsPowered) {
                return;
            }

            Energy = Mathf.Clamp(Energy + EnergyPerTick, 0, MaxEnergy);
            PowerDownCheck();
        }

        protected void transferEnergy(float ammount, BaseEntity target) {
            if (ammount > Energy) {
                ammount = Energy;
            }
            if (target.MaxEnergy - target.Energy < ammount) {
                ammount = target.MaxEnergy - target.Energy;
            }
            target.Energy += ammount;
            Energy -= ammount;
            PowerDownCheck();
        }

        public virtual IEnumerator Interact(BaseEntity otherEntity) {
            return null;
        }

        protected virtual void PowerDown() {}
        protected virtual void PowerUp() {}

        [ContextMenu("Align to grid")]
        public void AlignToGrid() => transform.position = Pathfinder.GetWorldPosition(Position);
    }
}
