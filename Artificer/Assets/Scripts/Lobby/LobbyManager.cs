using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Steamworks;

namespace Lobby
{
    /// <summary>
    /// Manager object for Lobby scene
    /// Manages interactions between lobby and user
    /// and server messages e.g. display player in lobby
    /// 
    /// </summary>
    public class LobbyManager : MonoBehaviour
    {
        [SerializeField]
        private LobbyAttributes m_att;

        // CurrentStored State
        //public OnlineState CurrentState;

        // Delegate event for state changed
        //public delegate void ChangeState(OnlineState newState);

        //public event ChangeState OnStateChanged;

        #region MONO BEHAVIOUR

        void Awake()
        {
            // Add Attributes
            //m_att = GetComponent<LobbyAttributes>();
        }

        void Start()
        {
            if (SteamManager.Initialized)
            {
                //CreateHiddenLobby();
            }
        }

        void LateUpdate()
        {
            if (m_att.CurrentLobby != null)
            {
                if (SteamMatchmaking.GetLobbyData(m_att.CurrentLobby.GetID, "live") == "true")
                {
                    // we are in a live game, need the leave option and invite option
                    m_att.SearchBtn.gameObject.SetActive(false);
                    m_att.InviteBtn.gameObject.SetActive(true);
                    m_att.LeaveBtn.gameObject.SetActive(true);
                }
                else
                {
                    //cant leave our private lobby
                    m_att.LeaveBtn.gameObject.SetActive(false);
                    m_att.InviteBtn.gameObject.SetActive(true);
                    // only lobby owners can start search
                    if (SteamMatchmaking.GetLobbyOwner(m_att.CurrentLobby.GetID).Equals(SteamUser.GetSteamID()))
                        m_att.SearchBtn.gameObject.SetActive(true);
                    else
                        m_att.SearchBtn.gameObject.SetActive(false);
                }
            }
            else
            {
                // possible error
                m_att.SearchBtn.gameObject.SetActive(false);
                m_att.InviteBtn.gameObject.SetActive(false);
                m_att.LeaveBtn.gameObject.SetActive(false);
            }
        }

        // Use this for initialization
        /*void Start ()
        {
            m_att.OnlineState = OnlineState.LobbyList;
            CurrentState = OnlineState.LobbyList;
            OnStateChanged(OnlineState.LobbyList);
        }
        
        void Update()
        {

            if (!m_att.OnlineState.Equals(CurrentState))
                OnStateChanged(CurrentState);
        }*/

        #endregion

        #region PUBLIC INTERATION

        [Server]
        public void InitializeLobby(string type)
        {
            switch(type)
            {
                case "lan":
                    m_att.LType = LobbyType.LAN;
                    break;
                case "steam":
                    m_att.LType = LobbyType.Steam;
                    break;
            }

            // create empty Lobby (just act as fully LAN)

        }

        #endregion

