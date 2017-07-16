using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

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

        #region WRECKAGE GENERATOR

        /// <summary>
        /// Builds and spawns a prefab of wreckage
        /// in the given position and initializes it 
        /// </summary>
        /// <param name="wData"></param>
        /// <param name="position"></param>
        private void BuildWreck(Vector2 position)
        {
            GameObject newWreckage = Instantiate(WreckagePrefabs[Random.Range(0, WreckagePrefabs.Length)]);
            newWreckage.transform.parent = this.transform;
            newWreckage.transform.localPosition = position;
            newWreckage.transform.Rotate(0,0, Random.Range(0, 360));

            NetworkServer.Spawn(newWreckage);
            newWreckage.GetComponent<WreckageBehaviour>().
                InitializeParameters(this.netId);
        }

        #endregion
    }
}
