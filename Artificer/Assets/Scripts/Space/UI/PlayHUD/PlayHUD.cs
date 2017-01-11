using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;
using Space.UI.Target;
using Space.UI.Tracker;
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
        // Ship attributes for player ship
        private ShipAttributes _shipData;

        // Other GUIs
        public IntegrityHUD _int;
        public WarpHUD _warp;
        public TargetHUD _target;
        public TrackerHUD _tracker;
        public ShieldHUD _shields;
        public StationHUD _station;

    	// Use this for initialization
        public void BuildShipData()
    	{
            // Retreive data of player ship
            _shipData = 
                GameObject.FindGameObjectWithTag
                    ("PlayerShip").GetComponent
                    <ShipAttributes>();

            _int.SetShipData(_shipData);
            _warp.SetShipData(_shipData);
            _target.SetShipData(_shipData);
            _shields.SetShipData(_shipData);
    	}

        public void HidePanel(string panel)
        {
            switch(panel)
            {
                case "station":
                    _station.ToggleHUD();
                    break;
            }
        }
    }
}
