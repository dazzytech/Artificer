using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

// Artificer
using Data.Space;
using Space.Ship.Components.Listener;

namespace Space.Ship.Components.Attributes
{
    [System.Serializable]
    public class ConstructInfo
    {
        public string material;
        public float amount;
    }

    [System.Serializable]
    public class StyleInfo
    {
        public Sprite sprite;
        public string name;
    }

    public class ComponentAttributes: NetworkBehaviour
    {
        #region INPUT INTERACTION

        [HideInInspector] 
    	public KeyCode TriggerKey;
        [HideInInspector] 
        public KeyCode CombatKey;
        [HideInInspector]
    	public bool active;
    	[HideInInspector]
    	public bool CombatState;

        #endregion

        public EllipsoidParticleEmitter emitter;

        public ShieldListener Shield;

        #region SPAWN INFORMATION

        [SyncVar]
        public bool ServerReady;

        public bool HasSpawned;

        #endregion

        #region IDENTIFICATION

        public string Name;

        public int ID;

        public Sprite iconImage;

        [SyncVar]
        public NetworkInstanceId ParentID;

        #endregion

        #region INTEGRITY

        public bool TrackIntegrity;
    	public float Integrity;
        public float MaxIntegrity;

        #endregion

        #region SHIP STRUCTURE

        /// <summary>
        /// The Component that this 
        /// component is attached to
        /// </summary>
        [SyncVar]
        public NetworkInstanceId ConnectedObjectNetID;

        /// <summary>
        /// The Socket that this 
        /// component is attached to
        /// </summary>
        [SyncVar]
        public SocketData Socket;

        /// <summary>
        /// Synced list over network 
        /// of components that this 
        /// ship is connected to
        /// </summary>
        public SyncListUInt ConnectedIDs = new SyncListUInt();

        /// <summary>
        /// Contains list of connected 
        /// component listener made on startup
        /// </summary>
        [HideInInspector]
        public List<ComponentListener> connectedComponents;

        #endregion

        #region CONSTRUCTION DATA

        public string Description;
        public ConstructInfo[] RequiredMats;

        #endregion

        #region VISUAL STYLES

        [HideInInspector]
        [SyncVar]
        public string currentStyle;

        public StyleInfo[] componentStyles;

        #endregion

        #region SHIP DATA REFERENCE

        [SyncVar]
        public ComponentData Data;

        public ShipAccessor Ship
        {
            get
            {
                return transform.
                  GetComponentInParent
                  <ShipAccessor>();
            }
        }

        public ShipData ShipData
        {
            get { return Ship.Data; }
        }

        #endregion

        #region ACCESSORS

        public float NormalizedHealth
        {
            get
            {
                if (MaxIntegrity == 0)
                    return 1;
                else
                    return Integrity / MaxIntegrity;
            }
        }

        #endregion
    }
}
