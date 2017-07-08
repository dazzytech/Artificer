using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
// Artificer
using Data.Shared;
using Space.Ship.Components.Listener;

namespace Space.UI.Station.Editor
{
    /// <summary>
    /// Ship storage controller.
    /// script that controls and manages all the ship
    /// components 
    /// </summary>
    public class EditorUI
        : MonoBehaviour
    {
        #region ATTRIBUTES

        private ShipEditor m_editor;

        #region HUD ELEMENTS

        [Header("HUD Elements")]

        public Text WeightText;

        public Transform ChangeTrans;

        public Toggle RotorFollowsMouse;
        public RectTransform RFRect;

        // Right click items
        public GameObject RCPrefab;
        GameObject RCWindow;
        bool RCDelay;

        #endregion

        #endregion

        // Reset Data function and clear panel here
        public void Initialize(ShipEditor editor)
        {
            m_editor = editor;

            //RotorFollowsMouse.isOn = _ship.Ship.CombatResponsive;
        }

        public delegate void DelegateHead(BaseComponent newHead);

        public void SetHead(BaseComponent newHead)
        {
            m_editor.Ship.Head = newHead;
        }

        public void UpdateUI()
        {
            /*HintBoxController.Clear("Use Right-Click to display additional options");

            foreach (BaseComponent component in m_editor.Ship.Components)
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
                    //if(MouseOver)
                       //StartDrag(component);
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

                        //RCWindow.GetComponent<RightClickComponentMenu>().DisplayComp(component, _ship.Head, new DelegateHead(SetHead));

                        RCDelay = true;
                        Invoke("RCDel", .3f);
                    }
                }
                else if(Input.GetMouseButtonUp(0))
                {
                    //ReleaseDrag();
                }
                else
                {
                    if(MouseOver && RCWindow == null)
                        HintBoxController.Display("Use Right-Click to display additional options");
                }
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
            }*/
        }

        public void RCDel()
        {
            RCDelay = false;
        }


        /*public int ReturnNextAvailableInstance()
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
            foreach (ShipData ship in SystemManager.Player.ShipList)
            {
                if(ship.Name.Contains("Untitled_"))
                {
                    UnAvailable.Add(int.Parse(Regex.Match(ship.Name, @"\d+").Value));
                }
            }

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
        }*/



        /*public void UpdateWeight()
        {
            float wgt = 0;
            foreach (BaseComponent bSC in _ship.Components)
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
        }*/
    }
}
