using Entities;
using UnityEngine;

namespace Manager {
    public class CameraManager : MonoBehaviour {
        private MovingEntity target => gameObject.GetComponent<SelectionManager>().CurrentTarget;
        private new Camera camera => Camera.main;

        public Vector3 offset;

        private float distance = 0.5f;
        private float rotation;

        private void Update() {
            camera.transform.position = target.transform.position + Quaternion.AngleAxis(rotation, Vector3.up) * offset * distance;
            camera.transform.LookAt(target.transform);
            
            if (Input.GetKey(KeyCode.A)) {
                rotation -= 1f;
            }
            if (Input.GetKey(KeyCode.D)) {
                rotation += 1f;
            }

            var scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll < 0f) {
                distance = Mathf.Min(distance - scroll, 1.2f);
            }
            if (scroll > 0f) {
                distance = Mathf.Max(distance - scroll, 0.2f);
            }
        }
    }
}
