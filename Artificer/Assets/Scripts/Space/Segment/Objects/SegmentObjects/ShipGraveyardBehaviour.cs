using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

using Data.Shared;
using Data.Space;
using Data.Space.DataImporter;

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

        #region MONO BEHAVIOUR 

        // Use this for initialization
        protected override void Start()
        {
            if (isServer)
            {
                // build up this graveyard
                BuildGraveyard();
            }

            base.Start();
        }

        #endregion

        #region WRECKAGE GENERATOR

        /// <summary>
        /// Generates a number of ships
        /// picks a random position and 
        /// builds a random wreckage prefab into
        /// that location
        /// </summary>
        private void BuildGraveyard()
        {
            // Pick number of ships to generate
            int numWreckedShips = Random.Range(10, 50);

            // width and height of our ship graveyard
            int width = Random.Range(20, 100);
            int height = Random.Range(20, 100);

            // loop through each wrecked ship component
            for (int i = 0; i < numWreckedShips; i++)
            {
                // pick a posititon to spawn
                Vector2 position = new Vector2(Random.Range(-width * .5f, width*.5f),
                    Random.Range(-height * .5f, height * .5f));

                // pick a random wreckage prefab
                GameObject wreckagePrefab = WreckagePrefabs[Random.Range(0, WreckagePrefabs.Length)];

                // This should now be our base wreckage
                BuildWreck(wreckagePrefab, position);
            }
        }

        /// <summary>
        /// Builds and spawns a prefab of wreckage
        /// in the given position and initializes it 
        /// </summary>
        /// <param name="wData"></param>
        /// <param name="position"></param>
        private void BuildWreck(GameObject wreckagePrefab, Vector3 position)
        {
            GameObject newWreckage = Instantiate(wreckagePrefab);
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
