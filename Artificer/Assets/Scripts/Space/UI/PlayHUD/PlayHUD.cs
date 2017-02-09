using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;
using Space.Ship;


/// <summary>
/// Central HUD manager
/// Currently only role is pass data to other huds when
/// called by player ship
/// </summary>
namespace Space.UI.Ship
{
    public class PlayHUD : MonoBehaviour
    {
        #region ATTRIBUTES

        // Ship attributes for player ship
        //private ShipAttributes _shipData;

        #region HUD ELEMENTS

        // Other GUIs
        //public IntegrityHUD _int;
        //public WarpHUD _warp;
        [Header("HUD Elements")]

        [SerializeField]
        public TargetHUD m_targetHUD;
        //public TrackerHUD _tracker;
        //public ShieldHUD _shields;

        [SerializeField]
        public StationHUD m_station;

        #endregion

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Initializes and passes player ship
        /// attributes to each panel
        /// </summary>
        public void BuildShipData()
    	{
            // Retreive data of player ship
            ShipAttributes shipData = 
                GameObject.FindGameObjectWithTag
                    ("PlayerShip").GetComponent
                    <ShipAttributes>();

            m_targetHUD.SetShipData(shipData);

            //_int.SetShipData(shipData);
            //_warp.SetShipData(shipData);
            //_shields.SetShipData(shipData);
    	}

        /// <summary>
        /// Hides the specified panel
        /// </summary>
        /// <param name="panel"></param>
        public void HidePanel(string panel)
        {
            switch(panel)
            {
                case "station":
                    m_station.ToggleHUD();
                    break;
            }
        }

        #endregion
    }
}
