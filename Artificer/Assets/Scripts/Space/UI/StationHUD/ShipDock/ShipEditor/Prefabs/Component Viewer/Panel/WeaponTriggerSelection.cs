using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;
using Space.UI.Station.Editor.Component;

namespace Space.UI.Station.Utility
{
    /// <summary>
    /// Enables user to define 
    /// weapon firemode
    ///  The index of firetype selected
        /// 0 - primary
        /// 1 - secondary
        /// 2 - tertiary
    /// </summary>
    public class WeaponTriggerSelection : BaseRCPanel
    {
        #region PUBLIC INTERACTION

        /// <summary>
        /// Creates the panel and assigns configuration
        /// </summary>
        /// <param name="att"></param>
        /// <param name="bSC"></param>
        public override void Display(ComponentAttributes att, BaseComponent bC)
        {
            WeaponAttributes weapon = att as WeaponAttributes;

            m_panel.Find("Damage").GetComponent<Text>().text = weapon.WeaponDamage.ToString();
            m_panel.Find("Range").GetComponent<Text>().text = (weapon.WeaponRange*.1f).ToString() + "km";
            m_panel.Find("Delay").GetComponent<Text>().text = weapon.WeaponDelay.ToString("F2");
            m_panel.Find("Type").GetComponent<Text>().text = weapon.ProjectilePrefab.name.ToString();

            m_index = (int)bC.WType;
            
            base.Display(att, bC);
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
