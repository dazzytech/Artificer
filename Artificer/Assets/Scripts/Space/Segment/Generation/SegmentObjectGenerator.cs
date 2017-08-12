using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Data.Space;

namespace Space.Segment.Generator
{
    public class SegmentObjectGenerator : MonoBehaviour
    {
        #region ATTRIBUTES

        // Prefabs
        public GameObject SatellitePrefab;

        #endregion

        #region GENERATION

        public GameObject GenerateSatellite(SegmentObjectData segObj)
        {
            GameObject newSatellite = Instantiate(SatellitePrefab);
            newSatellite.transform.position = segObj._position;

            NetworkServer.Spawn(newSatellite);

            return newSatellite;
        }

        #endregion
    }
}

