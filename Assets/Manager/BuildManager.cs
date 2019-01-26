using UnityEngine;

namespace Manager {
    public class BuildManager : MonoBehaviour {
        public GameObject wPrefab;
        public GameObject ePrefab;

        private void Update() {
            if (Input.GetKeyDown(KeyCode.W)) {
                GetComponent<Pathfinder>().OnBuild(wPrefab);
            }
            if (Input.GetKeyDown(KeyCode.E)) {
                GetComponent<Pathfinder>().OnBuild(ePrefab);
            }
        }
    }
}
