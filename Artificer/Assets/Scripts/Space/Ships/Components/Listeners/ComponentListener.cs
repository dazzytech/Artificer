using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
// Artificer
using Networking;
using Data.Space;
using Space.Segment;
using Space.Ship.Components.Attributes;
using System;

namespace Space.Ship.Components.Listener
{
    public class ComponentListener : NetworkBehaviour
    {
        #region ATTRIBUTES

        protected Rigidbody2D rb;

    	public float Weight;
        
        // Used by editor tools to determine type
        public string ComponentType;

        #endregion
        
        #region ACCESSORS

        public ComponentAttributes GetAttributes()
        {
            if (transform == null)
                return null;
            else if (transform.GetComponent<ComponentAttributes>() != null)
                return transform.GetComponent<ComponentAttributes>();
            else
                return null;
        }

        private ComponentStyleUtility Style
        {
            get
            {
                if (transform == null)
                    return null;
                else if (transform.GetComponent<ComponentStyleUtility>() != null)
                    return transform.GetComponent<ComponentStyleUtility>();
                else
                    return null;
            }
        }

        public int ID
        {
            get
            {
                return GetAttributes().ID;
            }
            set
            {
                GetAttributes().ID = value;
            }
        }

        public NetworkInstanceId ConnectedID
        {
            get { return GetAttributes().ConnectedObjectNetID; }
            set { GetAttributes().ConnectedObjectNetID = value; }
        }

        private Transform ConnectedObj
        {
            get { return ClientScene.FindLocalObject(ConnectedID).transform; }
        }

        public SocketData Socket
        {
            get { return GetAttributes().Socket; }
            set { GetAttributes().Socket = value; }
        }

        private ComponentData Data
        {
            get { return GetAttributes().Data; }
            set { GetAttributes().Data = value; }
        }

        private bool HasSpawned
        {
            get { return GetAttributes().HasSpawned; }
            set { GetAttributes().HasSpawned = value; }
        }

        private bool ServerReady
        {
            get { return GetAttributes().ServerReady; }
            set { GetAttributes().ServerReady = value; }
        }

        private NetworkInstanceId ParentID
        {
            get { return GetAttributes().ParentID; }
            set { GetAttributes().ParentID = value; }
        }

        /// <summary>
        /// Returns the UI icon of this components
        /// </summary>
        /// <returns></returns>
        public Sprite Icon
        {
            get
            {
                ComponentAttributes att = GetAttributes();
                if (att.iconImage == null)
                    return GetComponentInChildren<SpriteRenderer>().sprite;
                else
                    return att.iconImage;
            }
        }

        /// <summary>
        /// Lower left point of component
        /// item in world space units
        /// </summary>
        public Vector2 Min
        {
            get
            {
                return (Vector2)(transform.localPosition -
                    GetComponentInChildren<SpriteRenderer>().
                    sprite.bounds.extents);
            }
        }

        /// <summary>
        /// Upper Right point of component item 
        /// in world space units
        /// </summary>
        public Vector2 Max
        {
            get
            {
                return (Vector2)(transform.localPosition +
                    GetComponentInChildren<SpriteRenderer>().
                    sprite.bounds.extents);
            }
        }

        /// <summary>
        /// Quick access local position world units
        /// </summary>
        public Vector3 Postion
        {
            get { return transform.localPosition; }
        }

        public float NormalizedHealth
        {
            get
            {
                ComponentAttributes att = GetAttributes();
                if (att.MaxIntegrity == 0)
                    return 1;
                else
                    return att.Integrity / att.MaxIntegrity;
            }
        }

        #endregion

        #region MONO BEHAVIOUR

        /// <summary>
        /// Builds the ship if it was built on server
        /// </summary>
        public override void OnStartClient()
        {
            if (ServerReady)
            {
                InitializeComponent();
            }
        }

        private void Update()
        {
            // Check that data has synced accross network
            if (ServerReady && !HasSpawned)
                InitializeComponent();

            if (ServerReady && hasAuthority)
                RunUpdate();
        }

        #endregion

        #region PUBLIC INTERACTION

        #region INITIALIZE COMPONENT

        /// <summary>
        /// Receives the component data
        /// for the component that assigns all the 
        /// synced data for initialization
        /// </summary>
        /// <param name="cData"></param>
        [Server]
        public virtual void InitializeData
            (ComponentData cData, NetworkInstanceId parent,
            NetworkInstanceId connected, SocketData socket)
        {
            // Assign the data to the att
            Data = cData;

            // Assign our parent netID
            ParentID = parent;

            // Assign connection data
            ConnectedID = connected;
            Socket = socket;

            // Change visual style
            Style.SetStyle(Data.Style);

            // Allow comp to init
            ServerReady = true;
        }

