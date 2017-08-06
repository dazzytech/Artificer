using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class that details 
/// an item that can be stored
/// </summary>
namespace Data.Space.Collectable
{
    /// <summary>
    /// Singular list object that 
    /// stores the item alongside its owned quantity
    /// </summary>
    [System.Serializable]
    public struct ItemCollectionData
    {
        /// <summary>
        /// Reference to the item
        /// we have stored
        /// </summary>
        public int Item;
        /// <summary>
        /// Quantity this is our possession
        /// </summary>
        public float Amount;
        /// <summary>
        /// Due to item unable
        /// to be null, this bool
        /// is true when the object
        /// is created
        /// </summary>
        public bool Exist;
    }

    public class ItemData : IndexedObject
    {
        #region ITEM DESCRIPTION

        public string Name;             // e.g. "Au"
        public string Description;      // e.g. "Gold"
        public float Density;           // 547 kg per cubic foot (ft^3)
        public int Value;             // Standard currency value per cubic foot

        #endregion
    }
}