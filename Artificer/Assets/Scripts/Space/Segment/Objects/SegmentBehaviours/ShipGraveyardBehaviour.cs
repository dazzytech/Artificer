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
    public class ShipGraveyardBehaviour : NetworkBehaviour
    {
        private static ShipData[] wreckageCollection;

        public GameObject WreckagePrefab;

        #region MONO BEHAVIOUR 

        // Use this for initialization
        void Start()
        {
            if (isServer)
            {
                // Import our destroyed list
                wreckageCollection = ShipDataImporter.LoadShipDataWreckage();
                // build up this graveyard
                BuildGraveyard();
            }
        }

        #endregion

        #region WRECKAGE GENERATOR

        /// <summary>
        /// Generates a number of ships
        /// picks a random position and 
        /// builds a full ship in that location
        /// with a chance if breaking the ship
        /// into different components
        /// </summary>
        private void BuildGraveyard()
        {
            // Pick number of ships to generate
            int numWreckedShips = Random.Range(50, 200);

            // width and height of our ship graveyard
            int width = Random.Range(500, 1000);
            int height = Random.Range(500, 1000);

            // loop through each wrecked ship component
            for (int i = 0; i < numWreckedShips; i++)
            {
                // container to wreckage we are currently adding wreckage to
                WreckageData current = new WreckageData();

                // select a ship we will make into a wreckage
                ShipData shipwreck = wreckageCollection[Random.Range(0, wreckageCollection.Length)];

                // pick a posititon to spawn
                Vector2 position = new Vector2(Random.Range(-width * .5f, width*.5f),
                    Random.Range(-height * .5f, height * .5f));

                // Begin the recursive process of building the destroyed ship
                AddWreckedComponent(ref current, shipwreck.Head, 0, position, shipwreck, new List<int>());

                // This should now be our base wreckage
                BuildWreck(current, position);
            }
        }

        /// <summary>
        /// recurring loop that adds component to
        /// wreckage data then has a chance of adding connected peice to 
        /// same wreckage to passing in a new wreckage
        /// </summary>
        /// <param name="wreck"></param>
        private void AddWreckedComponent(ref WreckageData wreck, Data.Shared.Component wreckComp,
            int iteration, Vector2 basePos, ShipData shipwreck, List<int> addedComps)
        {
            // return if we have gone in a loop
            if (addedComps.Contains(wreckComp.InstanceID))
                return;

            // Add head to components
            wreck.AddComponent(wreckComp);

            // Add to our list to avoid re-adding
            addedComps.Add(wreckComp.InstanceID);

            // iterate through each of the sockets connected to the component
            foreach (Data.Shared.Socket sock in wreckComp.sockets)
            {
                // chance of breaking component off increases each iteration
                if(Random.Range(0, (Mathf.Max(4, 10 - iteration)))
                    == (Mathf.Min(10, iteration)) && iteration != 0)
                {
                    // Break the component into another component
                    WreckageData brokeWreckage = new WreckageData();

                    // Position nearby
                    Vector2 newPos = new Vector2(
                        Random.Range(basePos.x - 5, basePos.x + 5),
                        Random.Range(basePos.y - 5, basePos.y + 5));

                    // Create broken off comp restarting iterations
                    AddWreckedComponent(ref brokeWreckage, shipwreck.GetComponent(sock.OtherID),
                    0, newPos, shipwreck, addedComps);

                    // add wreck to server
                    BuildWreck(brokeWreckage, newPos);
                }

                // if still connected then just connect the component and reincurr component creation
                wreck.AddSocket(wreckComp.InstanceID,
                    sock.SocketID, sock.OtherLinkID, sock.OtherID);

                AddWreckedComponent(ref wreck, shipwreck.GetComponent(sock.OtherID),
                    ++iteration, basePos, shipwreck, addedComps);
            }
        }

        /// <summary>
        /// Builds and spawns a prefab of wreckage
        /// in the given position and initializes it 
        /// to build the wreckage components
        /// </summary>
        /// <param name="wData"></param>
        /// <param name="position"></param>
        private void BuildWreck(WreckageData wData, Vector3 position)
        {
            GameObject newWreckage = Instantiate(WreckagePrefab);
            newWreckage.transform.parent = this.transform;
            newWreckage.transform.localPosition = position;
            newWreckage.transform.Rotate(0,0, Random.Range(0, 360));

            NetworkServer.Spawn(newWreckage);
            newWreckage.GetComponent<WreckageBehaviour>().
                InitializeWreckage(wData, this.netId);
        }

        #endregion
    }
}
