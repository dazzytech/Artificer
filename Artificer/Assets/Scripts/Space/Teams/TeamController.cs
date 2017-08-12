using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Space;
using Space.Ship;
using Space.SpawnManagers;
using Networking;
using Data.Space.Collectable;

/// <summary>
/// An enitity solely responsible for managing all of
/// a teams processes and attributes e.g currently surviving stations and
/// material inventory.
/// Based entirely on server object
/// </summary>
namespace Space.Teams
{

    public class TeamController : NetworkBehaviour
    {
        #region EVENTS

        public delegate void SyncListDelegate();

        // client HUDs listen to this event to update friendly list
        [SyncEvent]
        public event SyncListDelegate EventPlayerListChanged;

        [SyncEvent]
        public event SyncListDelegate EventStationListChanged;

        [SyncEvent]
        public event SyncListDelegate EventShipListChanged;

        #endregion

        #region ATTRIBUTES

        // Faction that the team belongs to
        [SyncVar]
        private FactionData m_faction;

        [SyncVar]
        private int m_ID;

        /// <summary>
        /// Shared assets available 
        /// to the team
        /// </summary>
        [SyncVar]
        private WalletData m_teamAssets;

        // Store a list of player connections for that team
        private SyncListUInt m_players = new SyncListUInt();

        // Store a list of Net IDs of stations that the stations owns
        private SyncListUInt m_stations = new SyncListUInt();

        public SyncListUInt WarpSyncList = new SyncListUInt();

        /// <summary>
        /// Ships available to the team that players may add to
        /// </summary>
        private SyncListShip m_ships = new SyncListShip();

        /*private List<Transform> _attackPoints;
        //private List<Transform> _homePoints;

        // unlocked components
        //private SyncListString _unlockedComponents;

        */

        [SerializeField]
        private TeamSpawnManager _teamSpawn;

        #endregion

        #region MONO BEHAVIOUR

        /*void OnEnable()
        {
            // Assign Events
            ShipMessageController.OnShipDestroyed += ProcessShipDestroyed;
            StationController.OnShipDestroyed += ProcessStationDestroyed;
        }

        void OnDisable()
        {
            // De-assign events
            ShipMessageController.OnShipDestroyed -= ProcessShipDestroyed;
            StationController.OnShipDestroyed -= ProcessStationDestroyed;
        }*/

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// for now just add the faction data to 
        /// local memory
        /// </summary>
        [Server]
        public void Initialize(FactionData faction, int id)
        {
            m_faction = faction;

            m_ID = id;

            // Assign callbacks
            m_players.Callback = PlayerListChanged;
            m_stations.Callback = StationListChanged;
            m_ships.Callback = ShipListChanged;
        }

        /// <summary>
        /// Adds player physical object to list when player spawns
        /// </summary>
        /// <param name="netID"></param>
        [Server]
        public void AddPlayerObject(NetworkInstanceId netID)
        {
            m_players.Add(netID.Value);
        }

        /// <summary>
        /// Called when player leaves team or leaves game or dies
        /// </summary>
        /// <param name="netID"></param>
        [Server]
        public void RemovePlayerObject(NetworkInstanceId netID)
        {
            m_players.Remove(netID.Value);
        }

        /// <summary>
        /// returns if the network ID is associated
        /// with player on this team (friendly fire)
        /// </summary>
        /// <param name="netID"></param>
        /// <returns></returns>
        public bool PlayerOnTeam(NetworkInstanceId netID)
        {
            return m_players.Contains(netID.Value);
        }

        [Server]
        public void AddStationObject(NetworkInstanceId netID)
        {
            m_stations.Add(netID.Value);
        }

        [Server]
        public void RemoveStationObject(NetworkInstanceId netID)
        {
            m_stations.Remove(netID.Value);
        }

        [Server]
        public void AddSpawnableShip(ShipSpawnData ship)
        {
            m_ships.Add(ship);
        }

        #region RESOURCE MANAGEMENT

        /// <summary>
        /// defines teams assets from
        /// starter assets
        /// </summary>
        /// <param name="assets"></param>
        [Server]
        public void DefineTeamAssets(WalletData assets)
        {
            m_teamAssets = assets;
        }

        [Server]
        public int Expend(int value)
        {
            if (m_teamAssets.Currency < value)
                value = m_teamAssets.Currency;

            m_teamAssets.Withdraw(value);

            return value;
        }

        [Server]
        public void Deposit(int value)
        {
            m_teamAssets.Deposit(value);
        }

        [Server]
        public void Expend(ItemCollectionData[] assets)
        {
            m_teamAssets.Withdraw(assets);
        }

        #endregion

        #endregion

        #region ACCESSORS

        /// <summary>
        /// Returns faction ID without access to faction
        /// </summary>
        public int ID
        {
            get { return m_ID; }
        }

