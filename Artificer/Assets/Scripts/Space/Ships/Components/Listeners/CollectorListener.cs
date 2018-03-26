using UnityEngine;
using System.Collections;

using Space.Ship.Components.Attributes;
using System.Collections.Generic;

namespace Space.Ship.Components.Listener
{
    public class CollectorListener : ComponentListener
    {
        #region ATTRIBUTES

        CollectorAttributes _attr;

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Creates an amount of item gathered
        /// and attempts to distribute them in storage
        /// </summary>
        /// <param name="ItemIndex"></param>
        public void ItemGathered(int ItemIndex, float amount)
        {
            // Create the dictionary item 
            // for storage
            Dictionary<int, float> yield = new Dictionary<int, float> {
                { ItemIndex, amount == -1? Random.Range(_attr.YieldModifierMin, _attr.YieldModifierMax): amount } };

            // first time only add the item to 
            // storages that already have it
            // for stacking
            foreach(StorageListener storage in _attr.Ship.Storage)
            {
                if (storage.ContainsItem(ItemIndex))
                    yield = storage.AddMaterial(yield);

                if (yield.Count == 0)
                    break;
            }

            // next if we still have the item
            // we add it to a new storage
            if (yield.Count > 0)
            {
                foreach (StorageListener storage in _attr.Ship.Storage)
                {
                    yield = storage.AddMaterial(yield);

                    if (yield.Count == 0)
                        break;
                }
            }
        }

        #endregion

        #region PRIVATE UTILITIES

        protected override void InitializeComponent()
        {
            base.InitializeComponent();

            ComponentType = "Collectors";
            _attr = GetComponent<CollectorAttributes>();
        }

        #endregion
    }
}
