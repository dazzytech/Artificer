using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Space.Segment
{
    /// <summary>
    /// Used for lootable objects
    /// </summary>
    public class Collectable : Debris
    {
        private int m_itemYield;

        // Use this for initialization
        void Start()
        {
            Invoke("Die", 40f);
        }

        /// <summary>
        /// Sends message to both ship and space 
        /// alerting to item pickup if 
        /// colliding with player ship.
        /// </summary>
        /// <param name="col"></param>
        void OnTriggerEnter2D(Collider2D col)
        {
            //if (col.transform.parent == null)
            return;
            /*if (col.transform.parent.tag == "PlayerShip")
            {
                // discover a way to pass loot info
                // class?
                SystemManager.Space.SendMessage
                    ("ItemCollected", m_itemYield,
                    SendMessageOptions.RequireReceiver);

                Destroy(this.gameObject);
            }*/
        }
    }
}

