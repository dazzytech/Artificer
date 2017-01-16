using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Space.UI.Station.Viewer
{
    public class ViewerItem : MonoBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private Image Icon;

        private Color StandardColor = Color.white;

        [SerializeField]
        private Color HighlightColor;

        [SerializeField]
        private Color SelectedColor;

        private int ID;

        #endregion

        public void Define(GameObject Obj)
        {
            // extract the sprite from the components 
            // game object
            Sprite Img = Obj.gameObject.
                GetComponentInChildren<SpriteRenderer>().sprite;

            // next set ID
            Icon.sprite = Img;
            Icon.rectTransform.sizeDelta = Img.rect.size;
            Icon.rectTransform.localRotation = Obj.transform.localRotation;
        }
    }
}
