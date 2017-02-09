using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using Space.UI.Station;
using Space.Ship.Components.Listener;

namespace UI
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

        [Header("HUD Elements")]

        // component image
        [SerializeField]
        private Image Icon;

        #endregion

        public int ID;

        private bool Selected;

        private ComponentListener Listener; 

        #region COLOR

        [Header("Colour")]

        private Color StandardColor;

        [SerializeField]
        private Color HighHealth;
        [SerializeField]
        private Color MedHealth;
        [SerializeField]
        private Color LowHealth;

        [SerializeField]
        private Color HighlightColor;

        [SerializeField]
        private Color SelectedColor;

        #endregion

        #endregion

        #region MONOBEHAVIOUR

        void OnDestroy()
        {
            StopAllCoroutines();
        }

        #endregion

        public void Define(GameObject Obj, int id)
        {
            // extract the sprite from the components 
            // game object
            Listener = Obj.GetComponent<ComponentListener>();

            Sprite Img = Listener.Icon;

            // next set ID
            Icon.sprite = Img;
            Icon.rectTransform.sizeDelta = Img.rect.size;
            Icon.rectTransform.localRotation = Obj.transform.localRotation;
            StandardColor = HighHealth;


            //transform.localPosition = Obj.GetComponent<ComponentListener>().Postion * 100;

            ID = id;

            // Start coroutine that updates health
            StartCoroutine("Step");
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

        #region COROUTINE

        private IEnumerator Step()
        {
            while(true)
            {
                // Detect if component is destroyed
                // Destroy for now but implement
                // destroyed colour
                if(Listener == null)
                {
                    Destroy(gameObject);
                    yield break;
                }

                if (Listener.NormalizedHealth < 0.3)
                    StandardColor = LowHealth;
                else if
                    (Listener.NormalizedHealth < 0.6)
                    StandardColor = MedHealth;
                else
                    StandardColor = HighHealth;

                if(Icon.color != StandardColor)
                    Icon.color = StandardColor;

                yield return null;
            }
        }

        #endregion

        // Create another prefab that extends this prefab 
        // but has pointerEventHandlers?
        #region IPOINTEREVENTS

        public void OnPointerEnter(PointerEventData eventData)
        {
            ItemHover(ID);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
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
