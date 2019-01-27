using Entities;
using UnityEngine;

namespace Manager {
    public class CameraManager : MonoBehaviour {
        private MovingEntity target => gameObject.GetComponent<SelectionManager>().CurrentTarget;
        private new Camera camera => Camera.main;

        public Vector3 offset;

        private float zoomLevel = 0.5f;
        private float rotation;

        private float forcedDistance;

        private void Start() {
            forcedDistance = 1000;
        }

        private void Update() {
            if (!target) { return; }

            camera.transform.position = target.transform.position + Quaternion.AngleAxis(rotation, Vector3.up) * offset * zoomLevel;
            camera.transform.LookAt(target.transform);

            var distanceDiff = Vector3.Distance(camera.transform.position, target.transform.position) - forcedDistance;
            if (distanceDiff > 0) {
                var distanceMoved = Mathf.Max(9 * distanceDiff / 10, 0.01f);
                camera.transform.position = Vector3.MoveTowards(camera.transform.position, target.transform.position, distanceMoved);
                forcedDistance += distanceDiff - distanceMoved;
            }
            
            if (Input.GetKey(KeyCode.A)) {
                rotation -= 1f;
            }
            if (Input.GetKey(KeyCode.D)) {
                rotation += 1f;
            }

            var scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll < 0f) {
                zoomLevel = Mathf.Min(zoomLevel - scroll, 1.2f);
            }
            if (scroll > 0f) {
                zoomLevel = Mathf.Max(zoomLevel - scroll, 0.2f);
            }
            
            var targetPos = target.transform.position;
            var cameraPos = camera.transform.position;
            if (Physics.Raycast(targetPos, cameraPos - targetPos, out var hit, Vector3.Distance(targetPos, cameraPos), ~LayerMask.NameToLayer("Tiles"))) {
                camera.transform.position = hit.point;
                forcedDistance = Vector3.Distance(camera.transform.position, targetPos);
            }
        }
    }
}
