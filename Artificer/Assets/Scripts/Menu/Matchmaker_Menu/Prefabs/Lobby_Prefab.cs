using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Steamworks;

namespace Menu
{
    public class Lobby_Prefab : MonoBehaviour
    {
        // Prefab Control Vars
        [SerializeField]
        private Text Name;
        [SerializeField]
        private Text Desc;
        [SerializeField]
        private Text Type;
        [SerializeField]
        private Text Version;
        [SerializeField]
        private Text Players;
        [SerializeField]
        private Button Btn;

        // Store lobby Steam ID
        private CSteamID m_Lobby;

        // Initialize Button and assign text
        public IEnumerator InitializeLobby(CSteamID pLobby, LobbyList_Behaviour.selectLobby SelLobby)
        {
            // For iterate through each data item 
            for(int i = 0; i < SteamMatchmaking.GetLobbyDataCount(pLobby); i++)
            {
                // return parameters
                string Key;
                string Value;
                
                // retrive name data from lobby
                bool success = SteamMatchmaking.GetLobbyDataByIndex
                    (pLobby, i, out Key, 255, out Value, 255);

                //output Lobby data
                if(success)
                {
                    // if its the info we want then output it to the right text
                    switch(Key)
                    {
                        case "name":
                            Name.text = Value;
                            break;
                        case "desc":
                            Desc.text = Value;
                            break;
                        case "type":
                            Type.text = Value;
                            break;
                        case "ver":
                            Version.text = Value;
                            break;
                    }
                }
                yield return null;
            }

            // create member count
            int MaxMembers = SteamMatchmaking.GetLobbyMemberLimit(pLobby);
            int CurrentMembers = SteamMatchmaking.GetNumLobbyMembers(pLobby);

            // Display ratio
            Players.text = string.Format("{0} -- {1}", CurrentMembers, MaxMembers);

            // Assign delegate to button
            Btn.onClick.AddListener(() => {SelLobby(pLobby);});

            // Assign lobby
            m_Lobby = pLobby;

            yield return null;
        }
    }
}

