using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
// Artificer
using Data.Shared;
using Space.Ship.Components.Listener;

namespace Construction.ShipEditor
{
    /// <summary>
    /// Ship storage controller.
    /// script that controls and manages all the ship
    /// components 
    /// </summary>
    public class ShipUIUtility
        : MonoBehaviour
    {
        // Ship is stored in a designated container
        private ShipContainer _ship;

        // UI utils
        [HideInInspector]
        public bool IsDragging;
        [HideInInspector]
        public BaseShipComponent DraggedObj;

        public Text WeightText;

        public Transform ChangeTrans;

        public Toggle RotorFollowsMouse;
        public RectTransform RFRect;

        // Right click items
        public GameObject RCPrefab;
        GameObject RCWindow;
        bool RCDelay;

        void Awake()
        {
            IsDragging = false;
        }

        // Reset Data function and clear panel here
        public void SetShip(ShipContainer ship)
        {
            _ship = ship;

            RotorFollowsMouse.isOn = _ship.Ship.CombatResponsive;
        }

        public delegate void DelegateHead(BaseShipComponent newHead);

        public void SetHead(BaseShipComponent newHead)
        {
            _ship.Head = newHead;
        }

        public void UpdateUI()
        {
            HintBoxController.Clear("Use Right-Click to display additional options");

            foreach (BaseShipComponent component in _ship.Components)
            {
                // Update and drag if mouse button is down over object
                // Store rotation and face upwards
                Quaternion orig = component.GetComponent<RectTransform>().rotation;
                component.GetComponent<RectTransform>().rotation = Quaternion.Euler(0,0,0);
                bool MouseOver = InBounds(component.GetComponent<RectTransform>(), Input.mousePosition);
                // Reapply old rotation
                component.GetComponent<RectTransform>().rotation = orig;

                if(RCWindow != null && !RCDelay)
                {
                    if (Input.GetMouseButtonDown(0)|| Input.GetMouseButtonDown(1))
                    {
                        if(!InBounds(RCWindow.GetComponent<RectTransform>(), Input.mousePosition)
                           || Input.GetMouseButtonDown(1))
                        {
                            Destroy(RCWindow);
                            RCDelay = true;
                            Invoke("RCDel", .3f);
                        }
                        else
                        {
                            // player is interacting with the RCWindow so we can exit
                            return;
                        }
                    }
                }

                if (Input.GetMouseButtonDown(0))
                {
                    // Use utility function
                    if(MouseOver)
                       StartDrag(component);
                }
                else if(Input.GetMouseButtonDown(1))
                {
                    // Player has clicked the right mouse button
                    if(RCWindow == null && MouseOver)
                    {
                        // Create the Right Click Window
                        RCWindow = Instantiate(RCPrefab);
                        RCWindow.transform.SetParent(GameObject.Find("_root").transform);

                        RCWindow.transform.position = Input.mousePosition
                            + new Vector3(-RCWindow.GetComponent<RectTransform>().rect.width*.5f-10,
                                          RCWindow.GetComponent<RectTransform>().rect.height*.5f+10, 1);

                        // Create delegate function

                        RCWindow.GetComponent<RightClickComponentMenu>().DisplayComp(component, _ship.Head, new DelegateHead(SetHead));

                        RCDelay = true;
                        Invoke("RCDel", .3f);
                    }
                }
                else if(Input.GetMouseButtonUp(0))
                {
                    ReleaseDrag();
                }
                else
                {
                    if(MouseOver && RCWindow == null)
                        HintBoxController.Display("Use Right-Click to display additional options");
                }
            }

            if (IsDragging)
            {
                UpdateComponent(DraggedObj);
            }
            if (DraggedObj != null)
            {
                HintBoxController.Display("When dragging an object - use the directional keys " +
                    "to change the direction the object is facing");
                if (Input.GetKeyUp(KeyCode.UpArrow))
                    DraggedObj.ProcessDirection("up");
                if (Input.GetKeyUp(KeyCode.DownArrow))
                    DraggedObj.ProcessDirection("down");
                if (Input.GetKeyUp(KeyCode.LeftArrow))
                    DraggedObj.ProcessDirection("left");
                if (Input.GetKeyUp(KeyCode.RightArrow))
                    DraggedObj.ProcessDirection("right");
            } else
            {
                HintBoxController.Clear("When dragging an object - use the directional keys " +
                    "to change the direction the object is facing");
            }

            // if mouse over combat style panel will display message
            if (InBounds(RFRect, Input.mousePosition))
            {
                HintBoxController.Display("When on, the ship will turn to face the mouse in combat mode." +
                                          "When off, the ship will be independent of the mouse, use in " +
                                          "conjuction with mouse follow targeters as" +
                                          "weapons still fire with combat controls.");
            } else
            {
                HintBoxController.Clear("When on, the ship will turn to face the mouse in combat mode." +
                                        "When off, the ship will be independent of the mouse, use in " +
                                        "conjuction with mouse follow targeters as " +
                                        "weapons still fire with combat controls.");
            }
        }

        public void RCDel()
        {
            RCDelay = false;
        }

        public void StartDrag(BaseShipComponent comp)
        {
            IsDragging = true;
            DraggedObj = comp;
        }

        /// <summary>
        /// Updates the component.
        /// Detects if the component is operlapping any other component
        /// </summary>
        /// <param name="comp">Comp.</param>
        public void UpdateComponent(BaseShipComponent comp)
        {
            // Error Checking
            if(_ship.Components == null)
            {
                Debug.Log("Ship Editor - " +
                          "UpdateComponent: component list is null ");
                return;
            }
            
            // Drag the component centered under mouse
            IsDragging = true;
            comp.transform.position = Input.mousePosition;
            
            // Grabs all other components for testing overlapping and connecting
            foreach (BaseShipComponent other in _ship.Components)
            {
                // First detect collision with head
                if (!comp.Equals(other))
                {
                    if (comp.GetComponent<RectTransform>().rect.Overlaps
                        (other.GetComponent<RectTransform>().rect))
                    {
                        comp.AddCollision(other);
                    }
                }
            }

            if (comp.Pending)
            {
                // Turn the RawImage to a faded red
                RawImage compImg = DraggedObj.GetComponent<RawImage>();
                compImg.color = new Color(.2f, 1, .2f, .4f);
            }
            else{
            // Turn the RawImage to a faded red
            RawImage compImg = DraggedObj.GetComponent<RawImage>();
            compImg.color = new Color(1, .2f, .2f, .4f);
            }
        }
        
        // Release Component
        /// <summary>
        /// Releases the component.
        /// </summary>
        /// <param name="comp">Comp.</param>
        public void ReleaseDrag()
        {
            IsDragging = false;

            if (DraggedObj != null)
            {
                // Turn the RawImage to a faded red
                RawImage compImg = DraggedObj.GetComponent<RawImage>();
                if (compImg != null)
                    compImg.color = new Color(1, 1, 1, 1);
            }
        }

        public int ReturnNextAvailableInstance()
        {
            int ID = 0;
            // Sort IDs numerically
            _ship.AddedIDs.Sort();
            // compare ID to each ID end result showed be the next available ID
            foreach (int otherID in _ship.AddedIDs)
            {
                if(otherID == ID)
                    ID = otherID+1;
            }

            return ID;
        }

        public string ReturnNextAvailableName()
        {
            string ShipName;

            List<int> UnAvailable = new List<int>(); 
            // Create a list of numbers of each untitled ship
            /*foreach (ShipData ship in GameManager.GetPlayer.ShipList)
            {
                if(ship.Name.Contains("Untitled_"))
                {
                    UnAvailable.Add(int.Parse(Regex.Match(ship.Name, @"\d+").Value));
                }
            }*/

            int ID = 0;         // Added next to untitled
            UnAvailable.Sort();
            // compare ID to each ID end result showed be the next available ID
            foreach (int otherID in UnAvailable)
            {
                if(otherID == ID)
                    ID = otherID+1;
            }

            ShipName = string.Format("Untitled_{0}", ID);

            return ShipName;
        }

        public bool InBounds(RectTransform r, Vector3 pos)
        {
            Vector3[] worldCorners = new Vector3[4];
            r.GetWorldCorners(worldCorners);

            if (pos.x >= worldCorners [0].x && pos.x < worldCorners [2].x 
                && pos.y >= worldCorners [0].y && pos.y < worldCorners [2].y)
            {
                return true;
            }

            return false;
        }

        public void UpdateWeight()
        {
            float wgt = 0;
            foreach (BaseShipComponent bSC in _ship.Components)
            {
                wgt += bSC.GO.GetComponent<ComponentListener>().Weight;
            }

            if((wgt * 1000)>= 1000)
                WeightText.text = "Ship Weight: " + wgt.ToString("F1")+"Ton(m)";
            else
                WeightText.text = "Ship Weight: " + (wgt * 1000).ToString("F0")+"KG";
        }

        public void ClearWeight()
        {
            WeightText.text = "";
        }

        public void SetRotorFollow()
        {
            _ship.Ship.CombatResponsive = RotorFollowsMouse.isOn;
        } 

        public void UpdateSavedMsg(bool saved)
        {
            if(saved)
            {
                if(!ChangeTrans.gameObject.activeSelf)
                    ChangeTrans.gameObject.SetActive(true);
            }
            else
            {
                ChangeTrans.gameObject.SetActive(false);
            }
        }
    }
}
