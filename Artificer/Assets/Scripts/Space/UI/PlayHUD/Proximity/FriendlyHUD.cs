using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//Artificier
using Space.Teams;
using Space.Ship;
using UI;

namespace Space.UI.Proxmity
{
    /// <summary>
    /// HUD element for left side
    /// for having a list of items
    /// corresponding to the status
    /// of each friendly ship.
    /// </summary>
    public class FriendlyHUD : SelectableHUDList
    {
        #region ATTRIBUTES

        // Reference to the team we will be retrieving 
        // team list from
        #region ACCESSORS

        private TeamController Team
        {
            get
            {
                if (SystemManager.Space != null)
                    return SystemManager.Space.Team;
                else
                    return null;
            }
        }

        #endregion

        // Stop duplicate HUD elements
        private List<uint> m_addedIDs = new List<uint>();

        #region HUD ELEMENTS

        // List panel to place prefabs
        [Header("HUD Elements")]

        [SerializeField]
        private ProximityHUD m_proximity;

        [SerializeField]
        private Transform m_friendlyList;

        #endregion

        #region PREFABS

        [Header("HUD Prefabs")]
        [SerializeField]
        private GameObject m_friendlyPrefab;

        #endregion

        #endregion

        #region MONO BEHAVIOUR

        // Use this for initialization
        protected override void OnEnable()
        {
            base.OnEnable();

            if (Team != null)
            {
                // event listener here
                Team.EventPlayerListChanged += GenerateTeamList;
                // regenerate team list incase of changes
                GenerateTeamList();
            }
        }

        // Update is called once per frame
        protected override void OnDisable()
        {
            base.OnDisable();

            if (Team != null)
            {
                // event listener here
                Team.EventPlayerListChanged -= GenerateTeamList;
            }
        }

        void Awake()
        {
            // Assign self to receive message
            FriendlyPrefab.Base = this;
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Remove ID from list so new ship can be added
        /// while previous ship HUD is being removed
        /// </summary>
        /// <param name="ID"></param>
        public void RemoveID(uint ID)
        {
            if(m_addedIDs.Contains(ID))
            {
                m_addedIDs.Remove(ID);
            }
        }

        #endregion

        #region PRIVATE UTILITIES

        private void GenerateTeamList()
        {
            if (m_friendlyList == null)
            {
                Debug.Log("ERROR: Friendly HUD - GenerateTeamList: " +
                    "Friendly List HUD has not been assigned.");
                return;
            }

            // Loop through each player and assign a friendly prefab
            foreach(uint ID in Team.PlayerShips)
            {
                // Skip if local player
                if (ID == SystemManager.Space.NetID || m_addedIDs.Contains(ID))
                    continue;

                NetworkInstanceId netID = new NetworkInstanceId(ID);

                // Get ship object from network id
                GameObject friendlyObj = ClientScene.FindLocalObject(netID);

                if (friendlyObj == null)
                {
                    Debug.Log("ERROR: Friendly HUD - GenerateTeamList: " +
                        "Friendly ship not found in client scene.");
                }
                else
                {
                    AddUIPiece(friendlyObj.GetComponent<ShipAccessor>(), ID);
                }
            }
        }

        private void AddUIPiece(ShipAccessor Ship, uint ID)
        {
            // Create Friendly HUD Prefab
            GameObject FriendlyObj = Instantiate(m_friendlyPrefab);

            FriendlyObj.transform.SetParent(m_friendlyList.transform, false);

            FriendlyPrefab FriendlyHUD = FriendlyObj.GetComponent<FriendlyPrefab>();

            // Set Friendly Prefab and initialise
            FriendlyHUD.DefineFriendly(Ship, ID);

            FriendlyHUD.Initialize(m_proximity.Select, m_proximity.Hover, m_proximity.Leave);

            FriendlyHUD.SharedIndex = (int)ID;

            m_prefabList.Add(FriendlyHUD);

            // add to addedlist
            m_addedIDs.Add(ID);
        }

        #endregion
    }
}
