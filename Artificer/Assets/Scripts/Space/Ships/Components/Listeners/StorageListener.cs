using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Space;
using Space.UI;
using Space.Ship.Components.Attributes;
using Data.Space.Collectable;

namespace Space.Ship.Components.Listener
{
    /// <summary>
    /// Storage listener.
    /// additional listener
    /// </summary>
    public class StorageListener : ComponentListener
    {
        #region ATTRIBUTES

        StorageAttributes _att;
        float totalWeight;

        #endregion

        #region ACCESSORS

        /// <summary>
        /// Total storage space
        /// </summary>
        public float Capacity
        {
            get { return _att.dimensions; }
        }

        /// <summary>
        /// Space used by current materials
        /// </summary>
        public float Used
        {
            get { return _att.currentCapacity; }
        }

        public Dictionary<int, float> Materials
        {
            get
            {
                return _att.storage;
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Discovers if this item 
        /// currently stores the material
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool ContainsItem(int item)
        {
            if (_att.storage == null)
                return false;

            else
                return _att.storage.ContainsKey(item);
        }

        /// <summary>
        /// Adds the item to the storage attributes.
        /// </summary>
        /// <returns>The material remainder.</returns>
        /// <param name="import">Import.</param>
        public Dictionary<int, float> AddMaterial(Dictionary<int, float> import)
        {
            if (_att.storage == null)
                _att.storage = new Dictionary<int, float>();

            Dictionary<int, float> remainder
                = new Dictionary<int, float>();

            foreach (int id in import.Keys)
            {
                float totalDimension = import[id] + _att.currentCapacity;
                float dimensionRemainder =                               // overspill of capacity
                    Mathf.Max(totalDimension - _att.dimensions, 0);        

                float valueToAdd = import[id] - dimensionRemainder;

                // retrieve information
                ItemData item = SystemManager.Items.Item(id);

                if (item == null)
                    continue;

                _att.currentWeight += valueToAdd * item.Density;
                _att.currentCapacity += valueToAdd;

                if(_att.storage.ContainsKey(id))
                    _att.storage[id] += valueToAdd;                   // add amount to current supply
                else if(valueToAdd > 0)
                    _att.storage.Add(id, valueToAdd);

                if( dimensionRemainder != 0)
                {
                    remainder.Add(id, dimensionRemainder);
                }
            }

            SetShipWeight();

            return remainder;
        }

        /// <summary>
        /// Ejects the material.
        /// Removes amount specified and then
        /// returns that exact amount for creating 
        /// a lootable object
        /// </summary>
        /// <returns>The material.</returns>
        /// <param name="eject">Import.</param>
        public Dictionary<int, float> EjectMaterial(Dictionary<int, float> eject)
        {
            if (_att.storage == null)
                return null;

            Dictionary<int, float> ejectedMat = new Dictionary<int, float>();

            if (eject == null)
                ejectedMat = _att.storage;
            else
            {
                foreach (int id in eject.Keys)
                {
                    if (_att.storage.ContainsKey(id))
                    {
                        float amountToRemove = eject[id];
                        float spillOver = Mathf.Max(amountToRemove - _att.storage[id], 0);
                        amountToRemove -= spillOver;

                        _att.storage[id] -= amountToRemove;

                        if (_att.storage[id] <= 0.001f)
                            _att.storage.Remove(id);

                        // retrieve information
                        ItemData item = SystemManager.Items.Item(id);

                        // remove weight
                        _att.currentWeight -= amountToRemove * item.Density;
                        _att.currentCapacity -= amountToRemove;

                        if (spillOver >= 0)
                            ejectedMat.Add(id, spillOver);
                    }
                }
            }

            return ejectedMat;
        }

        #endregion

        #region PRIVATE UTILITIES

        protected override void InitializeComponent()
        {
            base.InitializeComponent();

            ComponentType = "Storage";

            _att = GetComponent<StorageAttributes>();
            _att.storage = new Dictionary<int, float>();

            if (hasAuthority)
                totalWeight = 0;
        }

        private void SetShipWeight()
        {
            rb.mass -= totalWeight;
            totalWeight = _att.currentWeight * 0.000001f;
            rb.mass += totalWeight;
        }

        #endregion
    }
}