using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using Space.Ship.Components.Listener;
using Space.UI.Station.Viewer;

namespace UI
{
    public class ViewerItem : MonoBehaviour, 
        IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        #region EVENTS

        public static event SelectEvent ItemSelected;
        public static event SelectEvent ItemDeselected;
        public static event SelectEvent ItemHover;
        public static event SelectEvent ItemLeave;

        #endregion

        #region ATTRIBUTES

        [HideInInspector]
        public int ID;

        protected ComponentListener Listener;

        private bool Selected;

        private bool Highlighted;

        [SerializeField]
        private bool m_displayHealth;

        [SerializeField]
        private bool m_interactive;

        #region HUD ELEMENTS

        [Header("HUD Elements")]

        // component image
        protected Image m_icon;

        #endregion

        #region COLOR

        [Header("Colour")]

        [SerializeField]
        private Color HighHealth;
        [SerializeField]
        private Color MedHealth;
        [SerializeField]
        private Color LowHealth;

        private Color m_standardColour;

        private Color m_currentColour;

        #endregion

        #endregion

        #region MONOBEHAVIOUR

        void Awake()
        {
            m_icon = GetComponent<Image>();
            m_standardColour = m_icon.color;
        }

        void OnDestroy()
        {
            StopAllCoroutines();
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Assigns the component to the object
        /// and initializes
        /// </summary>
        /// <param name="Obj"></param>
        /// <param name="id"></param>
        public virtual void Define(GameObject Obj, int id)
        {
            // extract the sprite from the components 
            // game object
            Listener = Obj.GetComponent<ComponentListener>();

            Sprite Img = Listener.Icon;

            // next set ID
            m_icon.sprite = Img;
            m_icon.rectTransform.sizeDelta = Img.rect.size;
            m_icon.rectTransform.localRotation = Obj.transform.localRotation;

            // Start coroutine that updates health
            StartCoroutine("Step");

            ID = id;
        }

        public void SetColour(Color newColour)
        {
            m_standardColour = newColour;
            m_icon.color = m_standardColour;
        }

        public void Reset(bool Deselect)
        {
            m_icon.color = m_standardColour;

            if (Deselect)
                Selected = false;

            Highlighted = false;
        }

        public void Highlight()
        {
            Highlighted = true;
        }

        public void Select()
        {
            Selected = true;
            Highlighted = false;
        }

        #endregion

        #region COROUTINE

        private IEnumerator Step()
        {
            while (true)
            {
                // Detect if component is destroyed
                // Destroy for now but implement
                // destroyed colour
                if (Listener == null)
                {
                    Destroy(gameObject);
                    yield break;
                }

                if (m_displayHealth)
                {
                    if (Listener.NormalizedHealth < 0.3)
                        m_currentColour = LowHealth;
                    else if
                        (Listener.NormalizedHealth < 0.6)
                        m_currentColour = MedHealth;
                    else
                        m_currentColour = HighHealth;

                }
                else
                {
                    m_currentColour = m_standardColour;
                }

                if (m_interactive)
                {
                    if (Highlighted)
                        m_currentColour += new Color(.2f, -0.12f, 1f, 0.5f);
                    else if (Selected)
                        m_currentColour += new Color(.4f, -.1f, .1f);
                }

                if (m_icon.color != m_currentColour)
                    m_icon.color = m_currentColour;
                

                yield return null;
            }
        }

        #endregion

        #region IPOINTEREVENTS

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (ItemHover != null)
                ItemHover(ID);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (ItemLeave != null)
                ItemLeave(ID);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Selected)
            {
                if (ItemDeselected != null)
                    ItemDeselected(ID);
            }
            else
                if (ItemSelected != null)
                ItemSelected(ID);
        }

        #endregion
    }
}
