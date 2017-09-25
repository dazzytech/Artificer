using Data.Space;
using Data.UI;
using Space.Teams;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space.UI.Station
{
    public class TraderHubAttributes : MonoBehaviour
    {
        #region REFERENCES

        [HideInInspector]
        // reference to player ship data
        public PlayerData Player;

        [HideInInspector]
        public TeamController Team;

        #endregion
    }
}