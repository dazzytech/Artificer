using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//Artificier
using Space.Teams;
using Space.Ship;

namespace Space.UI.Ship
{
    /// <summary>
    /// HUD element for left side
    /// for having a list of items
    /// corresponding to the status
    /// of each friendly ship.
    /// </summary>
    public class FriendlyHUD : BasePanel
    {

        #region ATTRIBUTES

        // Reference to the team we will be retrieving 
        // team list from
        private TeamController m_team;

        // Stop duplicate HUD elements
        private List<uint> m_addedIDs = new List<uint>();

        #region HUD ELEMENTS

        // List panel to place prefabs
        [Header("Friendly List Panel")]
        [SerializeField]
        private Transform m_friendlyList;

        #endregion

        #region PREFABS

        [Header("Friendly HUD Prefab")]
        [SerializeField]
        private GameObject m_friendlyPrefab;

        #endregion

        #endregion

        #region MONO BEHAVIOUR

        // Use this for initialization
        void OnEnable()
        {
            if(m_team != null)
            {
                // event listener here
                m_team.EventPlayerListChanged += GenerateTeamList;
                // regenerate team list incase of changes
                GenerateTeamList();
            }
        }

        // Update is called once per frame
        void OnDisable()
        {
            if (m_team != null)
            {
                // event listener here
                m_team.EventPlayerListChanged -= GenerateTeamList;
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// On initialization HUD stores reference to 
        /// team and builds HUD
        /// </summary>
        /// <param name="newTeam"></param>
        public void AssignTeam(TeamController newTeam)
        {
            if(newTeam != null)
                m_team = newTeam;
            else
            {
                Debug.Log("ERROR: Friendly HUD - Passed team variable is null.");
                return;
            }

            // if comp is already active then we need to set up events
            if(isActiveAndEnabled)
            {
                m_team.EventPlayerListChanged += GenerateTeamList;
                GenerateTeamList();
            }
        }

        #endregion

        #region INTERNAL FUNCTIONALITY

        private void GenerateTeamList()
        {
            if (m_friendlyList == null)
            {
                Debug.Log("ERROR: Friendly HUD - GenerateTeamList: " +
                    "Friendly List HUD has not been assigned.");
                return;
            }

            // loop through each item in list and destroy it
            // refresh
            foreach (Transform child in m_friendlyList.transform)
                Destroy(child.gameObject);

            m_addedIDs.Clear();

            // Loop through each player and assign a friendly prefab
            foreach(uint ID in m_team.Players)
            {
                // Skip if local player
                if (ID == GameManager.Space.NetID || m_addedIDs.Contains(ID))
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
                    // retrive ship attributes
                    ShipAttributes friendlyShip = friendlyObj.GetComponent<ShipAttributes>();

                    // Create Friendly HUD Prefab
                    GameObject FriendlyHUD = Instantiate(m_friendlyPrefab);

                    FriendlyHUD.transform.SetParent(m_friendlyList.transform, false);

                    // Set Friendly Prefab and initialise
                    FriendlyHUD.SendMessage("DefineFriendly", friendlyShip);

                    // add to addedlist
                    m_addedIDs.Add(ID);
                }
            }
        }

        #endregion
    }
}
