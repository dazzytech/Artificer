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
    /// </summary>
    public class WeaponTriggerSelection : MonoBehaviour 
    {
        #region ATTRIUBTES

        public Transform Info;

        public Toggle Primary;
        public Toggle Secondary;
        public Toggle Tertiary;

        public ToggleGroup Group;

        BaseComponent BSC;

        int currentT;

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Creates the panel and assigns configuration
        /// </summary>
        /// <param name="att"></param>
        /// <param name="bSC"></param>
        public void Initialize(WeaponAttributes att, BaseComponent bSC)
        {
            Group.RegisterToggle(Primary);
            Group.RegisterToggle(Secondary);
            Group.RegisterToggle(Tertiary);

            Info.Find("Damage").GetComponent<Text>().text = att.WeaponDamage.ToString();
            Info.Find("Range").GetComponent<Text>().text = (att.WeaponRange*.1f).ToString() + "km";
            Info.Find("Delay").GetComponent<Text>().text = att.WeaponDelay.ToString("F2");
            Info.Find("Type").GetComponent<Text>().text = att.ProjectilePrefab.name.ToString();

            BSC = bSC;

            DisplayTrigger();
        }

        /// <summary>
        /// Sets trigger button when 
        /// player selects a button
        /// </summary>
        /// <param name="trigger"></param>
        public void SetButton(int trigger)
        {
            if (currentT == trigger)
                return;

            switch (trigger)
            {
                case 0:
                    if(Primary.isOn == false)
                        return;
                    BSC.WType = WeaponType.PRIMARY;
                    break;
                case 1:
                    if(Secondary.isOn == false)
                        return;
                    BSC.WType = WeaponType.SECONDARY;
                    break;
                case 2:
                    if(Tertiary.isOn == false)
                        return;
                    BSC.WType = WeaponType.TERTIARY;
                    break;
            }

            BSC.AssignKey();

            DisplayTrigger();
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Updates the trigger visually
        /// </summary>
        private void DisplayTrigger()
        {
            switch(BSC.WType)
            { 
                case WeaponType.PRIMARY:
                    Primary.isOn = true;
                    Secondary.isOn = false;
                    Tertiary.isOn = false;
                    currentT = 0;
                    break;
                case WeaponType.SECONDARY:
                    Primary.isOn = false;
                    Secondary.isOn = true;
                    Tertiary.isOn = false;
                    currentT = 1;
                    break;
                case WeaponType.TERTIARY:
                    Primary.isOn = false;
                    Secondary.isOn = false;
                    Tertiary.isOn = true;
                    currentT = 2;
                    break;
            }
        }

        #endregion
    }
}
