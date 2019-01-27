using UnityEngine;
using UnityEngine.UI;

namespace Manager {
    public class Icon : MonoBehaviour {
        private Image image;
        private Image Image => image ?? (image = transform.Find("Mask").Find("Image").GetComponent<Image>());
        private Text textField;
        private Text TextField => textField ?? (textField = transform.Find("Text").GetComponent<Text>());

        public Sprite Sprite;
        public string Text;
        public Color Color = Color.white;
        
        private void Start() {
            Image.sprite = Sprite;
            Image.color = Color;
            TextField.text = Text;
            TextField.color = Color;
        }

        public void ChangeColor(Color color) {
            Color = color;
            Image.color = color;
            TextField.color = color;
        }
    }
}
