using UnityEngine;

namespace Manager {
    public class WreckageGenerator : MonoBehaviour {
        private const int WreckageSpawnDistance = 20;

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
            // peak at 60 seconds, calm at 180 seconds, period 120
            var probability = (Mathf.Sin(Time.timeSinceLevelLoad * 0.02617993877f) + 1) * 50;
            if (timeManager.running && probability > Random.Range(0, 100)) {
                GenerateMeteor();
            }
        }
    }
}
