using UnityEngine;
using UnityEngine.UI;

namespace Manager {
    public class Icon : MonoBehaviour {
        private Image image;
        private Text text;

        public Sprite Sprite;
        public string Text;
        public Color Color = Color.white;

        // Start is called before the first frame update
        private void Start() {
            image = transform.Find("Mask").Find("Image").GetComponent<Image>();
            text = transform.Find("Text").GetComponent<Text>();

            image.sprite = Sprite;
            image.color = Color;
            text.text = Text;
        }
    }
}
