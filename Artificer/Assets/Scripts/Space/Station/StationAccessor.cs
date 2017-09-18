using Data.UI;
using Networking;
using Space.Segment;
using Space.Ship;
using Space.Teams;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Stations
{
    /// <summary>
    /// Enables external objects
    /// to interface with the station prefab.
    /// </summary>
    public class StationAccessor : NetworkBehaviour
    {
        #region ATTRIBUTES

        [SerializeField]
        private StationAttributes m_att;

        [SerializeField]
        private StationController m_con;

        #endregion

        #region ACCESSORS

        /// <summary>
        /// Return unique ID
        /// </summary>
        public int SpawnID
        {
            get { return m_att.SpawnID; }
            set { m_att.SpawnID = value; }
        }

        /// <summary>
        /// returns int labeling the state the station is currently in
        /// 0 - Safe
        /// 1 - Under Attack
        /// 2 - Destroyed
        /// 3 - Building
        /// </summary>
        public int Status
        {
            get
            {
                if (m_att.UnderAttack)
                    return 1;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Return texture within sprite renderer of this object as sprite
        /// </summary>
        public Sprite Icon
        {
            get
            {
                if (m_att.Icon != null)
                    return m_att.Icon;
                else
                    return GetComponent<SpriteRenderer>().sprite;
            }
        }

        /// <summary>
        /// Return health in a value between 1.0 - 0.0
        /// </summary>
        public float NormalizedHealth
        {
            get
            {
                if (m_att.Integrity == 0)
                    return 1;
                else
                    return m_att.CurrentIntegrity / m_att.Integrity;
            }
        }

        /// <summary>
        /// Distance between the player and this station
        /// </summary>
        public float Distance
        {
            get
            {
                // Retrieve player object and check if 
                // Player object currently exists
                GameObject playerTransform =
                    GameObject.FindGameObjectWithTag("PlayerShip");

                if (playerTransform == null)
                {
                    return -1;
                }

                // return distance
                return Vector3.Distance(this.transform.position,
                    playerTransform.transform.position);
            }
        }

        /// <summary>
        /// Distance the station needs to be empty
        /// </summary>
        public float Radius
        {
            get
            {
                return m_att.ClearRadius;
            }
        }

        /// <summary>
        /// Returns is the station object
        /// is actually active
        /// </summary>
        public bool Active
        {
            get { return m_con.Active; }
        }

        /// <summary>
        /// External tools react differently to types
        /// </summary>
        public STATIONTYPE Type
        {
            get { return m_att.Type; }
        }

        /// <summary>
        /// Quick access to station team object
        /// </summary>
        public TeamController Team
        {
            get
            {
                return ClientScene.FindLocalObject(m_att.TeamID).GetComponent<TeamController>();
            }
        }

        /// <summary>
        /// Returns a read only message
        /// that prompts the player to dock
        /// </summary>
        public PromptData DockPrompt
        {
            get { return m_att.DockPrompt; }
        }

        /// <summary>
        /// Returns a read only message
        /// That prompts an the player to interact 
        /// </summary>
        public PromptData InteractPrompt
        {
            get { return m_att.InteractPrompt; }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Called externally when 
        /// station deployed
        /// </summary>
        /// <param name="newTeam"></param>
        /// <param name="delayBuild"></param>
        [Server]
        public void Initialized
            (NetworkInstanceId newTeam, bool delayBuild = false)
        {
            m_con.Initialize(newTeam, delayBuild);
        }

        /// <summary>
        /// Called by space manager when
        /// player has pressed or released
        /// interact when in range of station
        /// </summary>
        public void Interact(bool keyPressed, 
            ShipAccessor ship = null)
        {
            if (keyPressed)
                m_con.Interact(ship);
            else
                m_con.Idle(ship);
        }

        /// <summary>
        /// Called when the player 
        /// has pressed dock within
        /// range of station
        /// </summary>
        public void Dock(bool dock, 
            ShipAccessor ship = null)
        {
            if (dock)
                m_con.Dock(ship);
            else
                m_con.UnDock(ship);
        }

        /// <summary>
        /// Called by space manager when its
        /// range event is triggered
        /// </summary>
        public void Range(bool entered, ShipAccessor ship = null)
        {
            if (entered)
                m_con.EnterRange(ship);
            else
                m_con.ExitRange(ship);
        }

        #endregion
    }
}