using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;
using Space.UI.Station.Editor.Component;

namespace Space.UI.Station.Utility
{
    /// <summary>
    /// The index of firetype selected
        /// 0 - primary
        /// 1 - secondary
        /// 2 - tertiary
    /// </summary>
    public class LauncherCustomizerSelection : BaseRCPanel
    {
        #region ATTRIUBTES

        [Header("Launcher Panel")]

        /// <summary>
        /// if the laucher selects targets automatically
        /// </summary>
        [SerializeField]
        private Toggle m_autoTarget;

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Creates the panel and assigns configuration
        /// </summary>
        /// <param name="att"></param>
        /// <param name="bSC"></param>
        public override void Display(ComponentAttributes att, BaseComponent bC)
        {
            LauncherAttributes launcher = att as LauncherAttributes;

            m_panel.Find("Damage").GetComponent<Text>().text = launcher.WeaponDamage.ToString();
            m_panel.Find("Range").GetComponent<Text>().text = (launcher.WeaponRange*.1f).ToString() + "km";
            m_panel.Find("Delay").GetComponent<Text>().text = launcher.WeaponDelay.ToString("F2");
            m_panel.Find("TargetRange").GetComponent<Text>().text = (launcher.AttackRange*.1f).ToString() + "km";
            m_panel.Find("RCount").GetComponent<Text>().text = launcher.Rockets.ToString();
            m_panel.Find("RDelay").GetComponent<Text>().text = launcher.RocketDelay.ToString("F2");

            m_index = (int)bC.WType;

            base.Display(att, bC);

            m_autoTarget.isOn = m_BC.ShipComponent.AutoLock;
        }

        /// <summary>
        /// Player decides is laucher selects targets
        /// </summary>
        public void AssignAutoAttack()
        {
            m_BC.ShipComponent.AutoLock = m_autoTarget.isOn;
        }

        #endregion

        #region PRIVATE UTILITIES

        protected override void ApplyToggle(int index)
        {
            m_BC.WType = (WeaponType)index;

            m_BC.AssignKey();
        }

        #endregion
    }
}
