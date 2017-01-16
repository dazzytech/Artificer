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

        public delegate void SelectEvent(int ID);

        public static event SelectEvent ItemSelected;

        public static event SelectEvent ItemDeselected;

        #endregion

        #region ATTRIBUTES

        private bool Activated;

        private bool Selected;

        public IntegrityTracker Integrity;

        private ComponentAttributes Attributes;

        #region HUD ELEMENTS

        public Text Label;

        public Image SelfPanel;

        #endregion

        #region COLORS

        public Color highlightColor;

        public Color selectedColor;

        private Color standardColor;

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

            standardColor = SelfPanel.color;

            Activated = true;

            StartCoroutine("Step");
        }

        public void ResetColor()
        {
            SelfPanel.color = standardColor;
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
            if (!Selected)
                SelfPanel.color = highlightColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!Selected)
                SelfPanel.color = standardColor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Selected)
            {
                SelfPanel.color = standardColor;
                ItemDeselected(Attributes.ID);
            }
            else
            {
                SelfPanel.color = selectedColor;
                ItemSelected(Attributes.ID);
            }

            Selected = !Selected;
        }

        #endregion
    }
}