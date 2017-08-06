using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Space;
using Space.Ship;
using Space.UI.Ship.Target;

namespace Space.UI.Ship
{
    /// <summary>
    /// Central HUD manager
    /// Currently only role is pass data to other huds when
    /// called by player ship
    /// </summary>
    public class ShipHUD : HUDPanel
    {
        #region ATTRIBUTES

        #region HUD ELEMENTS

        [Header("Ship HUD")]

        [Header("HUD Elements")]

        [SerializeField]
        private TargetHUD m_targetHUD;
        [SerializeField]
        private StorageHUD m_storageHUD;
        [SerializeField]
        private IntegrityHUD m_integrityHUD;

        #endregion

        #endregion

        #region MONOBEHAVIOUR 

        protected override void OnEnable()
        {
            base.OnEnable();

            BuildShipData();
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Initializes and passes player ship
        /// attributes to each panel
        /// </summary>
        public void BuildShipData()
    	{
            GameObject GO = GameObject.FindGameObjectWithTag
                    ("PlayerShip");

            if (GO == null)
                return;

            // Retreive data of player ship
            ShipAccessor ship = GO.GetComponent
                    <ShipAccessor>();

            m_targetHUD.SetShip(ship);

            m_storageHUD.SetShip(ship);

            m_integrityHUD.SetShip(ship);
    	}

        #endregion
    }
}
