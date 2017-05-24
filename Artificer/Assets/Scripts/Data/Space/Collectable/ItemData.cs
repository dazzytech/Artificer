using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class that details 
/// an item that can be stored
/// </summary>
namespace Data.Space.Collectable
{
    public class ItemData : IndexedObject
    {
        #region ITEM DESCRIPTION

        public string Name;            // e.g. "Gold"
        public string Description;      // e.g. "Conductive unresponsive metal
        public float Density;            // 547 kg per cubic foot (ft^3)

        #endregion
    }
}