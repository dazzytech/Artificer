using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

using Data.Space;
using Data.Space.Library;
using Networking;

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
        public RectTransform TeamARect;

        // TEAM B UI Elements
        public RawImage TeamBIcon;
        public Text TeamBName;
        public Text TeamBCount;
        public RectTransform TeamBRect;

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

            Texture2D iconA = Resources.Load("Textures/FactionTextures/" + 
                factionA.Icon, typeof(Texture2D)) as Texture2D;

            Texture2D iconB = Resources.Load("Textures/FactionTextures/" + 
                factionB.Icon, typeof(Texture2D)) as Texture2D;

            if (iconA != null)
                TeamAIcon.texture = iconA;

            if (iconB != null)
                TeamBIcon.texture = iconB;
        }

        /// <summary>
        /// Called by buttons to tell server which team we want
        /// </summary>
        /// <param name="selection"></param>
        public void SelectedTeam(int selection)
        {
            // Create our message for the server
            TeamSelectionMessage tsm = new TeamSelectionMessage();
            tsm.ID = SystemManager.Space.ID;
            tsm.Selected = selection;
            // Send message baack to server
            SystemManager.singleton.client.Send((short)MSGCHANNEL.TEAMSELECTED, tsm);
        }
    }
}
