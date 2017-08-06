using Data.Space.Collectable;
using Data.Space.Library;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data.Space
{
    /// <summary>
    /// Container for all assets
    /// including cash and items
    /// </summary>
    [System.Serializable]
    public struct WalletData
    {
        #region ATTRIBUTES

        public int m_currency;

        public ItemCollectionData[] m_inventory;

        #endregion

        #region ACCESSORS

        public int Currency
        {
            get { return m_currency; }
        }

        public ItemCollectionData[] Assets
        {
            get { return m_inventory; }
        }

        #endregion

        #region PUBLIC INTERACTION

        #region CURRENCY

        /// <summary>
        /// Adds money to account
        /// returns if transaction is 
        /// successful
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool Deposit(int amount)
        {
            if(amount > 0)
            {
                m_currency += amount;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempt to purchase, return
        /// true if successful
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool Purchase(int amount)
        {
            if (amount <= m_currency)
            {
                m_currency -= amount;
                return true;
            }

            return false;
        }

        #endregion

        #region ASSETS

        /// <summary>
        /// Places item within our assets
        /// returns if the transaction is
        /// successful
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool Deposit(int item, float amount)
        {
            if(amount > 0 
                && item != -1)
            {
                if (m_inventory == null)
                    m_inventory = new ItemCollectionData[0]; 

                // Discover if we already have this
                for(int i = 0; i < m_inventory.Length; i++)
                {
                    if(m_inventory[i].Item == item)
                    {
                        m_inventory[i].Amount += amount;
                        return true;
                    }
                }

                // create new item
                int index = IncrementAssets();
                m_inventory[index].Amount = amount;
                m_inventory[index].Item = item;
                m_inventory[index].Exist = true;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Decrements our asset amount by
        /// the specific amount
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool Withdraw(int item, float amount)
        {
            if (amount > 0
               && item != -1)
            {
                // Discover if we already have this
                for (int i = 0; i < m_inventory.Length; i++)
                {
                    if (m_inventory[i].Item == item)
                    {
                        if (m_inventory[i].Amount < amount)
                            return false;

                        m_inventory[i].Amount -= amount;
                        if (m_inventory[i].Amount == 0)
                            RemoveAsset(i);
                        // transaction successful
                        return true;
                    }
                }

                return false;
            }

            return false;
        }

        #endregion

        public void Clear()
        {
            m_currency = 0;

            m_inventory = new ItemCollectionData[0];
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Increments the asset list
        /// by one and returns the index
        /// of the next item
        /// </summary>
        /// <returns></returns>
        private int IncrementAssets()
        {
            int newIndex = m_inventory.Length;

            ItemCollectionData[] temp = new ItemCollectionData[newIndex + 1];

            for (int i = 0; i < newIndex; i++)
                temp[i] = m_inventory[i];

            m_inventory = temp;

            return newIndex;
        }

        /// <summary>
        /// deletes an asset from our list
        /// </summary>
        /// <param name="index"></param>
        private void RemoveAsset(int index)
        {
            ItemCollectionData[] temp = new ItemCollectionData[m_inventory.Length - 1];

            for (int i = 0; i < temp.Length; i++)
                if(i != index)
                    temp[i] = m_inventory[i];

            m_inventory = temp;
        }

        #endregion
    }
}