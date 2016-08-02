using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

using Data.Space;
using Data.Space.Library;

namespace Space.UI.Teams
{
    /// <summary>
    /// one time run team picker
    /// displays information regarding the two teams 
    /// and listens for player selection
    /// </summary>
    public class TeamSelectController : NetworkBehaviour
    {
        #region ATTRIBUTES

        // TEAM A UI Elements
        public RawImage TeamAIcon;
        public Text TeamAName;
        public Text TeamACount;

        // TEAM B UI Elements
        public RawImage TeamBIcon;
        public Text TeamBName;
        public Text TeamBCount;

        #endregion

        /// <summary>
        /// Builds the team selectors
        /// </summary>
        /// <param name="teams"></param>
        public void SetTeams(int[] teams)
        {
            FactionData factionA = FactionLibrary.ReturnFaction(teams[0]);

            FactionData factionB = FactionLibrary.ReturnFaction(teams[1]);

            TeamAName.text = factionA.Name;

            TeamBName.text = factionB.Name;


        }

        /// <summary>
        /// Called by buttons to tell server which team we want
        /// </summary>
        /// <param name="selection"></param>
        public void SelectedTeam(int selection)
        {
            GameManager.GameMSG.CmdAssignToTeam
                (selection, GameManager.Space.ID);

            GameManager.Space.InitializePlayer();
        }
    }
}
