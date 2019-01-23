using Entities;
using Entities.Tile;
using Entities.Wreckage;
using Manager;
using UnityEngine;

namespace Meteor {
    public class FallingWreckage : MonoBehaviour {
        private Rigidbody rigidBody;
        private TimeManager timeManager;

        private bool hasLanded;

        public GameObject FallenWreckagePrefab;

        public float Energy;

        public const int MaximumEnergy = 10000;
        public const int MinimumEnergy = 100;

        [HideInInspector] public Vector3 Velocity;
        [HideInInspector] public Vector3 AngularVelocity;

        private float SizeFactor => (Energy - MinimumEnergy) / MaximumEnergy;

        private void Start() {
            rigidBody = GetComponent<Rigidbody>();
            timeManager = FindObjectOfType<TimeManager>();

            Energy = Random.Range(MinimumEnergy, MaximumEnergy);
            
            Velocity = new Vector3(
                -2 + Random.Range(-0.1f, 0.1f),
                -2 + Random.Range(-0.1f, 0.1f),
                2 + Random.Range(-0.1f, 0.1f)
            ) / SizeFactor;

            AngularVelocity = new Vector3(
                2 * Random.Range(0.1f, 2f) * 2 - 1 + Random.Range(-1f, 1f),
                2 * Random.Range(0.1f, 2f) * 2 - 1 + Random.Range(-1f, 1f),
                2 * Random.Range(0.1f, 2f) * 2 - 1 + Random.Range(-1f, 1f)
            ) / SizeFactor;

            transform.localScale *= SizeFactor;

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
            if (hasLanded) {
                // Has collided with multiple tiles at once
                return;
            }

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

            // fallenWreckage.transform.position = transform.position;
            fallenWreckage.transform.localScale *= SizeFactor;
            fallenWreckage.transform.rotation = transform.rotation;
            
            tile.standingEntity = fallenWreckage;

            hasLanded = true;
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
