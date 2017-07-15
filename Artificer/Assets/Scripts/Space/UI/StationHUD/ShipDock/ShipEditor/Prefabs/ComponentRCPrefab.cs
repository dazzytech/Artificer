using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;
using Space.UI.Station.Editor;
using Space.UI.Station.Editor.Component;
using Space.UI.Station.Utility;

namespace Space.UI.Station.Prefabs
{
    /// <summary>
    /// Appears when a component is rightclicked
    /// interactable
    /// </summary>
    public class ComponentRCPrefab : MonoBehaviour, IDragHandler
    {
        #region ATTRIBUTES

        #region HUD ELEMENTS

        #region GENERAL

        public Text Title;
        public Text Type;
        public Text Weight;
        public Text Integrity;

        // interaction
        public Toggle HeadToggle;

        #endregion

        #region TYPES

        // Panel Items
        public Transform EnginePanel;
        public Transform WeaponPanel;
        public Transform LauncherPanel;
        public Transform RotorPanel;
        public Transform TargeterPanel;
        public Transform ShieldPanel;
        public Transform WellPanel;
        public Transform WarpPanel;
        public Transform ColourPanel;

        #endregion

        #endregion

        // Ship items
        private BaseComponent Head;
        private BaseComponent BSC;

        // Store the delegated function parent gives us.
        ShipEditor.DelegateHead callHead;

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Initializes RC menu with component
        /// and displays info, also
        /// assigns head assigning delegate
        /// </summary>
        /// <param name="bSC"></param>
        /// <param name="head"></param>
        /// <param name="newcall"></param>
        public void DisplayComp(BaseComponent bSC, BaseComponent head,
                                ShipEditor.DelegateHead newcall)
        {
            BSC = bSC;
            Head = head;
            callHead = newcall;

            ComponentListener Con = bSC.GO.GetComponent<ComponentListener>();
            
            ComponentAttributes att = Con.GetAttributes();
            // Set text items
            Title.text = att.Name;
            Type.text = Con.ComponentType;

            if (Con.ComponentType == "Components")
                HeadToggle.isOn = bSC == head;
            else
                HeadToggle.interactable = false;
            
            // Assign base attributes
            if((Con.Weight * 1000)>= 1000)
                Weight.text =  Con.Weight.ToString("F2")+"Ton(m)";
            else
                Weight.text =  (Con.Weight *1000).ToString("F0")+"KG";
            
            Integrity.text = att.Integrity.ToString();

            // Show panel to corresponding type
            // Any panel with interactivity will have their own script
            switch (Con.ComponentType)
            {
                case "Engines":
                    EnginePanel.gameObject.SetActive(true);
                    EnginePanel.Find("Acceleration").GetComponent<Text>().text = ((EngineAttributes)att).acceleration.ToString("F2");
                    EnginePanel.Find("MaxSpeed").GetComponent<Text>().text = ((EngineAttributes)att).maxSpeed.ToString();
                    break;
                case "Rotors":
                    RotorPanel.gameObject.SetActive(true);
                    RotorPanel.Find("Acceleration").GetComponent<Text>().text = ((RotorAttributes)att).turnAcceleration.ToString("F2");
                    RotorPanel.Find("MaxTurn").GetComponent<Text>().text = ((RotorAttributes)att).maxTurnSpeed.ToString();
                    break;
                case "Weapons":
                    WeaponPanel.gameObject.SetActive(true);
                    WeaponPanel.GetComponent<WeaponTriggerSelection>().
                        Initialize((WeaponAttributes)att, bSC);
                    break;
                case "Launchers":
                    LauncherPanel.gameObject.SetActive(true);
                    LauncherPanel.GetComponent<LauncherCustomizerSelection>().
                        DisplayWeaponInfo((LauncherAttributes)att, bSC);
                    break;
                case "Targeter":
                    TargeterPanel.gameObject.SetActive(true);
                    TargeterPanel.GetComponent<TargeterCustomizerSelection>().
                        DisplayInfo((TargeterAttributes)att, bSC);
                    break;
                case "Shields":
                    ShieldPanel.gameObject.SetActive(true);
                    ShieldPanel.Find("Integrity").GetComponent<Text>().text = ((ShieldAttributes)att).ShieldIntegrity.ToString();
                    ShieldPanel.Find("Radius").GetComponent<Text>().text = ((ShieldAttributes)att).ShieldRadius.ToString()+"m";
                    ShieldPanel.Find("Recharge").GetComponent<Text>().text = ((ShieldAttributes)att).RechargeDelay.ToString()+"s";
                    break;
                case "Wells":
                    WellPanel.gameObject.SetActive(true);
                    WellPanel.Find("Radius").GetComponent<Text>().text = ((CollectorAttributes)att).Radius.ToString()+"m";
                    WellPanel.Find("Force").GetComponent<Text>().text = ((CollectorAttributes)att).PullForce.ToString();
                    break;
                case "Warps":
                    WarpPanel.gameObject.SetActive(true);
                    WarpPanel.Find("WarmUp").GetComponent<Text>().text = ((WarpAttributes)att).WarpWarmUp.ToString()+"s";
                    WarpPanel.Find("CoolDown").GetComponent<Text>().text = ((WarpAttributes)att).WarpDelay.ToString()+"s";
                    WarpPanel.Find("Min").GetComponent<Text>().text = (((WarpAttributes)att).MinDistance*.1f).ToString()+"km";
                    WarpPanel.Find("Max").GetComponent<Text>().text = (((WarpAttributes)att).MaxDistance*.1f).ToString()+"km";
                    break;
                default:
                    break;
            }

            ColourPanel.GetComponent<StyleController>().DisplayInfo(att, bSC);
        }

        /// <summary>
        /// Updates the head toggle visually
        /// </summary>
        public void AssignHead()
        {
            if (HeadToggle.isOn)
            {
                Head = BSC;
                callHead(BSC);
            } else if(Head == BSC)
            {
                HeadToggle.isOn = true;
            }
        }

        #endregion

        #region POINTER DATA

        public void OnDrag(PointerEventData data)
        {
            // can drag to better position     
            Vector2 currentPosition = transform.position;
            currentPosition.x += data.delta.x;
            currentPosition.y += data.delta.y;
            transform.position = currentPosition;
        }

        #endregion
    }
}