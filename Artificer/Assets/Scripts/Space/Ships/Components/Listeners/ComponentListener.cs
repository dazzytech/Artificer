using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
// Artificer
using Data.Shared;
using Space;

namespace ShipComponents
{
    public class ComponentListener : NetworkBehaviour
    {
    	protected Rigidbody2D rb;
    	public float Weight;
        public float TotalHP;

        // Used by editor tools to determine type
        public string ComponentType;

    	void Start()
    	{
            ComponentType = "Components";
            SetRB();
    	}

        public void SetRB()
        {
            rb = transform.parent.GetComponent<Rigidbody2D> ();
            rb.mass += Weight;

            TotalHP = GetAttributes().Integrity;
        }

        /// <summary>
        /// Keeps a reference to the ship it belongs 
        /// to for component configuration
        /// </summary>
        /// <param name="Ship"></param>
        public void SetShip(ShipAttributes Ship)
        {
            GetAttributes().ShipAtt = Ship;
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

        public void SetID(int ID)
        {
            GetAttributes().ID = ID;
        }

        public void LockTo(Transform trans)
        {
            GetAttributes().LockedGO = trans;
        }

        public void SetSock(Socket sock)
        {
            GetAttributes().sockInfo = sock;
        }

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

        public virtual void Activate(){}

    	public virtual void Deactivate(){}

        public ComponentAttributes GetAttributes()
        {
            if (transform.GetComponent<ComponentAttributes>() != null)
                return transform.GetComponent<ComponentAttributes>();
            else
                return null;
        }

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

        public void SetColour(float integrity)
        {
            // Change color to show damage for now
            float brightness = (integrity / TotalHP) * 0.5f;

            GetComponentInChildren<SpriteRenderer>().color =
                new Color(0.5f + brightness, 0.5f + brightness, 0.5f + brightness);
        }

        public virtual void Destroy()
        {
            // will be important when able to loot, and stop locking
            SetRB();
            this.enabled = false;
        }
    }
}

