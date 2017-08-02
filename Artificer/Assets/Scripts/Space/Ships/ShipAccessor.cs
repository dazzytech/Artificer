using Data.Space;
using Space.Ship.Components.Listener;
using Space.UI.Ship;
using Space.UI.Ship.Target;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Space.Ship
{
    /// <summary>
    /// Attached to the ship
    /// used to access read only attributes 
    /// by components not part of the ship
    /// </summary>
    public class ShipAccessor : MonoBehaviour
    {
        #region EVENTS

        public delegate void ShipEvent();

        public event ShipEvent OnShipCompleted;

        public event ShipEvent OnStorageChanged;

        #endregion

        #region ATTRIBUTES

        [SerializeField]
        private ShipAttributes m_ship;

        [SerializeField]
        private ShipGenerator m_gen;

        [SerializeField]
        private ShipInputReceiver m_input;

        private bool m_completed = false;

        #endregion

        #region ACCESSORS

        /// <summary>
        /// Exteral objects e.g. AI
        /// are able to interact with the ship
        /// </summary>
        public ShipInputReceiver Input
        {
            get { return m_input; }
        }

        #region COMPONENTS

        /// <summary>
        /// Retrieves the list
        /// of components attached to this GO
        /// </summary>
        public ComponentListener[] Components
        {
            get { return m_ship.Components; }
        }

        /// <summary>
        /// Returns all shield components 
        /// within a ship
        /// </summary>
        public List<ShieldListener> Shields
        {
            get
            {
                List<ShieldListener> targets = new List<ShieldListener>();
                foreach (ComponentListener comp in Components)
                {
                    if (comp is ShieldListener)
                        targets.Add(comp as ShieldListener);
                }

                return targets;
            }
        }

        /// <summary>
        /// Returns the first
        /// occuring instance of a warp component
        /// </summary>
        public WarpListener Warp
        {
            get
            {
                foreach (ComponentListener comp in Components)
                {
                    if (comp is WarpListener)
                        return comp as WarpListener;
                }
                return null;
            }
        }

        /// <summary>
        /// Returns all Targeter components 
        /// within a ship
        /// </summary>
        public List<TargeterListener> Targeter
        {
            get
            {
                List<TargeterListener> targets = new List<TargeterListener>();
                foreach (ComponentListener comp in Components)
                {
                    if (comp is TargeterListener)
                        targets.Add(comp as TargeterListener);
                }

                return targets;
            }
        }

        /// <summary>
        /// Returns all Launcher components 
        /// within a ship
        /// </summary>
        public List<LauncherListener> Launchers
        {
            get
            {
                List<LauncherListener> targets = new List<LauncherListener>();
                foreach (ComponentListener comp in Components)
                {
                    if (comp is LauncherListener)
                        targets.Add(comp as LauncherListener);
                }

                return targets;
            }
        }

        /// <summary>
        /// Returns the number of storage 
        /// components the ship has
        /// </summary>
        public List<StorageListener> Storage
        {
            get { return m_ship.Storage; }
        }

        /// <summary>
        /// Evac is needed.
        /// Tests the ship attributes to see is ship
        /// functions correctly
        /// </summary>
        /// <returns><c>true</c>, if evaced is needed, <c>false</c> otherwise.</returns>
        public bool EvacNeeded
        {
            get
            {
                if (m_ship.Engines == 0)
                    return true;

                if (m_ship.Weapons == 0)
                    return true;

                if (m_ship.Rotors == 0)
                    return true;

                return false;
            }
        }

        #endregion

        #region TARGETS

        /// <summary>
        /// Returns our current target
        /// for external HUD elements
        /// </summary>
        public Transform Target
        {
            get { return m_ship.Target; }
        }

        /// <summary>
        /// Returns selection reference to self
        /// </summary>
        public ShipSelect Self
        {
            get { return m_ship.SelfTarget; }
        }

        /// <summary>
        /// Returns list of targets
        /// for external HUD elements
        /// </summary>
        public List<ShipSelect> Targets
        {
            get { return m_ship.TargetedShips; }
        }

        #endregion

        /// <summary>
        /// Returns transform of head for 
        /// targetting purposes
        /// </summary>
        public Transform Head
        {
            get { return m_ship.Head.transform; }
        }

        /// <summary>
        /// Accessor for team alignment
        /// </summary>
        public int TeamID
        {
            get { return m_ship.TeamID; }
        }

        /// <summary>
        /// used for identifing ship
        /// in clients
        /// </summary>
        public NetworkInstanceId NetID
        {
            get { return m_ship.NetworkID; }
        }

        /// <summary>
        /// returns int labeling the state the station is currently in
        /// 0 - Safe
        /// 1 - Under Attack
        /// 2 - Docked
        /// </summary>
        public int Status
        {
            get
            {
                if (m_ship.ShipDocked)
                    return 2;
                if (m_ship.InCombat)
                    return 1;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Finds the distance between
        /// this remote playerobject
        /// and the local player object
        /// </summary>
        public float Distance
        {
            get
            {
                // Retrieve player object and check if 
                // Player object currently exists
                GameObject playerTransform =
                    GameObject.FindGameObjectWithTag("PlayerShip");

                if (playerTransform == null)
                {
                    return -1;
                }

                // return distance
                return Vector3.Distance(this.transform.position,
                    playerTransform.transform.position);
            }
        }

        /// <summary>
        /// Used for building
        /// external ShipViewers
        /// </summary>
        public ShipData Data
        {
            get { return m_ship.Ship; }
        }

        #endregion

        #region MONO BEHAVIOUR

        private void Update()
        {
            if(!m_completed)
            {
                // +1 for the head
                if((Data.components.Length + 1) == Components.Length)
                {
                    m_completed = true;
                    if(OnShipCompleted != null)
                        OnShipCompleted();
                }
            }
            else if ((Data.components.Length + 1) > Components.Length)
                m_completed = false;
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Returns a list of components from a list of IDs
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ComponentListener[] SelectedComponents(int[] list)
        {
            List<ComponentListener> retList = new List<ComponentListener>();

            // Add components with corresponding IDs 
            // to return list
            foreach (ComponentListener listener in Components)
            {
                if (listener.GetAttributes() != null)
                {
                    foreach (int i in list)
                    {
                        if (listener.GetAttributes().ID == i)
                        {
                            retList.Add(listener);
                        }
                    }
                }
            }

            return retList.ToArray();
        }

        /// <summary>
        /// Rebuilds the ship to new
        /// specifications
        /// </summary>
        /// <param name="newShip"></param>
        public void ResetShip(ShipData newShip)
        {
            // make sure we have an essential aspects
            // before assigning
            if (newShip.Head.Path == null ||
                newShip.components.Length == 0)
                return;

            m_ship.Ship = newShip;

            m_gen.ResetShip();
        } 

        /// <summary>
        /// If we have a collector listener then 
        /// we add the material to it
        /// </summary>
        /// <param name="itemID"></param>
        public void MaterialGathered(int itemID)
        {
            if(m_ship.Collector != null)
            {
                m_ship.Collector.ItemGathered(itemID);

                if(OnStorageChanged != null)
                    OnStorageChanged();
            }
        }

        #endregion
    }
}
