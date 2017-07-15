using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
// Artificer
using Data.Shared;
using Space.Ship.Components.Listener;
using Space.UI.Station.Editor.Component;
using UI.Effects;
using Space.UI.Station.Prefabs;

namespace Space.UI.Station.Editor
{
    /// <summary>
    /// Script that interacts with the UI
    /// e.g. placing pieces, updating
    /// text.
    /// </summary>
    public class EditorUI
        : MonoBehaviour
    {
        #region ATTRIBUTES

        private ShipEditor m_editor;

        #region HUD ELEMENTS

        [Header("HUD Elements")]

        #region SHIP INFORMATION

        /// <summary>
        /// Displays the weight of the ship
        /// in tons
        /// </summary>
        [SerializeField]
        private Text m_weightText;

        /// <summary>
        /// Highlights if the ship has been
        /// edited in any way
        /// </summary>
        [SerializeField]
        private Transform m_changedText;

        /// <summary>
        /// Text field for editing the ship name
        /// </summary>
        [SerializeField]
        private InputField m_shipName;

        #region ROTOR FOLLOW

        /// <summary>
        /// Toggle box that determines 
        /// if the rotors follow the mouse
        /// when the ship is in combat mode
        /// </summary>
        [SerializeField]
        private Toggle m_rotorFollowsMouse;
        [SerializeField]
        private RectTransform m_RFRect;

        #endregion

        #endregion

        #region CONSTRUCTION

        /// <summary>
        /// The panel that the ship is built on
        /// </summary>
        [SerializeField]
        private Transform m_shipConstructPanel;

        /// <summary>
        /// The transform that limits the movement
        /// of the ship construct panel
        /// </summary>
        [SerializeField]
        private Transform m_shipBoundsPanel;

        /// <summary>
        /// Prefab interactable object for base component
        /// object
        /// </summary>
        [SerializeField]
        private GameObject m_componentPrefab;

        #endregion

        #region RIGHT CLICK

        // Right click items
        public GameObject RCPrefab;
        GameObject RCWindow;
        bool RCDelay;

        #endregion

        #endregion

        #endregion

        #region PUBLIC INTERACTION

        // Reset Data function and clear panel here
        public void Initialize(ShipEditor editor, string newShipName)
        {
            m_editor = editor;

            m_shipName.text = newShipName;

            //RotorFollowsMouse.isOn = _ship.Ship.CombatResponsive;
        }

        public void UpdateUI()
        {
            HintBoxController.Clear("Use Right-Click to display additional options");
            HintBoxController.Clear("Use the directional keys to change direction the component is facing.");

            if (ShipEditor.DraggedObj != null)
                HintBoxController.Display("Use the directional keys to change direction the component is facing.");
            else if(ShipEditor.HighlightedObj != null)
                HintBoxController.Display("Use Right-Click to display additional options");
            
            if(RCWindow != null && !RCDelay)
            {
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    if(!RectTransformExtension.InBounds(RCWindow.GetComponent<RectTransform>(), 
                        Input.mousePosition) || Input.GetMouseButtonDown(1))
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
                else if(Input.GetMouseButtonDown(1))
                {
                    // Player has clicked the right mouse button
                    if(RCWindow == null && ShipEditor.HighlightedObj != null)
                    {
                        // Create the Right Click Window
                        RCWindow = Instantiate(RCPrefab);
                        RCWindow.transform.SetParent(GameObject.Find("_gui").transform);

                        RCWindow.transform.position = Input.mousePosition
                            + new Vector3(-RCWindow.GetComponent<RectTransform>().rect.width*.5f-10,
                                          RCWindow.GetComponent<RectTransform>().rect.height*.5f+10, 1);

                        // Create delegate function

                        RCWindow.GetComponent<ComponentRCPrefab>().DisplayComp
                            (ShipEditor.HighlightedObj, m_editor.Ship.Head,
                            new ShipEditor.DelegateHead(m_editor.SetHead));

                        RCDelay = true;
                        Invoke("RCDel", .3f);
                    }
                }
            
            /*
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

        #region EDITOR UTILITY

        /// <summary>
        /// Returns if a given component (change to position?)
        /// it within bounds of the ui editor
        /// </summary>
        /// <param name="BC"></param>
        /// <returns></returns>
        public bool IsWithinBounds(BaseComponent BC)
        {
            return RectTransformExtension.InBounds
                            (m_shipBoundsPanel.GetComponent<RectTransform>(),
                            BC.transform.position);
        }

        /// <summary>
        /// Places the component GameObject to the panel.
        /// </summary>
        /// <returns>The component.</returns>
        /// <param name="component"></param>
        public BaseComponent PlaceComponentGO(Data.Shared.ComponentData component)
        {
            // Create Ship Object and apply transform parent
            GameObject newObj = Instantiate(m_componentPrefab);
            newObj.transform.SetParent(m_shipConstructPanel);

            // Create BaseShipComponent
            BaseComponent BC = newObj.GetComponent<BaseComponent>();
            BC.InitComponent(component);

            //UpdateWeight();

            return BC;
        }

        #endregion

        #endregion

        #region PRIVATE UTILITIES

        private void RCDel()
        {
            RCDelay = false;
        }

        #endregion


        /*

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
