using Meteor;
using UnityEngine;

namespace Entities.Buildings {
    public class ShieldGenerator : StaticEntity {
        private GameObject shield;
        protected override void Start() {
            base.Start();
            
            shield = transform.Find("Shield").gameObject;
        }

        private void OnTriggerEnter(Collider other) {
            if (!IsPowered) {
                return;
            }

            var fallingWreckage = other.GetComponent<FallingWreckage>();
            if (fallingWreckage == null) {
                return;
            }

            if (Energy > fallingWreckage.Energy) {
                Destroy(fallingWreckage.gameObject);
                Energy -= fallingWreckage.Energy;
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
