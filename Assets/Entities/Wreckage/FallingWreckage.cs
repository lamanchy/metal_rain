using Entities;
using Entities.Tile;
using Entities.Wreckage;
using Manager;
using UnityEngine;

namespace Meteor {
    public class FallingWreckage : MonoBehaviour {
        private Rigidbody rigidBody;
        private TimeManager timeManager;

        public GameObject FallenWreckagePrefab;

        public float Energy;

        [HideInInspector] public Vector3 Velocity;
        [HideInInspector] public Vector3 AngularVelocity;

        private void Start() {
            rigidBody = GetComponent<Rigidbody>();
            timeManager = FindObjectOfType<TimeManager>();
        
            SetSpeed();
        }

        public void SetSpeed() {
            if (timeManager.running) {
                rigidBody.velocity = Velocity;
                rigidBody.angularVelocity = AngularVelocity;
            } else {
                rigidBody.velocity = new Vector3(0, 0, 0);
                rigidBody.angularVelocity = new Vector3(0, 0, 0);
            }
        }

        private void OnCollisionEnter(Collision collision) {
            var tile = collision.gameObject.GetComponent<TileEntity>();
            if (tile == null) {
                return;
            }

            if (transform.position.x < collision.gameObject.transform.position.x) {
                // Wreckage hit the tile from side
                Destroy(gameObject);
                return;
            }

            // TODO impact explosion

            if (tile.standingEntity != null && Energy < tile.standingEntity.Energy) {
                // Wreckage hit entity with more power and is destroyed
                tile.standingEntity.Energy -= Energy;
                return;
            }

            var fallenWreckage = Instantiate(FallenWreckagePrefab).GetComponent<FallenWreckage>();
            fallenWreckage.Position = tile.Position;
            fallenWreckage.AlignToGrid();
            tile.standingEntity = fallenWreckage;

            Destroy(gameObject);
        }

        private void Update() {
            // TODO rework pause system
            // if (timeManager.hasChanged) {
            //     SetSpeed();
            // }

            // TODO add collider to y0 instead of this check in update
            if (transform.position.y < 0) {
                Destroy(gameObject);
            }
        }
    }
}
