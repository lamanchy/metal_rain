using UnityEngine;

namespace Manager {
    public class TimeManager : MonoBehaviour {
        public bool hasChanged;
        public bool running;

        private void Start() {
            running = true;
            hasChanged = false;
        }

        // Update is called once per frame
        private void LateUpdate() {
            if (hasChanged) {
                hasChanged = false;
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                running = !running;
                hasChanged = true;
            }
        }
    }
}
