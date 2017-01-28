using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

// Artificer
using Data.Shared;
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

    public class ComponentAttributes: MonoBehaviour
    {
    	[HideInInspector] 
    	public KeyCode TriggerKey;
        [HideInInspector] 
        public KeyCode CombatKey;
        [HideInInspector]
    	public bool active;
    	[HideInInspector]
    	public bool CombatState;
        public EllipsoidParticleEmitter emitter;

        public string Name;
        public int ID;
        public bool TrackIntegrity;
    	public float Integrity;
        public float MaxIntegrity;

        public Transform LockedGO;
        public Socket sockInfo;

        public ShieldListener Shield;

        [HideInInspector]
        public List<ComponentListener> connectedComponents;
        [HideInInspector]
        public string currentStyle;

        #region CONSTRUCTION DATA

        public string Description;
        public ConstructInfo[] RequiredMats;
        public StyleInfo[] componentStyles;

        #endregion

        #region SHIP DATA REFERENCE

        public ShipAttributes Ship;

        public ShipData ShipData
        {
            get { return Ship.Ship; }
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
