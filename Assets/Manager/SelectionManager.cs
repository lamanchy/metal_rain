using System.Collections.Generic;
using Entities;
using UnityEngine;

namespace Manager {
    public class SelectionManager : MonoBehaviour {
        public List<MovingEntity> Entities;
        public Icon[] Icons;

        private int index;

        public MovingEntity CurrentTarget => Entities[index];
        public Mothership Mothership => Entities[0] as Mothership;

        private Pathfinder pathfinder => GetComponent<Pathfinder>();
        private CanvasManager canvasManager => GetComponent<CanvasManager>();

        private void Start() {
            Icons[index].ChangeColor(HexColors.SelectedUnit);
            MovingEntity.OnMovingEntityDestroyed += OnMovingEntityDestroyed;
        }

        private void OnDestroy() {
            MovingEntity.OnMovingEntityDestroyed -= OnMovingEntityDestroyed;
        }

        private void Update() {
            for (var i = 0; i < Entities.Count; ++i) {
                CheckSelection(i);
            }

            if (Input.GetKeyDown(KeyCode.S)) {
                CurrentTarget.Interrupt();
            }

            if (Input.GetKeyDown(KeyCode.Q)) {
                CurrentTarget.CancelLastAction();
            }
        }

        private void CheckSelection(int i) {
            var digit = (i + 1).ToString();
            if (!Input.GetKeyDown(digit) || Entities[i] == null) {
                return;
            }
            canvasManager.SelectionChanged(index, i);
            Icons[index].ChangeColor(Entities[index] != null ? Color.white : HexColors.DeadUnit);
            index = i;
            Icons[index].ChangeColor(HexColors.SelectedUnit);
            pathfinder.RepaintHexColors();
        }

        private void OnMovingEntityDestroyed(MovingEntity entity) {
            var i = Entities.IndexOf(entity);
            if (i > 0) {
                return;
            }
            if (i == index) {
                index = 0;
            }
            Icons[i].ChangeColor(HexColors.DeadUnit);

        }
    }
}
