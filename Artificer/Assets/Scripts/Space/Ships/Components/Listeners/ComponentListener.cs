using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
// Artificer
using Data.Shared;
using Space.Segment;
using Space.Ship.Components.Attributes;

namespace Space.Ship.Components.Listener
{
    public class ComponentListener : NetworkBehaviour
    {
        #region ATTRIBUTES

        protected Rigidbody2D rb;
    	public float Weight;
        
        // Used by editor tools to determine type
        public string ComponentType;

        #region ACCESSORS

        public ComponentAttributes GetAttributes()
        {
            if (transform.GetComponent<ComponentAttributes>() != null)
                return transform.GetComponent<ComponentAttributes>();
            else
                return null;
        }

        public int ID
        {
            set
            {
                GetAttributes().ID = value;
            }
        }

        #endregion

        #endregion

        #region MONO BEHAVIOUR

        void Start()
    	{
            ComponentType = "Components";
            SetRB();
    	}

        #endregion

        #region INITIALZATION

        /// <summary>
        /// Initialise the physics component
        /// of the listener
        /// </summary>
        protected void SetRB()
        {
            rb = transform.parent.GetComponent<Rigidbody2D> ();
            rb.mass += Weight;

            GetAttributes().MaxIntegrity = GetAttributes().Integrity;
        }

        /// <summary>
        /// Keeps a reference to the ship it belongs 
        /// to for component configuration
        /// </summary>
        /// <param name="Ship"></param>
        public void SetShip(ShipAttributes Ship)
        {
            GetAttributes().ShipAtt = Ship;

            // If ship is already docked then we must hide it
            if (Ship.ShipDocked)
            {
                if (GetComponent<Collider2D>() != null)
                    GetComponent<Collider2D>().enabled = false;

                GetComponentInChildren<SpriteRenderer>().color =
                    new Color(1.0f, 1.0f, 1.0f, 0f);
            }
        }

        /// <summary>
        /// Initialized the connected object list
        /// </summary>
        public void InitCL()
        {
            ComponentAttributes att = GetAttributes();

            att.connectedComponents = new 
                System.Collections.Generic.List<ComponentListener>();
        }

        #endregion

        #region CONSTRUCTION

        /// <summary>
        /// Pass reference to other component 
        /// this component will attach to
        /// </summary>
        /// <param name="trans"></param>
        public void LockTo(Transform trans)
        {
            GetAttributes().LockedGO = trans;
        }

        /// <summary>
        /// Pass reference to connected sockets we
        /// will use to attach components
        /// </summary>
        /// <param name="sock"></param>
        public void SetSock(Socket sock)
        {
            GetAttributes().sockInfo = sock;
        }

        /// <summary>
        /// Snap this component to the component
        /// passed via reference with socket information
        /// </summary>
        public void FixToConnected()
        {
            if (GetAttributes().LockedGO != null &&
                GetAttributes().sockInfo.SocketID == -1)
            {               
                Vector3 otherPos = transform.Find 
                    (string.Format 
                     ("socket_{0}", GetAttributes().sockInfo.OtherLinkID)).position;
                
                Vector3 thisPos = 
                    GetAttributes().LockedGO.Find 
                        (string.Format 
                         ("socket_{0}", GetAttributes().sockInfo.SocketID)).position;
                
                Vector3 snapDistance = new Vector3();
                snapDistance.x = Math.Round(otherPos.x - thisPos.x, 2);
                snapDistance.y = Math.Round(otherPos.y - thisPos.y, 2);
                snapDistance.z = 0f;
                
                if(snapDistance.x != 0f || snapDistance.y != 0f)
                {
                    transform.position -= snapDistance;
                    transform.rotation = GetAttributes().LockedGO.rotation;
                }
            }
        }

        /// <summary>
        /// Adds the connected component to the list
        /// </summary>
        /// <param name="connected">Connected.</param>
        public void AddConnection(ComponentListener connected)
        {
            ComponentAttributes att = GetAttributes();
            
            att.connectedComponents.Add(connected);
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Applys damage from impact collider to the
        /// component
        /// </summary>
        /// <param name="hit"></param>
        public void DamageComponent(HitData hit)
        {
            ComponentAttributes att = GetAttributes();

            if (att.Shield != null)
            {
                // Bypass damage as shield absorbs damage
                att.Shield.Impact(hit);
                return;
            }

            att.Integrity -= hit.damage;

            SetColour(att.Integrity);

            if (att.Integrity <= 0)
            {
                hit.hitComponent = att.ID;
                transform.parent.gameObject.
                    SendMessage("DestroyComponent", hit,
                                SendMessageOptions.DontRequireReceiver);
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
            if(GetComponent<Collider2D>() != null)
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

        #region VIRTUAL FUNCTIONS

        public virtual void Activate(){}

    	public virtual void Deactivate(){}

        #endregion

    }
}

