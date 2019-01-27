using System;
using System.Collections.Generic;
using Entities.Tile;
using Entities.Wreckage;
using Manager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Meteor {
    public class FallingWreckage : MonoBehaviour {
        private static Transform fallenWreckageContainer;
        private static PrefabContainer prefabContainer;
        private static TimeManager timeManager;

        private Rigidbody rigidBody;
        private MeshFilter meshFilter;

        private bool hasLanded;

        public GameObject FallenWreckagePrefab;

        public float Energy;

        public const int MaximumEnergy = 10000;
        public const int MinimumEnergy = 100;

        public static event Action<FallenWreckage, FallingWreckage> OnWreckageFallen;

        [HideInInspector] public Vector3 Velocity;
        [HideInInspector] public Vector3 AngularVelocity;

        private float SizeFactor => (Energy - MinimumEnergy) / MaximumEnergy;

        private void Start() {
            // Initialize static members
            if (fallenWreckageContainer == null) {
                fallenWreckageContainer = GameObject.Find("FallenWreckageContainer").transform;
            }
            if (prefabContainer == null) {
                prefabContainer = PrefabContainer.Instance;
            }
            if (timeManager == null) {
                timeManager = FindObjectOfType<TimeManager>();
            }
            
            rigidBody = GetComponent<Rigidbody>();
            meshFilter = GetComponent<MeshFilter>();

            Energy = Random.Range(MinimumEnergy, MaximumEnergy);
            meshFilter.sharedMesh = prefabContainer.GetWreckageMesh(Energy);
            
            const float velocityMultiplier = 0.1f;
            Velocity = new Vector3(
                -2 + Random.Range(-0.1f, 0.1f),
                -2 + Random.Range(-0.1f, 0.1f),
                2 + Random.Range(-0.1f, 0.1f)
            ) / SizeFactor * velocityMultiplier;

            AngularVelocity = new Vector3(
                2 * Random.Range(0.1f, 2f) * 2 - 1 + Random.Range(-1f, 1f),
                2 * Random.Range(0.1f, 2f) * 2 - 1 + Random.Range(-1f, 1f),
                2 * Random.Range(0.1f, 2f) * 2 - 1 + Random.Range(-1f, 1f)
            ) / SizeFactor * velocityMultiplier;

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

            tile.standingEntity?.Explode();

            var fallenWreckage = Instantiate(FallenWreckagePrefab, fallenWreckageContainer).GetComponent<FallenWreckage>();
            fallenWreckage.Energy = Energy / 5;
            fallenWreckage.Position = tile.Position;
            fallenWreckage.GetComponent<MeshFilter>().sharedMesh = meshFilter.sharedMesh;
            fallenWreckage.AlignToGrid();
            
            fallenWreckage.transform.rotation = transform.rotation;

            OnWreckageFallen?.Invoke(fallenWreckage, this);

            hasLanded = true;
            Explode();
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

        public void Explode() {
            Destroy(gameObject);
        }

        public IEnumerable<FallingWreckage> Split() {
            var splitEnergy = Energy / 3f;
            var firstSplit = Instantiate(this);
            firstSplit.Energy = splitEnergy;
            var secondSplit = Instantiate(this);
            secondSplit.Energy = splitEnergy;

            Explode();
            return new []{ firstSplit, secondSplit };
        }
    }
}
