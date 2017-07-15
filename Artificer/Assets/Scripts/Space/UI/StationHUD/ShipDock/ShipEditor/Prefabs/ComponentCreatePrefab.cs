using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

using Data.Shared;
using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;

namespace Space.UI.Station.Prefabs
{
    /// <summary>
    /// When clicked will create an instance
    /// of it's corresponding gameobject
    /// </summary>
    public class ComponentCreatePrefab : MonoBehaviour, 
        IPointerDownHandler, IPointerEnterHandler, 
        IPointerExitHandler
    {
        #region ATTRIBUTES

        /// <summary>
        /// Display component
        /// </summary>
        [SerializeField]
        private RawImage m_icon;
        
        /// <summary>
        /// component prefab created when clicked
        /// </summary>
        private GameObject m_componentPrefab;
        
        /// <summary>
        /// The delegate function for 
        /// when the object is clicked
        /// </summary>
        private ShipDockController.Create m_create;

        #region HOVERING 

        /// <summary>
        /// Prefab to be created when 
        /// mouse over
        /// </summary>
        [SerializeField]
        private GameObject m_hoverPrefab;

        /// <summary>
        /// Window instance that appears
        /// when mouse hovers over
        /// </summary>
        private GameObject m_hoverWindow;

        /// <summary>
        /// Whether of not the mouse is over
        /// </summary>
        private bool m_hovering;

        #endregion

        #endregion

        #region MONO BEHAVIOUR

        void Update()
        {
            if (m_hovering && m_hoverWindow != null)
            {
                m_hoverWindow.transform.position = Input.mousePosition
                    + new Vector3(-m_hoverWindow.GetComponent<RectTransform>().rect.width*.5f-10,
                                  m_hoverWindow.GetComponent<RectTransform>().rect.height*.5f+10, 1);

                // Keep edges within rect of parent
                Vector2 newPos = Vector3.zero;
                float thisLeft = m_hoverWindow.transform.position.x;

                float parentLeft = GameObject.Find("_gui").transform.position.x;
                
                float thisRight = m_hoverWindow.transform.position.x + 
                    (m_hoverWindow.GetComponent<RectTransform>().rect.width*.5f);

                float parentRight = GameObject.Find("_gui").transform.GetComponent<RectTransform>().rect.width;
                
                float thisBottom = m_hoverWindow.GetComponent<RectTransform>().rect.height;

                float parentBottom = GameObject.Find("_gui").transform.position.y - 
                    (GameObject.Find("_gui").transform.GetComponent<RectTransform>().rect.height*.5f);
                
                float thisTop = transform.position.y + 40; // upper buffer zone

                float parentTop = GameObject.Find("_gui").transform.position.y;
                
                if (thisLeft < parentLeft)
                {
                    newPos.x = -(thisLeft - parentLeft);
                }
                
                if (thisRight > parentRight)
                {
                    newPos.x = -(thisRight - parentRight);
                }
                
                if (thisBottom < parentBottom)
                {
                    newPos.y = -(thisBottom - parentBottom);
                }
                
                if (thisTop > parentTop)
                {
                    newPos.y = -(thisTop - parentTop);
                }
                
                m_hoverWindow.transform.Translate(newPos);
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Creates image and assigns delegate
        /// and component prefab
        /// </summary>
        /// <param name="GO"></param>
        /// <param name="create"></param>
        public void CreateItem(GameObject GO, 
            ShipDockController.Create create)
        {
            m_componentPrefab = GO;

            m_hovering = false;

            m_create = create;
            
            // RETRIEVE TEXTURE FOR IMAGE 
            // if component has a renderer then set the sprite as comp texture
            SpriteRenderer newTex = null;
            // retreive texture from the first child object
            foreach(Transform child in GO.transform)
            {
                newTex = child.GetComponent<SpriteRenderer>();
                if(newTex != null)
                    break;
            }
            
            if(newTex != null)
            {
                // set as objects tex
                m_icon.texture = newTex.sprite.texture;

                m_icon.SetNativeSize();

                // Define scale of texture
                // height = 75
                float maxAngle = Mathf.Max(m_icon.texture.width, m_icon.texture.height);

                if (maxAngle > 75)
                {
                    float sizeScale = 75f / maxAngle;

                    m_icon.transform.localScale = new Vector3(sizeScale, sizeScale, 1);
                }
            }
            else
                m_icon.texture = null;
        }

        #endregion

        #region POINTER DATA

        public void OnPointerDown(PointerEventData data)
        {
            if(data.button == PointerEventData.InputButton.Left)
                m_create(m_componentPrefab);
            if (m_hovering)
            {
                GameObject.Destroy(m_hoverWindow);
                m_hovering = false;
                HintBoxController.Clear();
            }
        }

        public void OnPointerEnter(PointerEventData data)
        {
            if (!m_hovering && !Input.GetMouseButton(0))

            {
                HintBoxController.Display("Left Click and drag the object to the " +
                     "editor panel to create an instance of the object.");

                // Create a hover window with Component data
                m_hoverWindow = Instantiate(m_hoverPrefab);
                m_hoverWindow.transform.SetParent(GameObject.Find("_gui").transform);
                m_hoverWindow.transform.localPosition = Vector3.zero;

                m_hoverWindow.GetComponent<ComponentHoverPrefab>().Display
                    (m_componentPrefab);

                m_hovering = true;
            }
        }

        public void OnPointerExit(PointerEventData data)
        {
            if (m_hovering)
            {
                HintBoxController.Clear();
                GameObject.Destroy(m_hoverWindow);
                m_hovering = false;
            }
        }

        #endregion
    }
}