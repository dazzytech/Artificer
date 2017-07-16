using Space.Ship.Components.Attributes;
using Space.Ship.Components.Listener;
using Space.UI.Station.Editor.Component;
using Space.UI.Station.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Space.UI.Station.Prefabs
{
    public class ComponentViewerPrefab : MonoBehaviour
    {
        [Header("Component Viewer")]

        #region HUD ELEMENTS

        #region GENERAL

        [SerializeField]
        private Text m_title;
        [SerializeField]
        private Text m_type;
        [SerializeField]
        private Text m_weight;
        [SerializeField]
        private Text m_integrity;

        #endregion

        #region TYPES

        [SerializeField]
        protected Transform m_enginePanel;
        [SerializeField]
        protected Transform m_weaponPanel;
        [SerializeField]
        protected Transform m_launcherPanel;
        [SerializeField]
        protected Transform m_rotorPanel;
        [SerializeField]
        protected Transform m_targeterPanel;
        [SerializeField]
        protected Transform m_shieldPanel;
        [SerializeField]
        protected Transform m_wellPanel;
        [SerializeField]
        protected Transform m_warpPanel;
        [SerializeField]
        protected Transform m_storagePanel;
        [SerializeField]
        protected Transform m_buildPanel;

        #endregion

        #endregion

        #region PUBLIC INTERACTION

        public virtual void Display(GameObject GO)
        {
            ComponentListener Con = GO.GetComponent<ComponentListener>();

            if (Con != null)
            { 
                ComponentAttributes att = Con.GetAttributes();

                if (att != null)
                {
                    // Set text items
                    m_title.text = att.Name;
                    m_type.text = Con.ComponentType;

                    // Assign base attributes
                    if ((Con.Weight * 1000) >= 1000)
                        m_weight.text = Con.Weight.ToString("F2") + "Ton(m)";
                    else
                        m_weight.text = (Con.Weight * 1000).ToString("F0") + "KG";

                    m_integrity.text = att.Integrity.ToString();
                }
            }
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Adds additional panels dependant on
        /// component type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="att"></param>
        protected virtual void AssignType(string type, ComponentAttributes att)
        {
            DisablePanels();

            switch (type)
            {
                case "Engines":
                    m_enginePanel.gameObject.SetActive(true);
                    m_enginePanel.Find("Acceleration").GetComponent<Text>().text = ((EngineAttributes)att).acceleration.ToString("F2");
                    m_enginePanel.Find("MaxSpeed").GetComponent<Text>().text = ((EngineAttributes)att).maxSpeed.ToString();
                    break;
                case "Rotors":
                    m_rotorPanel.gameObject.SetActive(true);
                    m_rotorPanel.Find("Acceleration").GetComponent<Text>().text = ((RotorAttributes)att).turnAcceleration.ToString("F2");
                    m_rotorPanel.Find("MaxTurn").GetComponent<Text>().text = ((RotorAttributes)att).maxTurnSpeed.ToString();
                    break;
                case "Shields":
                    m_shieldPanel.gameObject.SetActive(true);
                    m_shieldPanel.Find("Integrity").GetComponent<Text>().text = ((ShieldAttributes)att).ShieldIntegrity.ToString();
                    m_shieldPanel.Find("Radius").GetComponent<Text>().text = ((ShieldAttributes)att).ShieldRadius.ToString() + "m";
                    m_shieldPanel.Find("Recharge").GetComponent<Text>().text = ((ShieldAttributes)att).RechargeDelay.ToString() + "s";
                    break;
                case "Wells":
                    m_wellPanel.gameObject.SetActive(true);
                    m_wellPanel.Find("Radius").GetComponent<Text>().text = ((CollectorAttributes)att).Radius.ToString() + "m";
                    m_wellPanel.Find("Force").GetComponent<Text>().text = ((CollectorAttributes)att).PullForce.ToString();
                    break;
                case "Warps":
                    m_warpPanel.gameObject.SetActive(true);
                    m_warpPanel.Find("WarmUp").GetComponent<Text>().text = ((WarpAttributes)att).WarpWarmUp.ToString() + "s";
                    m_warpPanel.Find("CoolDown").GetComponent<Text>().text = ((WarpAttributes)att).WarpDelay.ToString() + "s";
                    m_warpPanel.Find("Min").GetComponent<Text>().text = (((WarpAttributes)att).MinDistance * .1f).ToString() + "km";
                    m_warpPanel.Find("Max").GetComponent<Text>().text = (((WarpAttributes)att).MaxDistance * .1f).ToString() + "km";
                    break;
                case "Storage":
                    m_storagePanel.gameObject.SetActive(true);
                    m_storagePanel.Find("Dimension").GetComponent<Text>().text = 
                        ((StorageAttributes)att).dimensions.ToString() + "ft³";
                    break;
                case "Construct":
                    m_buildPanel.gameObject.SetActive(true);
                    m_buildPanel.GetComponent<BuildController>().
                        Display((BuildAttributes)att);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Enables multi item panels to reset
        /// </summary>
        protected virtual void DisablePanels()
        {
            m_enginePanel.gameObject.SetActive(false);
            m_rotorPanel.gameObject.SetActive(false);
            m_shieldPanel.gameObject.SetActive(false);
            m_wellPanel.gameObject.SetActive(false);
            m_warpPanel.gameObject.SetActive(false);
            m_storagePanel.gameObject.SetActive(false);
            m_buildPanel.gameObject.SetActive(false);
        }

        #endregion
    }
}
