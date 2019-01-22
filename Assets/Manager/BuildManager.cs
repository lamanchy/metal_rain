using UnityEngine;

namespace Manager {
    public class BuildManager : MonoBehaviour {
        public GameObject placeholderBuildingPrefab;

        private void Update() {
            if (Input.GetKeyDown(KeyCode.W)) {
                GetComponent<Pathfinder>().OnBuild(placeholderBuildingPrefab);
            }
        }
    }
}
