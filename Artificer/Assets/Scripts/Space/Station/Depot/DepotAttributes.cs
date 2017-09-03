using Data.Space;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stations
{
    public class DepotAttributes : StationAttributes
    {
        /// <summary>
        /// whether or not the depot is currently 
        /// refining components
        /// </summary>
        public bool IsDepositing;

        /// <summary>
        /// How long each materials that can be 
        /// deposited takes to refine
        /// </summary>
        public Dictionary<int, float> DepositDelayLists;

        /// <summary>
        /// Materials within the ship assets to be
        /// deposited
        /// </summary>
        public WalletData PendingDeposit;
    }
}