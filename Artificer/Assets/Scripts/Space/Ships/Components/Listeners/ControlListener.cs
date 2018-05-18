using Space.Ship.Components.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.Ship.Components.Listener
{
    public class ControlListener : ComponentListener
    {
        #region ATTRIBUTES

        private ControlAttributes m_att;

        #endregion

        #region ACCESSOR

        // private add NPCPREFAB here

        #endregion

        #region PUBLIC INTERACTION

        public override void Activate()
        {
            //open ui
        }

        #endregion

        #region PRIVATE UTILITIES

        protected override void InitializeComponent()
        {
            base.InitializeComponent();

            ComponentType = "Control";
            m_att = GetComponent<ControlAttributes>();

            if (hasAuthority)
            {
                // what is needed?
            }
        }

        #endregion
    }
}