using Entities;
using UnityEngine;

namespace Manager {
    public class SelectionManager : MonoBehaviour {
        private int index;
        public MovingEntity[] Entities;

        public MovingEntity CurrentTarget => Entities[index];

        private Pathfinder pathfinder => gameObject.GetComponent<Pathfinder>();

        private void Update() {
            for (var i = 0; i < Entities.Length; ++i) {
                if (Input.GetKeyDown((i + 1).ToString())) {
                    index = i;
                    pathfinder.RepaintHexColors();
                }
            }
        }
    }
}
