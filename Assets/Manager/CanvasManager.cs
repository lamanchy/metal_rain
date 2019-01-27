using System.Collections.Generic;
using System.Linq;
using Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Manager {
    public class CanvasManager : MonoBehaviour {
        private const float queueItemOffset = 75f;

        public GameObject queueItemPrefab;

        private HealthBar energyBar;
        private HealthBar thawBar;

        private SelectionManager selectionManager;

        private MovingEntity CurrentTarget => selectionManager.CurrentTarget;
        private Mothership Mothership => selectionManager.Mothership;

        private List<Image> queueItems = new List<Image>();

        private void Start() {
            energyBar = GameObject.Find("EnergyBar").GetComponent<HealthBar>();
            thawBar = GameObject.Find("ThawBar").GetComponent<HealthBar>();

            selectionManager = GetComponent<SelectionManager>();

            CurrentTarget.OnActionEnqueue += ActionEnqueued;
            CurrentTarget.OnActionDequeue += ActionDequeued;
        }

        private void Update() {
            energyBar.UpdateBar(
                $"{Format(CurrentTarget.Energy)} / {Format(CurrentTarget.MaxEnergy)} ({CurrentTarget.EnergyPerSecond}/s)",
                CurrentTarget.Energy / CurrentTarget.MaxEnergy,
                HexColors.EnergyColor(CurrentTarget.Energy)
            );

            if (Mothership.IsThawing) {
                thawBar.gameObject.SetActive(true);
                thawBar.UpdateBar(
                    $"{Mothership.FreezeValue / Mothership.ThawPerSecond:n0} seconds until death",
                    Mothership.FreezeValue / Mothership.MaxFreeze,
                    Color.red
                );
            } else {
                thawBar.gameObject.SetActive(false);
            }
        }

        public void SelectionChanged(int previous, int next) {
            var previousEntity = selectionManager.Entities[previous];
            previousEntity.OnActionEnqueue -= ActionEnqueued;
            previousEntity.OnActionDequeue -= ActionDequeued;

            queueItems.ForEach(item => Destroy(item.gameObject));

            var nextEntity = selectionManager.Entities[next];
            nextEntity.OnActionEnqueue += ActionEnqueued;
            nextEntity.OnActionDequeue += ActionDequeued;

            queueItems = nextEntity.ActionQueue.Aggregate(new List<Image>(), AddAction);
        }

        private static float GetItemOffset(int itemsCount) => queueItemOffset * itemsCount;

        private void ActionEnqueued(IUnitAction action) => AddAction(queueItems, action);

        private void ActionDequeued(int index) {
            for (var i = index + 1; i < queueItems.Count; ++i) {
                queueItems[i].rectTransform.Translate(-queueItemOffset, 0, 0);
            }
            Destroy(queueItems[index].gameObject);
            queueItems.RemoveAt(index);
        }

        private List<Image> AddAction(List<Image> items, IUnitAction action) {
            var image = Instantiate(queueItemPrefab, energyBar.GetComponent<Image>().rectTransform).GetComponent<Image>();
            image.rectTransform.Translate(GetItemOffset(items.Count), 0, 0);
            image.color = action.color;
            items.Add(image);
            return items;
        }

        private static string Format(float number) => $"{number:n0}";
    }
}
