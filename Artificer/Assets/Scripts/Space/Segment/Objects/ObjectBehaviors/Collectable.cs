using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;
using Data.Space.Library;

public class Collectable : MonoBehaviour
{
    private int m_itemYield;

    float destroyTime = 40f;

    // Use this for initialization
    void Start()
    {
        Invoke("Die", destroyTime);
    }

    public void InitializeCollectable(int itemID = -1)
    {
        m_itemYield = itemID;
    }

    /// <summary>
    /// Sends message to both ship and space 
    /// alerting to item pickup if 
    /// colliding with player ship.
    /// </summary>
    /// <param name="col"></param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.parent.tag == "PlayerShip")
        {
            SystemManager.Space.SendMessage
                ("ItemCollected", m_itemYield,
                SendMessageOptions.RequireReceiver);

            Destroy(this.gameObject);
        }
    }

    void Die()
    {
        Destroy(this.gameObject);
    }
}

