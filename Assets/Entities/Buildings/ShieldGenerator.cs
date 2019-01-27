using Meteor;
using UnityEngine;

namespace Entities.Buildings {
    public class ShieldGenerator : StaticEntity {
        private GameObject shield;

        protected override void Start() {
            shield = transform.Find("Shield").gameObject;
            base.Start();
        }

        private void OnTriggerEnter(Collider other) {
            if (!IsPowered) {
                return;
            }

            var fallingWreckage = other.GetComponent<FallingWreckage>();
            if (fallingWreckage == null) {
                return;
            }

            var impactEnergy = fallingWreckage.Energy * fallingWreckage.Energy / 100000;
            if (Energy > impactEnergy) {
                fallingWreckage.Explode();
                Energy -= impactEnergy;
            } else {
                Energy = 0;
            }
        }

        protected override void PowerUp() {
            base.PowerUp();

            shield.SetActive(true);
        }

        protected override void PowerDown() {
            base.PowerDown();

            shield.SetActive(false);
        }
    }
}