        /// <summary>
        /// Adds the connected component to the list
        /// </summary>
        /// <param name="connected">Connected.</param>
        [Server]
        public void AddConnection(NetworkInstanceId connected)
        {
            ComponentAttributes att = GetAttributes();

            att.ConnectedIDs.Add(connected.Value);
        }

        #endregion

        #region COMPONENT DAMAGE

        /// <summary>
        /// Applys damage from impact collider to the
        /// component
        /// </summary>
        /// <param name="hit"></param>
        public void DamageComponent
            (HitData hit, float damage, bool authority)
        {
            ComponentAttributes att = GetAttributes();

            if (att.Shield != null)
            {
                // Bypass damage as shield absorbs damage
                att.Shield.Impact(hit);
                return;
            }

            att.Integrity -= damage;

            SetColour(att.Integrity);

            if (authority)
            {
                // May need to add local player here
                if (att.Integrity <= 0)
                {
                    hit.hitComponent = att.ID;
                    transform.parent.gameObject.
                    SendMessage("DestroyComponent", hit,
                    SendMessageOptions.DontRequireReceiver);
                }

                IntegrityChangedMsg msg = new IntegrityChangedMsg();
                msg.Amount = -damage;
                msg.Location = transform.position;
                msg.PlayerID = SystemManager.Space.ID;

                SystemManager.singleton.client.Send((short)MSGCHANNEL.INTEGRITYCHANGE, msg);
            }
        }

        public void HealComponent(float amount)
        {
            ComponentAttributes att = GetAttributes();

            att.Integrity += amount;

            if (att.Integrity > att.MaxIntegrity)
            {
                att.Integrity = att.MaxIntegrity;
            }
        }

        /// <summary>
        /// Darkens the colour of the component based
        /// on integrity
        /// </summary>
        /// <param name="integrity"></param>
        public void SetColour(float integrity)
        {
            // Change color to show damage for now
            float brightness = (integrity / GetAttributes().MaxIntegrity) * 0.5f;

            GetComponentInChildren<SpriteRenderer>().color =
                new Color(0.5f + brightness, 0.5f + brightness, 0.5f + brightness);
        }

        /// <summary>
        /// called to disable this component
        /// </summary>
        public virtual void Destroy()
        {
            // will be important when able to loot, and stop locking
            SetRB();
            this.enabled = false;
        }

        #endregion

        #region COMPONENT VISIBILITY

        /// <summary>
        /// disables all external interaction
        /// with component and gradually fades out
        /// visual
        /// </summary>
        public void HideComponent()
        {
            // stop component from running
            Deactivate();

            // Deactivate collider object so other components 
            // do not interact
            if (GetComponent<Collider2D>() != null)
                GetComponent<Collider2D>().enabled = false;

            // begin fading process
            StartCoroutine("FadeOut");
        }

        /// <summary>
        /// Enables component again and fades in
        /// </summary>
        public void ShowComponent()
        {
            // Activate collider object 
            if (GetComponent<Collider2D>() != null)
                GetComponent<Collider2D>().enabled = true;

            // begin fading process
            StartCoroutine("FadeIn");
        }

        #endregion

        #region COMPONENT STATE

        public virtual void Activate()
        { CmdActivateFx(); }

        public virtual void Deactivate()
        { CmdDeactivateFx(); }

        #endregion

        /// <summary>
        /// Snap this component to the component
        /// used from external
        /// </summary>
        public void FixToConnected()
        {
            SetLocation();

            transform.rotation = ConnectedObj.rotation;
        }

        #endregion

        #region PRIVATE UTILTIES

        protected virtual void RunUpdate()
        { }

        #region VISUAL FX

        [Command]
        private void CmdActivateFx()
        {
            RpcActivateFx();
        }

        [ClientRpc]
        private void RpcActivateFx()
        {
            ActivateFx();
        }

        protected virtual void ActivateFx()
        {

        }

        [Command]
        private void CmdDeactivateFx()
        {
            RpcDeactivateFx();
        }

        [ClientRpc]
        private void RpcDeactivateFx()
        {
            DeactivateFx();
        }

        protected virtual void DeactivateFx()
        {

        }

        #endregion

        #region INITIALZATION

