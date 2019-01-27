using UnityEngine;
using UnityEngine.UI;

namespace Manager {
    public class HealthBar : MonoBehaviour {
        private Image background;
        private Image fill;
        private Text text;
        
        private void Start() {
            background = transform.Find("BarBackground").GetComponent<Image>();
            fill = background.transform.Find("BarFill").GetComponent<Image>();
            text = transform.Find("BarText").GetComponent<Text>();
        }
        
        public void UpdateBar(string text, float percentage, Color color) {
            this.text.text = text;
            var width = background.rectTransform.rect.size.x;
            fill.rectTransform.offsetMin = new Vector2((width - width * percentage) / 2, 0);
            fill.rectTransform.offsetMax = new Vector2((-width + width * percentage) / 2, 0);
            fill.color = color;
        }
    }
}
