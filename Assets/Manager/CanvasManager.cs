using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Manager {
    public class CanvasManager : MonoBehaviour {
        private static Color BarColorNormal = new Color(0.08f, 0.44f, 0.45f);
        private static Color BarColorDanger = new Color(0.45f, 0.08f, 0.33f);

        private const float queueItemOffset = 75f;

        public GameObject queueItemPrefab;

        private Image energyBar;
        public Image EnergyBar => energyBar ?? (energyBar = GameObject.Find("EnergyBar").GetComponent<Image>());

        private Image energyBarBackground;
        public Image EnergyBarBackground => energyBarBackground ?? (energyBarBackground = GameObject.Find("EnergyBarBackground").GetComponent<Image>());

        private Image energyBarFill;
        public Image EnergyBarFill => energyBarFill ?? (energyBarFill = GameObject.Find("EnergyBarFill").GetComponent<Image>());

        private Text energyBarText;
        public Text EnergyBarText => energyBarText ?? (energyBarText = GameObject.Find("EnergyBarText").GetComponent<Text>());

        private SelectionManager selectionManager;
        public SelectionManager SelectionManager => selectionManager ?? (selectionManager = GetComponent<SelectionManager>());

        public MovingEntity CurrentTarget => SelectionManager.CurrentTarget;

        private List<Image> queueItems = new List<Image>();

        private void Start() {
            SelectionManager.CurrentTarget.OnActionEnqueue += ActionEnqueued;
            SelectionManager.CurrentTarget.OnActionDequeue += ActionDequeued;
        }

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

        public void SelectionChanged(int previous, int next) {
            var previousEntity = SelectionManager.Entities[previous];
            previousEntity.OnActionEnqueue -= ActionEnqueued;
            previousEntity.OnActionDequeue -= ActionDequeued;

            queueItems.ForEach(item => Destroy(item.gameObject));

            var nextEntity = SelectionManager.Entities[next];
            nextEntity.OnActionEnqueue += ActionEnqueued;
            nextEntity.OnActionDequeue += ActionDequeued;

            queueItems = nextEntity.ActionQueue.Aggregate(new List<Image>(), AddAction);
        }

        private static float GetItemOffset(int itemsCount) => queueItemOffset * itemsCount;

        private void ActionEnqueued(IUnitAction action) => AddAction(queueItems, action);

        private void ActionDequeued(IUnitAction action) {
            Destroy(queueItems[0].gameObject);
            queueItems.RemoveAt(0);
            queueItems.ForEach(item => item.rectTransform.Translate(-queueItemOffset, 0 ,0));
        }

        private List<Image> AddAction(List<Image> items, IUnitAction action) {
            var image = Instantiate(queueItemPrefab, EnergyBar.rectTransform).GetComponent<Image>();
            image.rectTransform.Translate(GetItemOffset(items.Count), 0, 0);
            image.color = action.color;
            items.Add(image);
            return items;
        }

        private static string Format(float number) => $"{number:n0}";
    }
}
