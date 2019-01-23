using Meteor;
using UnityEngine;

namespace Manager {
    public class WreckageGenerator : MonoBehaviour {
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
            const int distance = 25;
            instance.transform.position = new Vector3(Random.Range(0f, 50f) + distance, distance, Random.Range(0f, 50f) - distance);
            var sizeFactor = Mathf.Sqrt(Random.Range(0.1f, 4f));

            instance.transform.localScale *= sizeFactor;
            var meteorScript = instance.GetComponent<FallingWreckage>();

            meteorScript.Velocity = new Vector3(-2 + Random.Range(-0.1f, 0.1f),
                                        -2 + Random.Range(-0.1f, 0.1f),
                                        2 + Random.Range(-0.1f, 0.1f)) / sizeFactor;
            meteorScript.AngularVelocity = new Vector3(2 * Random.Range(0, 2) * 2 - 1 + Random.Range(-1f, 1f),
                                               2 * Random.Range(0, 2) * 2 - 1 + Random.Range(-1f, 1f),
                                               2 * Random.Range(0, 2) * 2 - 1 + Random.Range(-1f, 1f)) / sizeFactor;
        }
        
        private void Update() {
            if (timeManager.running && 80 > Random.Range(0, 100)) {
                GenerateMeteor();
            }
        }
    }
}