        /// <summary>
        /// Returns spawner for use by game object
        /// </summary>
        public TeamSpawnManager Spawner
        {
            get { return _teamSpawn; }
        }

        public SyncListUInt Players
        {
            get { return m_players; }
        }

        public SyncListUInt Stations
        {
            get { return m_stations; }
        }

        public SyncListStruct<ShipSpawnData> Ships
        {
            get { return m_ships; }
        }

        #endregion

        #region EVENT LISTENER

        /// <summary>
        /// When ship is destroyed decide something to do (lol)
        /// </summary>
        /// <param name="destroyed"></param>
        public void ProcessShipDestroyed(DestroyDespatch destroyed)
        {

        }

        public void ProcessStationDestroyed(DestroyDespatch destroyed)
        {

        }

        // Here will be an event listening for station destroyed

        #endregion

        #region CALLBACKS

        void PlayerListChanged(SyncListUInt.
            Operation op, int itemIndex)
        {
            EventPlayerListChanged();
        }

        void StationListChanged(SyncListUInt.
            Operation op, int itemIndex)
        {
            EventStationListChanged();
        }

        void ShipListChanged(SyncListStruct<ShipSpawnData>.
            Operation op, int itemIndex)
        {
            EventShipListChanged();
        }

        #endregion

        /*
        /// <summary>
        /// Called to the server when components are unlocked for the faction.
        /// when added here these components can be added to ships by players
        /// </summary>
        /// <param name="comps"></param>
        [Command]
        public void CmdUnlockComponents(List<string> comps)
        {
            if (_unlockedComponents == null)
                _unlockedComponents = new SyncListString();

            foreach (string comp in comps)
            {
                if (!_unlockedComponents.Contains(comp))
                    _unlockedComponents.Add(comp);
            }
        }

        /// <summary>
        /// Called between objects within the same client
        /// tests that may actually use the ccomponent if in team's arsenal
        /// </summary>
        /// <param name="inComp"></param>
        /// <returns></returns>
        public bool IsUnlocked(string inComp)
        {
            foreach (string comp in _unlockedComponents)
            {
                if (_unlockedComponents.Contains(inComp))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Called from player objects to transfer
        /// materials to the team pool
        /// </summary>
        /// <param name="inMats"></param>
        [Command]
        public void CmdAddMaterialToPool(List<MaterialData> inMats)
        {
            // null if not assigned
            if (_collectedMaterials == null)
                _collectedMaterials = new MaterialListSync();

            foreach (MaterialData mat in inMats)
            {
                // See if station already has material
                int matPos = _collectedMaterials.IndexOf(mat);
                if (matPos == -1) // -1 means we don't already have it
                    _collectedMaterials.Add(mat);
                else
                {
                    // if we already own it, to add the amount to the existing amount.
                    MaterialData homeMat = _collectedMaterials[matPos];
                    homeMat.Amount += mat.Amount;
                    _collectedMaterials[matPos] = homeMat;
                }
            }
        }

        /// <summary>
        /// Returns true if the amount of a material in 
        /// team storage is more or equal to the requirement 
        /// material given
        /// </summary>
        /// <param name="inMat"></param>
        /// <returns></returns>
        public bool CheckTeamStorage(MaterialData inMat)
        {
            // null if not assigned
            if (_collectedMaterials == null)
                return false;

            // See if station already has material
            int matPos = _collectedMaterials.IndexOf(inMat);
            if (matPos == -1) // -1 means we don't already have it
                return false;
            else
            {
                // if we already own it, to add the amount to the existing amount.
                if (_collectedMaterials[matPos].Amount >= inMat.Amount)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Decreases the shared storage
        /// </summary>
        /// <param name="inMats"></param>
        /// <returns></returns>
        public bool ExpendTeamStorage(List<MaterialData> inMats)
        {
            // null if not assigned
            if (_collectedMaterials == null)
                return false;

            // Do our own check to make sure we have all the materials
            foreach (MaterialData inMat in inMats)
            {
                if (!CheckTeamStorage(inMat))
                    return false;           // inadequate storage
            }

            // now we can expend the material
            foreach (MaterialData inMat in inMats)
            {
                // Reduce storage on server
                CmdReduceInventory(inMat);
            }

            // Return successful expendature
            return true;
        }

        /// <summary>
        /// Made when making purchase and decreases stock
        /// </summary>
        /// <param name="inMat"></param>
        [Command]
        private void CmdReduceInventory(MaterialData inMat)
        {
            // See if station has the material
            int matPos = _collectedMaterials.IndexOf(inMat);
            // decrease the amount from the existing amount.
            MaterialData homeMat = _collectedMaterials[matPos];
            homeMat.Amount -= inMat.Amount;

            // Delete material if we don't have any of that material left
            if (homeMat.Amount <= 0)
                _collectedMaterials.RemoveAt(matPos);
            else
                _collectedMaterials[matPos] = homeMat;
        }*/
    }
}