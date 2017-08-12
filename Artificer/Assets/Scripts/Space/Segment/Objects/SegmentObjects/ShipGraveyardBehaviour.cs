using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Data.Space;

namespace Space.Segment
{
    /// <summary>
    /// Creates a collection of wrecked
    /// ships to the server when started on a 
    /// ship graveyard on the server
    /// </summary>
    public class ShipGraveyardBehaviour : SegmentObjectBehaviour
    {
        public GameObject[] WreckagePrefabs;

        private void Awake()
        {
            WreckagePrefabs
            = new GameObject[2]
            { Resources.Load("Space/Destructable/Wreckage_01", typeof(GameObject)) as GameObject,
              Resources.Load("Space/Destructable/Wreckage_02", typeof(GameObject)) as GameObject };
        }

        #region WRECKAGE GENERATOR

        /// <summary>
        /// Builds and spawns a prefab of wreckage
        /// in the given position and initializes it 
        /// </summary>
        /// <param name="wData"></param>
        /// <param name="position"></param>
        [Server]
        protected override void BuildObject()
        {
            // Init and set parent of object
            GameObject subObj = Instantiate
                (WreckagePrefabs[Random.Range(0, WreckagePrefabs.Length)]);
            subObj.transform.parent = transform;

            // Give a random location and size;
            Vector2 location = new Vector2
                (Random.Range(0f, m_segObject._size.x),
                 Random.Range(0f, m_segObject._size.y));

            // If we have a bounds then keep within them
            if (m_segObject._border != null)
                while (!Math.IsPointInPolygon
                   (m_segObject._position + location, m_segObject._border))
                    location = new Vector2
                        (Random.Range(0f, m_segObject._size.x),
                         Random.Range(0f, m_segObject._size.y));

            // position with our parameter
            subObj.transform.localPosition = location;
            subObj.transform.Rotate(0,0, Random.Range(0, 360));

            // Spawn on network and init object
            NetworkServer.Spawn(subObj);
            subObj.GetComponent<SegmentObject>().
                InitializeParameters(netId);
        }

        #endregion
    }
}
