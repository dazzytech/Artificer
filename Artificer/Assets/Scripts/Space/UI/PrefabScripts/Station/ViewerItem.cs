using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using Space.UI.Station;

namespace Space.UI.Station.Viewer
{
    public class ViewerItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
        IPointerClickHandler
    {
        #region EVENTS

        public static event SelectEvent ItemSelected;
        public static event SelectEvent ItemDeselected;
        public static event SelectEvent ItemHover;
        public static event SelectEvent ItemLeave;

        #endregion

        #region ATTRIBUTES

        #region HUD ELEMENTS

        // component image
        [SerializeField]
        private Image Icon;

        #endregion

        public int ID;

        private bool Selected;

        public static StationController Controller;

        #region COLOR

        private Color StandardColor = Color.white;

        [SerializeField]
        private Color HighlightColor;

        [SerializeField]
        private Color SelectedColor;

        #endregion

        #endregion

        public void Define(GameObject Obj, int id)
        {
            // extract the sprite from the components 
            // game object
            Sprite Img = Obj.gameObject.
                GetComponentInChildren<SpriteRenderer>().sprite;

            // next set ID
            Icon.sprite = Img;
            Icon.rectTransform.sizeDelta = Img.rect.size;
            Icon.rectTransform.localRotation = Obj.transform.localRotation;

            ID = id;
        }

        public void Reset(bool Deselect)
        {
            Icon.color = StandardColor;

            if (Deselect)
                Selected = false;
        }

        public void Highlight()
        {
            Icon.color = HighlightColor;
        }

        public void Select()
        {
            Icon.color = SelectedColor;

            Selected = true;
        }

        #region IPOINTEREVENTS

        public void OnPointerEnter(PointerEventData eventData)
        {
            //if (!Selected)
                //Icon.color = HighlightColor;
                ItemHover(ID);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
           // if (!Selected)
                //Icon.color = StandardColor;
                ItemLeave(ID);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Selected)
                ItemDeselected(ID);
            else
                ItemSelected(ID);
        }

        #endregion
    }
}
