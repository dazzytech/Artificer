using Data.Space;
using Data.Space.Collectable;
using Space.Ship.Components.Attributes;
using Space.Ship.Components.Listener;
using Space.UI.Ship;
using Space.UI.Ship.Target;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Space.Ship
{
    /// <summary>
    /// Attached to the ship
    /// used to access read only attributes 
    /// by components not part of the ship
    /// </summary>
    public class ShipAccessor : NetworkBehaviour
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
        private ShipMessageController m_msg;

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
        /// Returns if the ship is already completed
        /// </summary>
        public bool Complete
        {
            get { return Components.Length > 0; }
        }

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
                // Create a tracker for the best performing warp component
                WarpListener returnWarp = null;
                float range = 0;

                 // loop through each component and compare the warps to keep the best
                foreach (ComponentListener comp in Components)
                {
                    if (comp is WarpListener)
                    {
                        WarpListener compareWarp = comp as WarpListener;
                        // store the warp if is had best distance 
                        if(compareWarp.WarpDistance > range)
                        {
                            returnWarp = compareWarp;
                            range = compareWarp.WarpDistance;
                        }
                    }
                }
                return returnWarp;
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

        #region MATERIALS

        public ItemCollectionData[] Materials
        {
            get
            {
                List<ItemCollectionData> returnValue = new List<ItemCollectionData>();

                foreach (StorageListener storage in m_ship.Storage)
                {
                    StorageAttributes att =
                        (StorageAttributes)storage.GetAttributes();

                    foreach (int key in att.storage.Keys)
                    {
                        ItemCollectionData item = 
                            returnValue.FirstOrDefault(x => x.Item == key);

                        if (item.Exist)
                            item.Amount += att.storage[key];
                        else
                        {
                            item.Item = key;
                            item.Amount = att.storage[key];
                            item.Exist = true;

                            returnValue.Add(item);
                        }
                    }
                }

                return returnValue.ToArray();
            }
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

        public void SetTargetAttributes
            (bool fire, int behaviour = -1)
        {
            foreach (TargeterListener targeter in Targeter)
                targeter.SetTargeting(fire, behaviour);
        }

        /// <summary>
        /// Sets our ship in a combative 
        /// agaisnt the passed transform
        /// </summary>
        /// <param name="combatant"></param>
        public void SetCombatant(Transform combatant)
        {
            if (!hasAuthority)
                return;

            m_ship.Target = combatant;

            m_msg.CmdSetCombat(true);

            m_msg.CancelInvoke("AttackTimer");

            m_msg.Invoke("AttackTimer", 20f);
        }

        #region SHIP CMDS

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
        /// Ship has docked at a station therefore should 
        /// be interactive as well change appearance to indicate
        /// </summary>
        public void DisableShip()
        {
            // First disable any form of input through
            // player interaction
            ShipPlayerInputController input = GetComponent<ShipPlayerInputController>();

            if (input == null)
                return;

            input.enabled = false;

            // remove any targets
            m_ship.TargetedShips.Clear();
            m_ship.SelfTarget = null;

            // Begin the process of hiding components on all components
            m_msg.CmdDisableComponents();

            GetComponent<Rigidbody2D>().constraints =
                RigidbodyConstraints2D.FreezeAll;


            // disable any automatic functioning components
            foreach (TargeterListener listener in m_ship.Targeter)
            {
                listener.enabled = false;
            }
        }

        /// <summary>
        /// When ship has undocked we add player functionality once
        /// again
        /// </summary>
        public void EnableShip()
        {
            // First enable
            // player interaction
            ShipPlayerInputController input = GetComponent<ShipPlayerInputController>();

            if (input == null)
                return;

            input.enabled = true;

            // Begin the process of showing all components
            m_msg.CmdEnableComponents();

            GetComponent<Rigidbody2D>().constraints =
                RigidbodyConstraints2D.None;


            // disable any automatic functioning components
            foreach (TargeterListener listener in m_ship.Targeter)
            {
                listener.enabled = true;
            }
        }

        #endregion

        #region RESOURCE MANAGEMENT

        /// <summary>
        /// If we have a collector listener then 
        /// we add the material to it
        /// </summary>
        /// <param name="itemID"></param>
        public void MaterialGathered(int itemID, float amount = -1)
        {
            if(m_ship.Collector != null)
            {
                m_ship.Collector.ItemGathered(itemID, amount);

                if(OnStorageChanged != null)
                    OnStorageChanged();
            }
        }

        /// <summary>
        /// looting mechanic that inserts large amounts
        /// of materials at once, doesn't bother stacking
        /// </summary>
        /// <param name="mat"></param>
        public Dictionary<int, float> InsertMaterial(Dictionary<int, float> mat)
        {
            // first time only add the item to 
            // storages that already have it
            // for stacking
            foreach (StorageListener storage in Storage)
            {
                mat = storage.AddMaterial(mat);

                if (mat.Count == 0)
                    break;
            }

            if (OnStorageChanged != null)
                OnStorageChanged();

            return mat;
        }

        /// <summary>
        /// Clears the storage of the ship
        /// and returns the assets list
        /// </summary>
        /// <returns></returns>
        public ItemCollectionData[] RemoveAllMaterials()
        {
            if(m_ship.Storage.Count == 0)
            {
                return null;
            }
            else
            {
                // Create a wallet 
                ItemCollectionData[] retVal = Materials;

                foreach(StorageListener storage in m_ship.Storage)
                {
                    StorageAttributes att = 
                        (StorageAttributes)storage.GetAttributes();
                    
                    storage.EjectMaterial(new Dictionary<int, float>(att.storage));
                }

                if (OnStorageChanged != null)
                    OnStorageChanged();

                return retVal;
            }
        }

        /// <summary>
        /// Returns copies of materials
        /// that match the parameter index
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public ItemCollectionData[] GetMaterials(int[] keys)
        {
            return Materials.Where(x => keys.Contains(x.Item)).ToArray();
        }

        /// <summary>
        /// Clears the storage of the ship of a certain index
        /// and returns the assets list
        /// </summary>
        /// <returns></returns>
        public ItemCollectionData[] RemoveMaterials(int[] keys)
        {
            if (m_ship.Storage.Count == 0)
            {
                return null;
            }
            else
            {
                // Create a wallet 
                ItemCollectionData[] retVal = Materials.Where(x => keys.Contains(x.Item)).ToArray();

                foreach (StorageListener storage in m_ship.Storage)
                {
                    StorageAttributes att =
                        (StorageAttributes)storage.GetAttributes();

                    storage.EjectMaterial(new Dictionary<int, float>(att.storage.Where
                        (x => keys.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value)));
                }

                if (OnStorageChanged != null)
                    OnStorageChanged();

                return retVal;
            }
        }

        #region CURRENCY

        /// <summary>
        /// Apply asset transaction to the 
        /// ship storage
        /// </summary>
        /// <param name="assets"></param>
        public void ApplyTransaction(ItemCollectionData[] assets)
        {
            
        }

        #endregion

        #endregion

        #endregion
    }
}
