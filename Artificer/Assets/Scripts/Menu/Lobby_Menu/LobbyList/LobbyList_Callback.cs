using UnityEngine;
using System.Collections;
using Steamworks;

namespace Menu
{
    [RequireComponent(typeof(LobbyList_Behaviour))]
    public class LobbyList_Callback : MonoBehaviour
    {
        private LobbyList_Behaviour m_controller;

        /// <summary>
        /// Refresh the lobby list
        /// </summary>
        public void Refresh()
        {
            if(SteamManager.Initialized)
            {
                // Implement filtering in param and here
                if(m_controller.Filter != null)
                {
                    //SteamMatchmaking.AddRequestLobbyListCompatibleMembersFilter();
                    FilterInfo info = m_controller.Filter;
                    // apply filter to steam matchmaker
                    if(info.MaxLobbies > 0)
                        SteamMatchmaking.
                            AddRequestLobbyListResultCountFilter
                            (info.MaxLobbies);

                    if(info.SlotsAvailable > 0)
                        

                    if(info.LobbyName != null)
                        SteamMatchmaking.
                            AddRequestLobbyListStringFilter(
                                "name", info.LobbyName, 
                                ELobbyComparison.k_ELobbyComparisonEqual);

                }

                
            }
        }


      
    }
}

