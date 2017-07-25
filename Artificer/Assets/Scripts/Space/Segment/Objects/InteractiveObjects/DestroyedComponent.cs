using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

//Artificer
using Data.Space.Library;
using Space.Segment;
using Space.Ship;
using Space.Ship.Components.Listener;

namespace Space.Segment
{
    public class DestroyedComponent : SegmentObject
    {
        #region ATTRIBUTES

        private float m_secondsTillRemove = 60f;

        #endregion

        #region MONOBEHAVIOUR

        void Start()
        {
            m_maxDensity = m_pieceDensity = (40f * transform.childCount);
        }

        void Update()
        {
           m_secondsTillRemove -= Time.deltaTime;
           if (m_secondsTillRemove <= 0)
               Destroy(gameObject);
        }

        #endregion

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
        public void RpcBuildWreckage(int[] components, NetworkInstanceId playerID)
        {
            GameObject player = ClientScene.FindLocalObject
                (playerID);

            if (player == null)
                return;

            ShipAccessor ship = player.GetComponent<ShipAccessor>();

            if (ship == null)
                return;

            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            foreach (ComponentListener listener in ship.SelectedComponents(components))
            {
                listener.Destroy();
                listener.transform.parent = this.transform;
                rb.mass += listener.Weight;
            }

            if (ship.Components.Length == 0)
                Destroy(player);

            m_maxDensity = m_pieceDensity = (40f * transform.childCount);
        }

        #endregion
    }
}

