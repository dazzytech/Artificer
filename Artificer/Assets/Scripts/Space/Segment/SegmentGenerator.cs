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
    public class SegmentGenerator: MonoBehaviour
    {

        #region ATTRIBUTES

        // Generator Objects
        public ShipGenerator _shipGen;
        public StationGenerator _stationGen;
        public AsteroidGenerator _asteroidGen;
        public SatelliteGenerator _satelliteGen;
        public SpawnPointGenerator _spawnGen;
        public NodeGenerator _nodeGen;

        // default spawn distances
        public float defaultSpawnDistance;
        public float stationSpawnDistance;
        public float asteroidSpawnDistance;

        // Store list of segments object currently active
        // Dependant on client
        //private List<SegmentObject> SegObjs;

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

        #endregion

        #region GENERATION

        /// <summary>
        /// Initializes the segment generator
        /// </summary>
        public void GenerateBase()
        {
            // First time running
            //SegObjs = new List<SegmentObject>();

            // Build camera
            BuildCamera();
        }

        /// <summary>
        /// Generates the segment.
        /// Starts a process of cycling through obj spawns
        /// </summary>
        /// <param name="segment">Segment.</param>
        /*public void GenerateSegment(SyncListSO segment)
        {
            // Start new coroutine with new segment
            StartCoroutine("CycleSegment", segment);
        }*/

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

        #region SEGMENT MANAGEMENT

        /// <summary>
        /// Cycles the segment.
        /// Constructing objects within range
        /// </summary>
        /// <returns>The segment.</returns>
        /// <param name="segment">Segment.</param>
        
        /*private IEnumerator CycleSegment(SyncListSO segment)
        {
            // start an infinate loop
            for (;;)
            {
                GameObject player = GameObject.FindGameObjectWithTag 
                    ("PlayerShip");
                
                if(player == null)
                    break;
                
                Vector3 playerPos = player.transform.position;

                // Generate segments asteroids
                /*foreach (SegmentObject asteroid
                         in segment.GetObjsOfType("asteroid")) 
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
                            obj.Create(asteroidSpawnDistance, asteroid);
                            SegObjs.Add(asteroid);
                        }
                    }
                    yield return null;
                }
                
                foreach (SegmentObject satellite
                         in segment.GetObjsOfType("satellite")) 
                {
                    // Spawn only within distance
                    if(Vector3.Distance(playerPos, satellite._position)
                       < defaultSpawnDistance)
                    {
                        // Only spawn if not currently exists within space
                        if(!SegObjs.Contains(satellite))
                        {
                            GameObject newSatellite = _satelliteGen.GenerateSatellite
                                (satellite);

                            // Add attribute scripts
                            SegmentObjectBehaviour obj = newSatellite.AddComponent<SegmentObjectBehaviour>();
                            obj.Create(defaultSpawnDistance, satellite);
                            SegObjs.Add(satellite);
                        }
                    }
                    yield return null;
                }
                
                yield return null;
            }
        }*/

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
