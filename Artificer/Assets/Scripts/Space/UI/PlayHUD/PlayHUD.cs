using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Space;
using Space.Ship;



namespace Space.UI.Ship
{
    /// <summary>
    /// Central HUD manager
    /// Currently only role is pass data to other huds when
    /// called by player ship
    /// </summary>
    public class PlayHUD : MonoBehaviour
    {
        #region ATTRIBUTES

        #region HUD ELEMENTS

        [Header("HUD Elements")]

        [SerializeField]
        private TargetHUD m_targetHUD;
        [SerializeField]
        private StorageHUD m_storageHUD;

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
            ShipAccessor ship = 
                GameObject.FindGameObjectWithTag
                    ("PlayerShip").GetComponent
                    <ShipAccessor>();

            m_targetHUD.SetShip(ship);

            m_storageHUD.SetShip(ship);
    	}

        #endregion
    }
}
