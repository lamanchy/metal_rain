using Entities;
using UnityEngine;

namespace Manager {
    public class CameraManager : MonoBehaviour {
        private MovingEntity target => gameObject.GetComponent<SelectionManager>().CurrentTarget;
        private new Camera camera => Camera.main;

        public Vector3 offset;

        private void Update() {
            camera.transform.position = target.transform.position + offset;
        }
    }
}