        /// <summary>
        /// Called when the ship data has been set
        /// to populate variables
        /// </summary>
        protected virtual void InitializeComponent()
        {
            ComponentType = "Components";

            transform.SetParent(ClientScene.FindLocalObject
                (ParentID).transform);

            ID = Data.InstanceID;

            // update visually
            Style.ApplyStyle();

            InitEmitter();

            // If we are interacting with the ship then
            // Define the interactive variables
            if(hasAuthority)
            {
                SetRB();

                SetTriggers(Data.Trigger, Data.CTrigger);
            }

            SetRotation();

            SetLocation();

            // If ship is already docked then we must hide it
            if (GetAttributes().Ship.ShipDocked)
            {
                if (GetComponent<Collider2D>() != null)
                    GetComponent<Collider2D>().enabled = false;

                GetComponentInChildren<SpriteRenderer>().color =
                    new Color(1.0f, 1.0f, 1.0f, 0f);
            }

            SendMessageUpwards("AddComponent", this);

            HasSpawned = true;
        }

        /// <summary>
        /// Initializes visual engine particle effects
        /// on components. 
        /// </summary>
        private void InitEmitter()
        {
            Transform GO = transform.Find
                ("Engine");

            if(GO != null)
                GetAttributes().emitter = GO.GetComponent
                    <EllipsoidParticleEmitter>();
        }

        /// <summary>
        /// Initialise the physics component
        /// of the listener
        /// </summary>
        private void SetRB()
        {
            rb = transform.parent.GetComponent<Rigidbody2D>();
            rb.mass += Weight;

            GetAttributes().MaxIntegrity = GetAttributes().Integrity;
        }

        /// <summary>
        /// Defines the triggers to 
        /// activate a component
        /// </summary>
        private void SetTriggers(string trigger, string combat)
        {
            if(trigger != null)
                GetAttributes().TriggerKey = Control_Config
                    .GetKey(trigger, "ship");

            if(combat != null)
                GetAttributes().CombatKey = Control_Config
                    .GetKey(combat, "combat");
        }

        /// <summary>
        /// Applies the rotation
        /// of the component to the transform
        /// </summary>
        private void SetRotation()
        {
            // Set the direction of the new piece
            Vector3 dirEuler = new Vector3(0, 0, 0);

            switch (Data.Direction)
            {
                case "up":
                    dirEuler.z = 0f; break;
                case "down":
                    dirEuler.z = 180f; break;
                case "left":
                    dirEuler.z = 90; break;
                case "right":
                    dirEuler.z = 270f; break;
            }

            // Apply direction to obj and sockets
            transform.eulerAngles = dirEuler;
        }

        /// <summary>
        /// Applies the location 
        /// of the component to the transform
        /// </summary>
        private void SetLocation()
        {
            // Heads arent connected and should be at center
            if (ConnectedID == NetworkInstanceId.Invalid)
            {
                transform.localPosition = Vector3.zero;
                return;
            }

            // Get position of this socket
            // through the components transform
            Vector3 socketPos = ConnectedObj.Find
                (String.Format
                 ("socket_{0}", Socket.SocketID)).position;

            // find position of other piece and then
            // snap the pieces together
            Vector3 compPos = transform.Find
                (String.Format
                 ("socket_{0}", Socket.OtherLinkID)).position;

            Vector3 snapDistance = compPos - socketPos;

            transform.position -= snapDistance;
        }

        /// <summary>
        /// creates the connected object list
        /// </summary>
        private void InitCL()
        {
            ComponentAttributes att = GetAttributes();

            att.connectedComponents = new
                System.Collections.Generic.List<ComponentListener>();

            // Add the local side components that 
            // are attached to this component
            foreach (uint cID in att.ConnectedIDs)
            {
                // Find the component in our local scene
                GameObject cGO = ClientScene.FindLocalObject
                    (new NetworkInstanceId(cID));

                if (cGO == null)
                {
                    Debug.Log("Component Listener - " +
                            "InitCL:" +
                              "Component not found locally - "
                              + cID);
                    continue;
                }

                // extract the component listener
                ComponentListener cListener = cGO.GetComponent
                    <ComponentListener>();

                if(cListener == null)
                {
                    Debug.Log("Component Listener - " +
                            "InitCL:" +
                              "ID doesnt belong to a component- "
                              + cID);
                    continue;
                }

                // finally we can add our component
                att.connectedComponents.Add(cListener);
            }
        }

        #endregion

        #endregion

        #region COROUTINES

        private IEnumerator FadeOut()
        {
            float alpha = 1.0f;

            while(GetComponentInChildren<SpriteRenderer>().color.a > 0)
            {
                GetComponentInChildren<SpriteRenderer>().color =
                    new Color(1.0f, 1.0f, 1.0f, alpha -= 0.03f);
                yield return null;
            }
        }

        private IEnumerator FadeIn()
        {
            float alpha = 0.0f;

            while (GetComponentInChildren<SpriteRenderer>().color.a < 1)
            {
                GetComponentInChildren<SpriteRenderer>().color =
                    new Color(1.0f, 1.0f, 1.0f, alpha += 0.03f);
                yield return null;
            }
        }

        #endregion
    }
}

