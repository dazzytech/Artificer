using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

using Data.Shared;
using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;

namespace Construction.ShipEditor
{
    public class ComponentItemPrefab : MonoBehaviour, IPointerDownHandler,
        IPointerEnterHandler, IPointerExitHandler
    {
        public Button Select;
        public RawImage Icon;
        public Text Inventory;
        public GameObject HoverPrefab;
        ComponentListener Con;
        GameObject Tracked;
        EditorListener List;
        GameObject HoverWindow;
        bool Hovering;

        public void CreateItem(GameObject GO, EditorListener list)
        {
            Tracked = GO;

            Hovering = false;

            List = list;
            
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
                Icon.texture = newTex.sprite.texture;
                Icon.transform.localScale = new Vector3(newTex.sprite.texture.width*0.01f, newTex.sprite.texture.height*0.01f);
            }
            else
                Icon.texture = null;
            
            // retrieve attributes
            Con = GO.GetComponent<ComponentListener>();
            ComponentAttributes att = GO.GetComponent<ComponentAttributes>();
        }

        public void OnPointerDown(PointerEventData data)
        {
            /*if(data.button == PointerEventData.InputButton.Left)
                List.CreateItem(Tracked);
            if (Hovering)
            {
                GameObject.Destroy(HoverWindow);
                Hovering = false;
                HintBoxController.Clear();
            }*/
        }

        public void OnPointerEnter(PointerEventData data)
        {
            if (!Hovering && !Input.GetMouseButtonDown(0))

            {
                HintBoxController.Display("Left Click and drag the object to the " +
                     "editor panel to create an instance of the object.");

                // Create a hover window with Component data
                HoverWindow = Instantiate(HoverPrefab);
                HoverWindow.transform.SetParent(GameObject.Find("_root").transform);

                HoverWindow.GetComponent<ComponentHoverController>().DisplayComp(Con, Icon.texture);

                Hovering = true;
            }
        }

        public void OnPointerExit(PointerEventData data)
        {
            if (Hovering)
            {
                HintBoxController.Clear();
                GameObject.Destroy(HoverWindow);
                Hovering = false;
            }
        }

        void Update()
        {
            if (Hovering && HoverWindow != null)
            {
                HoverWindow.transform.position = Input.mousePosition
                    + new Vector3(-HoverWindow.GetComponent<RectTransform>().rect.width*.5f-10,
                                  HoverWindow.GetComponent<RectTransform>().rect.height*.5f+10, 1);

                // Keep edges within rect of parent
                Vector2 newPos = Vector3.zero;
                float thisLeft = HoverWindow.transform.position.x;

                float parentLeft = GameObject.Find("_root").transform.position.x;
                
                float thisRight = HoverWindow.transform.position.x + 
                    (HoverWindow.GetComponent<RectTransform>().rect.width*.5f);

                float parentRight = GameObject.Find("_root").transform.GetComponent<RectTransform>().rect.width;
                
                float thisBottom = HoverWindow.GetComponent<RectTransform>().rect.height;

                float parentBottom = GameObject.Find("_root").transform.position.y - 
                    (GameObject.Find("_root").transform.GetComponent<RectTransform>().rect.height*.5f);
                
                float thisTop = transform.position.y + 40; // upper buffer zone

                float parentTop = GameObject.Find("_root").transform.position.y;
                
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
                
                HoverWindow.transform.Translate(newPos);
            }
        }

    }
}