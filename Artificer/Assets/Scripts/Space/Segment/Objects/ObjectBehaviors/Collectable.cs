using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

        gameObject.layer = LayerMask.NameToLayer("Collectable");
        GetComponent<SpriteRenderer>().sortingLayerName = "BackgroundObjects";
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
        if (col.transform.parent.tag == "PlayerShip")
        {
            // discover a way to pass loot info
            // class?
            /*SystemManager.Space.SendMessage
                ("ItemCollected", m_itemYield,
                SendMessageOptions.RequireReceiver);*/

            Destroy(this.gameObject);
        }
    }

    void Die()
    {
        Destroy(this.gameObject);
    }
}

