using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

// Artificer
using Data.Shared;
using Data.Space;
using Data.Space.Library;
using Space.Segment.Generator;

namespace Space.Segment
{
    /// <summary>
    /// Segment generator.
    /// builds randomized, none esstenial space objects e.g. asteroids, debris, planets
    /// TODO: STORE SEGMENT DATA AND START COROUTINE
    /// TO BUILD OBJECTS THAT COME WITHIN RANGE
    /// </summary>
    public class SegmentGenerator: NetworkBehaviour
    {

        #region ATTRIBUTES

        // Generator Objects
        public ShipGenerator _shipGen;
        public AsteroidGenerator _asteroidGen;
        public SingleObjectGenerator _segmentObjectGen;
        public SpawnPointGenerator _spawnGen;
        public NodeGenerator _nodeGen;

        // default spawn distances
        public float defaultSpawnDistance;
        public float stationSpawnDistance;
        public float asteroidSpawnDistance;

        private SegmentAttributes _att;

        private List<SegmentObject> SegObjs;

        public GameObject SatellitePrefab;

        #endregion

        #region MONOGAME

        void OnEnable()
        {
            //SegmentObjectBehaviour.Destroyed += RemoveObject;
        }

        void OnDisable()
        {
            //SegmentObjectBehaviour.Destroyed -= RemoveObject;
        }

        void Awake()
        {
            _att = GetComponent<SegmentAttributes>();
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Initializes the segment generator
        /// </summary>
        public void GenerateBase()
        {
            SegObjs = new List<SegmentObject>();

            // Build camera
            BuildCamera();

            StartCoroutine("BuildSegment");
        }

        /// <summary>
        /// Generates the segment.
        /// Starts a process of cycling through obj spawns
        /// </summary>
        /// <param name="segment">Segment.</param>
        public void StartSegmentCycle()
        {
            // Start new coroutine with new segment
            //StartCoroutine("CycleSegment");
        }

        #endregion

        #region UTILITIES

        private void BuildCamera()
        {
            // ASSIGN MAIN CAMERA
            if (Camera.main == null)
            {
                // create the game object for the camera
                GameObject camObject = new GameObject();
                //camObject.transform.position = GOToFollow.transform.position;
                camObject.transform.Translate(new Vector3(0,0, -10f));
                camObject.transform.parent = this.transform.parent;
                camObject.name = "PlayerCamera";
                camObject.tag = "MainCamera";
                
                // create and format the camera
                Camera playerCam = camObject.AddComponent<Camera> ();
                playerCam.clearFlags = CameraClearFlags.Color;
                playerCam.backgroundColor = Color.black;
                playerCam.orthographic = true;
                playerCam.orthographicSize = 20f;
                
                // add and initialize scripts for the camera object
                CameraFollow camFollow = 
                    camObject.AddComponent<CameraFollow>();
                camObject.AddComponent<CameraShake>();
                
                camObject.AddComponent<AudioListener>();

                BuildStarfield(camObject.transform);
            }
        }

        private void BuildStarfield(Transform cam)
        {
            // create objects that follow player
            GameObject starField = (GameObject)Instantiate (Resources.Load ("Space/starfield")
                                                            , Vector3.zero, Quaternion.identity);
            starField.transform.parent = cam;

            float screenAspect = (float)Screen.width / (float)Screen.height;
            float cameraHeight = Camera.main.orthographicSize * 2;
            GameObject starDrop = (GameObject)Instantiate (Resources.Load ("Space/Backdrops/stardrop_0")
                                                           , Vector3.zero, Quaternion.identity);
            Vector3 size = new Vector3(cameraHeight * screenAspect, cameraHeight, 0f);
            starDrop.transform.localScale = size;
            starDrop.transform.parent = cam;
            starDrop.transform.Translate (new Vector3 (0f, 0f, 1f));
        }

        #endregion

        #region COROUTINES

        private IEnumerator BuildSegment()
        {
            foreach (SegmentObject segObj
                         in _att.SegObjs)
            {
                switch (segObj._type)
                {
                    case "asteroid":
                        // Build the game object in generator
                        GameObject field = _asteroidGen.GenerateField
                                (segObj);

                        // spawn on network? consider children


                        // disable the object by default
                        field.SetActive(false);
                        break;
                    case "satellite":
                        // Build the game object in generator
                        GameObject newSatellite = _segmentObjectGen.GenerateSatellite
                                (segObj, SatellitePrefab);

                        NetworkServer.Spawn(newSatellite);

                        // assign network inst id
                        break;
                }

                yield return null;
            }

            yield return null;
        }

        /// <summary>
        /// Cycles the segment.
        /// enables objects within range
        /// </summary>
        /// <returns>The segment.</returns>
        /// <param name="segment">Segment.</param>
        private IEnumerator CycleSegment()
        {
            // start an infinate loop
            for (;;)
            {
                GameObject player = GameObject.FindGameObjectWithTag 
                    ("PlayerShip");
                
                if(player == null)
                    break;
                
                Vector3 playerPos = player.transform.position;

                /*foreach (SegmentObject planet
                         in _att.SegObjs.GetObjsOfType("planet"))
                {
                    // Spawn only within distance
                   // if (Vector3.Distance(playerPos, planet._position)
                      // < defaultSpawnDistance)
                   // {
                        // Only spawn if not currently exists within space
                        if (!SegObjs.Contains(planet))
                        {
                            GameObject newPlanet = _segmentObjectGen.GeneratePlanet
                                (planet);

                            // Add attribute scripts
                            //SegmentObjectBehaviour obj = newSatellite.AddComponent<SegmentObjectBehaviour>();
                            //obj.Create(defaultSpawnDistance, satellite);
                            SegObjs.Add(planet);
                        }
                   // }
                    yield return null;
                }*/

                // Generate segments asteroids
                foreach (SegmentObject asteroid
                         in _att.SegObjs.GetObjsOfType("asteroid")) 
                {
                    // Spawn only within distance
                    if(Vector3.Distance(playerPos, asteroid._position)
                       < defaultSpawnDistance)
                    {
                        // Only spawn if not currently exists within space
                        if(!SegObjs.Contains(asteroid))
                        {
                            GameObject field = _asteroidGen.GenerateField
                                (asteroid);

                            SegmentObjectBehaviour obj = field.AddComponent<SegmentObjectBehaviour>();
                            //obj.Create(asteroidSpawnDistance, asteroid);
                            SegObjs.Add(asteroid);
                        }
                    }
                    yield return null;
                }
                
                foreach (SegmentObject satellite
                         in _att.SegObjs.GetObjsOfType("satellite")) 
                {
                    // Spawn only within distance
                    if(Vector3.Distance(playerPos, satellite._position)
                       < defaultSpawnDistance)
                    {
                        // Only spawn if not currently exists within space
                        if(!SegObjs.Contains(satellite))
                        {
                            //GameObject newSatellite = _segmentObjectGen.GenerateSatellite
                                //(satellite, );

                            // Add attribute scripts
                            //SegmentObjectBehaviour obj = newSatellite.AddComponent<SegmentObjectBehaviour>();
                            //obj.Create(defaultSpawnDistance, satellite);
                            SegObjs.Add(satellite);
                        }
                    }
                    yield return null;
                }
                
                yield return null;
            }
        }

        /// <summary>
        /// Removes the object.
        /// called by segment object when destoryed
        /// </summary>
        /// <param name="obj">Object.</param>
        /*public void RemoveObject(SegmentObject obj)
        {
            if (SegObjs.Contains(obj))
                SegObjs.Remove(obj);
        }*/

        #endregion
    }
}
