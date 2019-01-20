using System;
using Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Manager {
    public class CanvasManager : MonoBehaviour {
        private static Color BarColorNormal = new Color(0.08f, 0.44f, 0.45f);
        private static Color BarColorDanger = new Color(0.45f, 0.08f, 0.33f);

        private Text energyBarText;
        public Text EnergyBarText => energyBarText ?? (energyBarText = GameObject.Find("EnergyBarText").GetComponent<Text>());

        private Image energyBarFill;
        public Image EnergyBarFill => energyBarFill ?? (energyBarFill = GameObject.Find("EnergyBarFill").GetComponent<Image>());

        private Image energyBarBackground;
        public Image EnergyBarBackground => energyBarBackground ?? (energyBarBackground = GameObject.Find("EnergyBarBackground").GetComponent<Image>());

        private SelectionManager selectionManager;
        public SelectionManager SelectionManager => selectionManager ?? (selectionManager = GetComponent<SelectionManager>());

        public MovingEntity CurrentTarget => SelectionManager.CurrentTarget;

        private void Update() {
            EnergyBarText.text = $"{Format(CurrentTarget.Energy)} / {Format(CurrentTarget.MaxEnergy)} ({CurrentTarget.EnergyPerSecond}/s)";
            var width = EnergyBarBackground.rectTransform.rect.size.x;
            var energyPercentage = CurrentTarget.Energy / CurrentTarget.MaxEnergy;
            EnergyBarFill.rectTransform.offsetMin = new Vector2((width - width * energyPercentage) / 2, 0);
            EnergyBarFill.rectTransform.offsetMax = new Vector2((-width + width * energyPercentage) / 2, 0);

            if (energyPercentage < 0.25) {
                EnergyBarFill.color = BarColorDanger;
            } else {
                EnergyBarFill.color = BarColorNormal;
            }
        }

        private string Format(float number) => $"{number:n0}";
    }
}
