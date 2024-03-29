using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Utils.UI {
    public static class ScrollView {
        private const int NEW_ITEM_GAME_OBJECT_WIDTH = 800;
        private const int NEW_ITEM_GAME_OBJECT_HEIGHT = 90;
        private const int NEW_ITEM_GAME_OBJECT_FONT_SIZE = 30;

        public static GameObject CreateNewTextItemForScrollView(object item, int index) {
            var newGameObject = new GameObject();
            var myText = newGameObject.AddComponent<Text>();
            myText.text = $"{index}. {item}";
            myText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            myText.fontSize = NEW_ITEM_GAME_OBJECT_FONT_SIZE;
            myText.color = Color.black;
            newGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(NEW_ITEM_GAME_OBJECT_WIDTH,
                NEW_ITEM_GAME_OBJECT_HEIGHT);
            return newGameObject;
        }

        public static void FillScrollViewWithObjects(IEnumerable<object> objects, Transform transform) {
            var index = 1;
            foreach (var ob in objects) {
                var newGameObject = CreateNewTextItemForScrollView(ob,
                    index);
                newGameObject.transform.SetParent(transform);
                index++;
            }
        }
    }
}