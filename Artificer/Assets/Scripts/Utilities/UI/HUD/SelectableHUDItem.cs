using System.Collections;
using System.Collections.Generic;
using UI.Effects;
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
        #region EVENTS

        // delegate to be called when item is clicked
        private SelectTrigger m_trigger;

        private SelectTrigger m_hover;

        private SelectTrigger m_leave;

        #endregion

        #region ATTRIBUTES

        #region BEHAVIOUR

        // whether or not the item is selected
        private bool m_selected;

        /// <summary>
        /// Decides whether or not the 
        /// background responds to mouse interaction
        /// </summary>
        [SerializeField]
        protected bool m_interactive = true;

        /// <summary>
        /// If list interacts with others
        /// this will be the key that binds
        /// the item in the other list
        /// </summary>
        public int SharedIndex;

        #endregion

        #region HUD ELEMENTS

        [Header("HUD Elements")]
        [SerializeField]
        protected RawImage m_background;

        #endregion

        #region COLOURS

        [Header("Colours")]
        [SerializeField]
        protected Color m_standardColour = Color.white;

        [SerializeField]
        protected Color m_highlightColour = Color.white;

        [SerializeField]
        protected Color m_selectedColour = Color.white;

        #endregion

        #endregion

        #region ACCESSORS

        /// <summary>
        /// Returns if the UI item
        /// is visible to the player
        /// </summary>
        public bool Visible
        {
            get { return m_background.color.a > 0.01; }
        }

        #endregion

        #region PUBLIC INTERACTION

        public virtual void Initialize(SelectTrigger trigger, 
            SelectTrigger hover = null, SelectTrigger leave = null)
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

            m_hover = hover;

            m_leave = leave;
        }

        public void SetColour(Color newColour)
        {
            m_standardColour = newColour;
            m_highlightColour = m_standardColour + new Color(.2f, -0.12f, 1f, 0.5f);
            m_selectedColour = m_standardColour + new Color(.4f, -.1f, .1f);
            m_background.color = m_standardColour;
        }

        /// <summary>
        /// Causes the item to revert
        /// to normal appearance
        /// </summary>
        public virtual void Deselect()
        {
            if (!m_interactive)
                return;

            m_selected = false;
            if (m_background != null) m_background.color = m_standardColour;
        }

        /// <summary>
        /// Highlights the item as selected
        /// </summary>
        public virtual void Select()
        {
            if (!m_interactive)
                return;

            m_selected = true;
            if (m_background != null)
                m_background.color = m_selectedColour;
        }

        public virtual void Highlight(bool highlighted)
        {
            if (!m_interactive)
                return;

            if (highlighted)
            {
                if (!m_selected) if (m_background != null)
                    {
                        m_background.color = m_highlightColour;
                    }
            }
            else
            {
                if (!m_selected) if (m_background != null)
                        m_background.color = m_standardColour;               
            }
        }

        /// <summary>
        /// gives the appearance of an image
        /// flashing into existance (HUD item or panel)
        /// </summary>
        /// <param name="item"></param>
        public void FlashImage()
        {
            StartCoroutine(PanelFadeEffects.FadeImg(m_background,
                m_highlightColour,
                PanelFadeEffects.FadeImg(m_background, m_standardColour)));
        }

        public void FadeImage()
        {
            if (isActiveAndEnabled)
                StartCoroutine(PanelFadeEffects.FadeImg(m_background,
                    new Color(0, 0, 0, 0)));
            else
                m_background.color = new Color(0, 0, 0, 0);
        }

        #endregion

        #region PRIVATE FUNCTIONALITY

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
            Highlight(true);
            if (m_hover != null)
                m_hover(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Highlight(false);
            if (m_leave != null)
                m_leave(this);
        }

        #endregion
    }
}