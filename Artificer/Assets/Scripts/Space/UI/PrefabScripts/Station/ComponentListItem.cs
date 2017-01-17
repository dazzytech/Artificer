using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

//artificer
using Space.UI.Ship;
using Space.Ship.Components.Attributes;

namespace Space.UI.Station
{
    public class ComponentListItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
        IPointerClickHandler
    {
        #region EVENTS

        public static event SelectEvent ItemSelected;
        public static event SelectEvent ItemDeselected;
        public static event SelectEvent ItemHover;
        public static event SelectEvent ItemLeave;

        #endregion

        #region ATTRIBUTES

        private bool Activated;

        private bool Selected;

        public IntegrityTracker Integrity;

        private ComponentAttributes Attributes;

        public int ID { get { return Attributes.ID;} } 

        #region HUD ELEMENTS

        public Text Label;

        public Image SelfPanel;

        #endregion

        #region COLORS

        public Color HighlightColor;

        public Color SelectedColor;

        private Color StandardColor;

        #endregion

        #endregion

        #region MONOBEHAVIOUR

        void OnEnable()
        {
            if (Activated)
                StartCoroutine("Step");
        }

        void OnDisable()
        {
            StopAllCoroutines();
        }

        #endregion

        #region PUBLIC INTERACTION

        public void DefineComponent(ComponentAttributes attributes)
        {
            Attributes = attributes;

            Label.text = attributes.name;

            StandardColor = SelfPanel.color;

            Activated = true;

            StartCoroutine("Step");
        }

        public void Reset(bool Deselect)
        {
            SelfPanel.color = StandardColor;

            if (Deselect)
                Selected = false;
        }

        public void Highlight()
        {
            SelfPanel.color = HighlightColor;
        }

        public void Select()
        {
            SelfPanel.color = SelectedColor;

            Selected = true;
        }

        #endregion

        #region COROUTINE

        private IEnumerator Step()
        {
            while (true)
            {
                if (Attributes == null)
                {
                    Destroy(this.gameObject);
                    StopAllCoroutines();
                    break;
                }

                // Pass info to integrity tracker 
                Integrity.Step(Attributes.NormalizedHealth);

                //finished this step
                yield return null;
            }

            yield return null;
        }

        #endregion

        #region IPOINTEREVENTS

        public void OnPointerEnter(PointerEventData eventData)
        {
            //if (!Selected)
            // SelfPanel.color = HighlightColor;
            ItemHover(ID);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //if (!Selected)
            //  SelfPanel.color = StandardColor;
            //ItemLeave(ID);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Selected)
                ItemDeselected(Attributes.ID);
            else
                ItemSelected(Attributes.ID);
        }

        #endregion
    }
}