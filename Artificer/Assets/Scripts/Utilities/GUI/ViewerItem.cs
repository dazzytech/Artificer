using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using Space.UI.Station;
using Space.Ship.Components.Listener;

namespace UI
{
    public class ViewerItem : MonoBehaviour, 
        IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        private enum Type { STATIC, RESPONSIVE }

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

        // does item recolour?
        [SerializeField]
        private Type m_type;

        #region HUD ELEMENTS

        [Header("HUD Elements")]

        // component image
        [SerializeField]
        protected Image Icon;

        #endregion

        #region COLOR

        [Header("Colour")]

        [SerializeField]
        private Color HighHealth;
        [SerializeField]
        private Color MedHealth;
        [SerializeField]
        private Color LowHealth;

        private Color m_standardColor;

        #endregion

        #endregion

        #region MONOBEHAVIOUR

        void Awake()
        {
            m_standardColor = Icon.color;
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
            Icon.sprite = Img;
            Icon.rectTransform.sizeDelta = Img.rect.size;
            Icon.rectTransform.localRotation = Obj.transform.localRotation;

            if (m_type == Type.RESPONSIVE)
            {
                m_standardColor = HighHealth;
                // Start coroutine that updates health
                StartCoroutine("Step");
            }

            ID = id;
        }

        public void SetColour(Color newColour)
        {
            m_standardColor = newColour;
        }

        public void Reset(bool Deselect)
        {
            Icon.color = m_standardColor;

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

                if (Listener.NormalizedHealth < 0.3)
                    m_standardColor = LowHealth;
                else if
                    (Listener.NormalizedHealth < 0.6)
                    m_standardColor = MedHealth;
                else
                    m_standardColor = HighHealth;

                if (Highlighted)
                    m_standardColor += new Color(.2f, -0.12f, 1f, 0.5f);
                else if (Selected)
                    m_standardColor += new Color(.4f, -.1f, .1f);

                if (Icon.color != m_standardColor)
                    Icon.color = m_standardColor;
                

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
