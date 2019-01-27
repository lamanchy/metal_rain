using Meteor;
using UnityEngine;

namespace Manager {
    public class WreckageGenerator : MonoBehaviour {
        private const int WreckageSpawnDistance = 25;

        private TimeManager timeManager;

        public GameObject WreckagePrefab;

        private void Start() {
            timeManager = FindObjectOfType<TimeManager>();
            for (var i = 0; i < 10; i++) {
                GenerateMeteor();
            }
        }

        private void GenerateMeteor() {
            var instance = Instantiate(WreckagePrefab, transform);
            instance.transform.position = new Vector3(Random.Range(0f, 50f) + WreckageSpawnDistance, WreckageSpawnDistance, Random.Range(0f, 50f) - WreckageSpawnDistance);
        }
        
        private void Update() {
            if (timeManager.running && 98 > Random.Range(0, 100)) {
                GenerateMeteor();
            }
        }
    }
}
