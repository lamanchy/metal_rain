using Meteor;
using UnityEngine;

namespace Manager {
    public class MeteorGenerator : MonoBehaviour
    {
        public GameObject meteor;
        private TimeManager timeManager;
        Pathfinder pathfinder;

        private void Start()
        {
            timeManager = FindObjectOfType<TimeManager>();
            pathfinder = FindObjectOfType<Pathfinder>();
            for (var i = 0; i < 10; i++)
                GenerateMeteor();
        }

        private void GenerateMeteor()
        {
            var instance = Instantiate(meteor, transform);
            const int distance = 50;
            instance.transform.position = new Vector3(Random.Range(0f, 50f) + distance, distance, Random.Range(0f, 50f) - distance);
            var sizeFactor = Mathf.Sqrt(Random.Range(0.1f, 4f));

            instance.transform.localScale *= sizeFactor;
            var meteorScript = instance.GetComponent<MeteorScript>();
        
            meteorScript.velocity = new Vector3(
                                        -2 + Random.Range(-0.1f, 0.1f), 
                                        -2 + Random.Range(-0.1f, 0.1f),
                                        2 + Random.Range(-0.1f, 0.1f)
                                    ) / sizeFactor;
            meteorScript.angularVelocity = new Vector3(
                                               2 * Random.Range(0,2) * 2 - 1 + Random.Range(-1f, 1f),
                                               2 * Random.Range(0, 2) * 2 - 1 + Random.Range(-1f, 1f),
                                               2 * Random.Range(0, 2) * 2 - 1 + Random.Range(-1f, 1f)
                                           ) / sizeFactor;
        }

        // Update is called once per frame
        private void Update()
        {
            if (timeManager.running && 40 > Random.Range(0, 100))
            {
                GenerateMeteor();
            }
        }
    }
}
