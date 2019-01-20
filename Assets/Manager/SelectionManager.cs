using System;
using Entities;
using UnityEngine;

namespace Manager {
    public class SelectionManager : MonoBehaviour {
        private int index;
        public MovingEntity[] Entities;

        public MovingEntity CurrentTarget => Entities[index];

        private Pathfinder pathfinder => GetComponent<Pathfinder>();
        private CanvasManager canvasManager => GetComponent<CanvasManager>();

        private void Update() {
            for (var i = 0; i < Entities.Length; ++i) {
                if (Input.GetKeyDown((i + 1).ToString())) {
                    canvasManager.SelectionChanged(index, i);
                    index = i;
                    pathfinder.RepaintHexColors();
                }
            }

            if (Input.GetKeyDown(KeyCode.S)) {
                CurrentTarget.Interrupt();
            }

            if (Input.GetKeyDown(KeyCode.Q)) {
                CurrentTarget.CancelLastAction();
            }
        }
    }
}
