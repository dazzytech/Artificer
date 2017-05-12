using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public delegate void SelectTrigger(SelectableHUDItem triggered);

    /// <summary>
    /// prebuilt component for 
    /// selection items for lists
    /// </summary>
    public class SelectableHUDItem : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        #region ATTRIBUTES

        #region BEHAVIOUR

        // delegate to be called when item is clicked
        private SelectTrigger m_trigger;

        // whether or not the item is selected
        private bool m_selected;

        #endregion

        #region HUD ELEMENTS

        [Header("HUD Elements")]
        [SerializeField]
        private RawImage m_background;

        #endregion

        #region COLOURS

        [Header("Colours")]
        [SerializeField]
        private Color m_standardColour = Color.white;

        [SerializeField]
        private Color m_highlightColour = Color.white;

        [SerializeField]
        private Color m_selectedColour = Color.white;

        #endregion

        #endregion

        #region PUBLIC INTERACTION

        public virtual void Initialize(SelectTrigger trigger)
        {
            // check that background is assigned
            if(m_background == null)
            {
                // we need to find a background image
                m_background = GetComponent<RawImage>();

                // if this didn't work send an error
                if(m_background == null)
                {
                    Debug.Log("Error: SelectableHUDItem(" +
                        transform.name + ") - " + "Initialize: " +
                        "background image could not be assigned!");

                    Destroy(this.gameObject);
                }
            }

            // check if the standard colour is assigned
            if(m_standardColour != Color.white)
                // we have a standard colour selected
                m_background.color = m_standardColour;
            else
                // we assign a standard colour with our background image
                // safest way to assign a default colour
                 m_standardColour = m_background.color;

            m_selected = false;

            // Attach delegate to our trigger
            m_trigger = trigger;
        }

        /// <summary>
        /// Causes the item to revert
        /// to normal appearance
        /// </summary>
        public virtual void Deselect()
        {
            m_selected = false;
            m_background.color = m_standardColour;
        }

        /// <summary>
        /// Highlights the item as selected
        /// </summary>
        public virtual void Select()
        {
            m_selected = true;
            m_background.color = m_selectedColour;
        }


        #endregion

        #region POINTER EVENTS

        public void OnPointerClick(PointerEventData eventData)
        {
            // if not already selected process selection
            if (!m_selected)
            {
                if(m_trigger != null)
                    m_trigger(this);
                else
                    Debug.Log("Error: SelectableHUDItem(" +
                        transform.name + ") - " + "OnPointerCLick: " +
                        "trigger delegate not assigned!");
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!m_selected)
            {
                m_background.color = m_highlightColour;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!m_selected)
            {
                m_background.color = m_standardColour;
            }
        }

        #endregion
    }
}