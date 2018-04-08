using Data.Space.Collectable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Space.Segment
{
    /// <summary>
    /// Destroyed station objects that allow 
    /// players to loot 
    /// </summary>
    public class DestroyedStation : Lootable
    {
        #region PUBLIC INTERACTION

        [Server]
        public void SetWreckage(Sprite sprite, ItemCollectionData[] loot)
        {
            GetComponent<SpriteRenderer>().sprite = sprite;

            m_yield = new Dictionary<int, float>();

            // Convert to dictionary so that it can be stored in a ship
            if(loot != null)
                foreach(ItemCollectionData item in loot)
                    if(item.Exist)
                        m_yield.Add(item.Item, item.Amount);
                
            Initialize();
        }

        #endregion

    }
}