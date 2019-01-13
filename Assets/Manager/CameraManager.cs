using Entities;
using UnityEngine;

namespace Manager {
    public class CameraManager : MonoBehaviour {
        public BaseEntity target;
        private new Camera camera => Camera.main;

        public Vector3 offset;

        private void Update() {
            if (target == null) {
                return;
            }

            camera.transform.position = target.transform.position + offset;
        }
    }
}