        /* #region EVENT LISTENER INTERFACE

         public void StartLobbySearch() // filter will be passed through params (maybe create)
         {
             // SystemManager keeps a persistant string that stores 
             // the current version
             SteamMatchmaking.AddRequestLobbyListStringFilter
                 ("ver", SystemManager.Version,
                 ELobbyComparison.k_ELobbyComparisonEqual);

             SteamMatchmaking.
             AddRequestLobbyListFilterSlotsAvailable
                 (SteamMatchmaking.GetNumLobbyMembers(m_att.CurrentLobby.GetID));
             // Find enough space for everyone within the private lobby

             // Apply request to callback
             SteamAPICall_t handle = SteamMatchmaking.RequestLobbyList();
             m_att.OnLobbyMatchListCallResult.Set(handle);
         }

         /// <summary>
         /// When lobby list has been created
         /// attempt to join the first lobby
         /// or make invisible lobby public if none available
         /// </summary>
         public void JoinSuitableLobby()
         {
             // check that we have lobbies
             if (m_att.LobbyList == null)
             {
                 // no lobbies, create one
                 MakeLobbyPublic();
                 return;
             }
             else if (m_att.LobbyList.Length == 0)
             {
                 // no lobbies, create one
                 MakeLobbyPublic();
                 return;
             }

             Debug.Log("attemping to join lobby");

             // As filters are already applied join the first available lobby
             CSteamID newLobby = m_att.LobbyList[0];

             if (newLobby != CSteamID.Nil)
                 JoinLobby(newLobby);
             else
                 Debug.Log("Something went wrong..");
         }

         /// <summary>
         /// Called when player joins a lobby
         /// func not yet realised
         /// </summary>
         public void JoinAttemptSuccess(CSteamID pLobby)
         {
             Debug.Log("joined");

             // Set user data in this section
             // This could be making the lobby build twice
             SteamMatchmaking.SetLobbyMemberData(pLobby, "ready", "false");

             // if we are not already in this lobby then leave the lobby
             // before joining new
             if (!SteamFriends.IsUserInSource(SteamUser.GetSteamID(), pLobby))
             {
                 LeaveLobby();
             }

             // create new lobby object within memory
             m_att.CurrentLobby = new LobbyObject(pLobby,
                 m_att.LobbyViewer);
         }

         /// <summary>
         /// Called when player rejected by lobby
         /// take lobby out of list and attempt to join next
         /// make invisible lobby public if none available
         /// </summary>
         /// <param name="pLobby"></param>
         public void JoinAttemptFailed(CSteamID pLobby)
         {
             // remove pLobby from lobbylist
             Debug.Log("failed");
             // create new list with one less lobby
             CSteamID[] newLobbyList = new CSteamID[m_att.LobbyList.Length];

             // Add lobbys to list that arent the failed lobby
             int index = 0;
             foreach (CSteamID lobby in m_att.LobbyList)
                 if (lobby != pLobby)
                     newLobbyList[index++] = lobby;

             // assign new list to old
             m_att.LobbyList = newLobbyList;

             // recall join lobby func
             JoinSuitableLobby();
         }

         /// <summary>
         /// if lobby has failed to create 
         /// then we need to inform the player
         /// </summary>
         public void OnLobbyCreationFailed()
         {
             Debug.Log("Lobby failed to create");
         }

         /// <summary>
         /// if lobby has been created then this
         /// function is responsible for assigning initial/default values
         /// </summary>
         public void OnLobbyCreationSuccess(CSteamID pLobby)
         {
             Debug.Log("Lobby created");

             // When a lobby is created it is originally invisible for inviting friends
             // Create data to inform if the lobby is live ot not
             SteamMatchmaking.SetLobbyData(pLobby, "live", "false");

             // Assign the game version to the lobby
             SteamMatchmaking.SetLobbyData(pLobby, "ver", SystemManager.Version);

             // Add more when games are customized
         }

         public void QuitLobby()
         {
             Debug.Log("Attempt to leave Lobby");
             // Check that we arent already in a hidden lobby
             if (SteamMatchmaking.GetLobbyData(m_att.CurrentLobby.GetID, "live") == "true")
             {
                 // Leave the current lobby we are occupying
                 LeaveLobby();

                 Debug.Log("Left Lobby");

                 // Create a new private lobby
                 CreateHiddenLobby();
             }
             // else we have no lobby to leave
         }

         /// <summary>
         /// Opens a steam overlay and that takes over
         /// </summary>
         public void InviteFriends()
         {
             // Make sure we have enough spaces

             // Open a steam window with a lobby invite
             SteamFriends.ActivateGameOverlay("LobbyInvite");
         }

         #endregion

         #region CALLBACK INTERACTION

         /// <summary>
         /// Creates a callback to join a lobby through steam
         /// </summary>
         /// <param name="pLobby"></param>
         private void JoinLobby(CSteamID pLobby)
         {
             if (SteamManager.Initialized)
             {
                 LeaveLobby();

                 SteamAPICall_t handle = SteamMatchmaking.JoinLobby(pLobby);
                 m_att.LobbyEnterCallResult.Set(handle);
             }
         }

         /// <summary>
         /// Couldn't find a suitable lobby,
         /// make our private lobby public
         /// </summary>
         private void MakeLobbyPublic()
         {
             if (SteamMatchmaking.SetLobbyType(m_att.CurrentLobby.GetID,
                 ELobbyType.k_ELobbyTypePublic))
             {
                 // Update the variable to show that we are now in a live lobby
                 SteamMatchmaking.SetLobbyData(m_att.CurrentLobby.GetID, "live", "true");
             }
             // need to consider if failed
         }

         /// <summary>
         /// Creates an friends only lobby for 
         /// invitations and initialing vars for filters 
         /// </summary>
         private void CreateHiddenLobby()
         {
             Debug.Log("creating lobby");

             if (SteamManager.Initialized)
             {
                 SteamAPICall_t handle = SteamMatchmaking.CreateLobby
                     (ELobbyType.k_ELobbyTypePrivate, 32);
                 m_att.LobbyCreatedCallResult.Set(handle);
             }
         }

         /// <summary>
         /// If player is currently in a lobby then leave 
         /// said lobby
         /// </summary>
         public void LeaveLobby()
         {
             if (m_att.CurrentLobby != null)
             {
                 SteamMatchmaking.LeaveLobby(m_att.CurrentLobby.GetID);
                 m_att.CurrentLobby = null;
             }
         }*/
    }
}
