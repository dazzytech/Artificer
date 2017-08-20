using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Space.Ship.Components.Listener;
using Space.Ship.Components.Attributes;
using Space.UI.Station.Editor.Component;

namespace Space.UI.Station.Utility
{
    /// <summary>
    /// index for selected action
        /// 0 - autofire
        /// 1 - select manual
        /// 2 - follow mouse
    /// </summary>
    public class TargeterCustomizerSelection : BaseRCPanel
    {
        #region PUBLIC INTERACTION

        /// <summary>
        /// Creates the panel and assigns configuration
        /// </summary>
        /// <param name="att"></param>
        /// <param name="bSC"></param>
        public override void Display(ComponentAttributes att, BaseComponent bC)
        {
            TargeterAttributes targeter = att as TargeterAttributes;

            m_panel.Find("Angle").GetComponent<Text>().text = 
                (targeter.MaxAngle + Mathf.Abs(targeter.MinAngle)).ToString();

            m_panel.Find("Rotate").GetComponent<Text>().text = 
                targeter.turnSpeed.ToString();

            m_panel.Find("Range").GetComponent<Text>().text = 
                (targeter.AttackRange*.1f).ToString()+"km";

            m_index = bC.ShipComponent.Behaviour;
            
            base.Display(att, bC);
        }

        #endregion

        #region PRIVATE UTILITIES

        protected override void ApplyToggle(int index)
        {
            m_BC.ShipComponent.Behaviour = index;
        }

        #endregion
    }
}

