using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Artificier
using Space.Teams;

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

            // Event listener here

            GenerateTeamList();
                
        }

        #endregion

        #region INTERNAL FUNCTIONALITY

        private void GenerateTeamList()
        {

        }

        #endregion
    }
}
