using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

//Artificer
using Data.Space.Library;
using Space.Segment;
using Space.Ship;
using Space.Ship.Components.Listener;
using System.Collections.Generic;

namespace Space.Segment
{
    public class DestroyedComponent : Lootable
    {
        #region MONOBEHAVIOUR

        private void Start()
        {
            m_maxDensity = m_pieceDensity = (40f * transform.childCount);
        }

        #endregion

        #region PUBLIC INTERACTION

        #region SERVER MESSAGES

        /// <summary>
        /// Receives parameters from server to
        /// initialize debris and calls client rpc
        /// </summary>
        /// <param name="components"></param>
        /// <param name="playerID"></param>
        [Server]
        public void SetWreckage(int[] components, NetworkInstanceId playerID)
        {
            RpcBuildWreckage(components, playerID);
        }

        #endregion

        #region DEBRIS INITIALIZATION

        /// <summary>
        /// Builds wreckage on each clients using 
        /// parameters. Retieve components and activate them
        /// and place them as wreckage. if ship has no components left
        /// destroy it.
        /// </summary>
        /// <param name="components"></param>
        /// <param name="playerID"></param>
        [ClientRpc]
        public void RpcBuildWreckage(int[] components, NetworkInstanceId shipID)
        {
            // Retrieve the Game Object to the destroyed ship
            GameObject GO = ClientScene.FindLocalObject
                (shipID);

            if (GO == null)
                return;

            m_yield = new Dictionary<int, float>();

            // Access the accessor for the ship
            ShipAccessor ship = GO.GetComponent<ShipAccessor>();

            if (ship == null)
                return;

            // We need the rb for this object to manipulate mass
            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            // loop through each component and add to this wreckage GO
            foreach (ComponentListener listener in ship.SelectedComponents(components))
            {
                // determine if the component that is destroyed was a storage component
                if (listener is StorageListener)
                {
                    // Release a number on cargo items from storage
                    Dictionary<int, float> yield = ((StorageListener)listener).EjectMaterial(null);

                    // Reduce storage to a portion
                    foreach(int index in yield.Keys)
                    {
                        float reduce = yield[index];
                        reduce *= Random.Range(0.3f, 0.6f);
                        m_yield.Add(index, reduce);
                    }
                }

                listener.Destroy();
                listener.transform.parent = this.transform;
                rb.mass += listener.Weight;
                foreach (Collider2D col in listener.transform.GetComponents<Collider2D>())
                    col.enabled = false;
            }

            // Destroy original ship if no more components remain
            if (ship.Components.Length == 0)
                Destroy(GO);

            m_maxDensity = m_pieceDensity = (40f * transform.childCount);

            Initialize();
        }

        #endregion

        #endregion
    }
}

