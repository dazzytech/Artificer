using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

// Artificer
using Data.Space;
using Data.Space.Library;
using Space.Segment.Generator;
using Space.CameraUtils;

namespace Space.Segment
{
    /// <summary>
    /// Segment generator.
    /// builds randomized, none esstenial space objects e.g. asteroids, debris, planets
    /// TODO: STORE SEGMENT DATA AND START COROUTINE
    /// TO BUILD OBJECTS THAT COME WITHIN RANGE
    /// </summary>
    public class SegmentObjectManager: NetworkBehaviour
    {
        #region ATTRIBUTES

        #region PREFABS

        public GameObject SatellitePrefab;
        public GameObject CloudPrefab;
        public GameObject AsteroidPrefab;
        public GameObject GravePrefab;

        #endregion

        [SerializeField]
        private SegmentObjectGenerator m_objectGenerator;

        [SerializeField]
        private SegmentAttributes m_att;

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Initializes the segment generator
        /// </summary>
        public void GenerateBase()
        {
            // Build camera
            BuildCamera();

            // Send our collection of scrolling items to the client camera
            foreach (SyncPI pItem in m_att.BGItem)
                Camera.main.SendMessage("AddScrollObject", pItem);
        }

        /// <summary>
        /// Generates the segment.
        /// Starts a process of cycling through obj spawns
        /// </summary>
        /// <param name="segment">Segment.</param>
        public void StartSegmentCycle()
        {
            // Start new coroutine with new segment
            StartCoroutine("CycleSegment");
        }

        [Server]
        public void GenerateServerObjects()
        {
            StartCoroutine("BuildSegment");
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
                playerCam.clearFlags = CameraClearFlags.Nothing;
                playerCam.depth = 1;
                playerCam.cullingMask &= ~(1 << LayerMask.NameToLayer("SpaceElements"));
                playerCam.backgroundColor = Color.black;
                playerCam.orthographic = true;
                playerCam.orthographicSize = 20f;

                // add and initialize scripts for the camera object
                CameraMessageHandler cMsg = camObject.AddComponent<CameraMessageHandler>();
                cMsg.CamFollow = camObject.AddComponent<CameraFollow>();
                camObject.AddComponent<CameraShake>();
                camObject.AddComponent<AudioListener>();
                camObject.AddComponent<ParellaxScroller>();

                BuildStarfield(camObject.transform, cMsg);
            }
        }

        private void BuildStarfield(Transform cam, CameraMessageHandler cMsg)
        {
            GameObject spaceCamObj = new GameObject();
            spaceCamObj.name = "SpaceCamera";
            spaceCamObj.transform.parent = cam;
            spaceCamObj.transform.Translate(new Vector3(0, 0, -10f));

            // create and format the camera
            Camera spaceCam = spaceCamObj.AddComponent<Camera>();
            spaceCam.clearFlags = CameraClearFlags.SolidColor;
            spaceCam.depth = 0;
            spaceCam.cullingMask = (1 << LayerMask.NameToLayer("SpaceElements"));
            spaceCam.backgroundColor = Color.black;
            spaceCam.orthographic = true;
            spaceCam.orthographicSize = 20f;

            // create objects that follow player
            GameObject starField = (GameObject)Instantiate(Resources.Load("Space/starfield")
                                                            , Vector3.zero, Quaternion.identity);
            cMsg.StarField = starField.GetComponent<StarFieldController>();
            starField.transform.parent = spaceCamObj.transform;

            GalaxyScroller gScroll = spaceCamObj.AddComponent<GalaxyScroller>();
            gScroll.AssignBG(1, spaceCam);
        }

        #endregion

        #region COROUTINES

        private IEnumerator BuildSegment()
        {
            for(int i = 0; i < m_att.SegObjs.Count; i++)
            {
                SegmentObjectData segObj = m_att.SegObjs[i];
                switch (segObj._type)
                {
                    case "_asteroids":
                        {
                            // Build the game object in generator
                            GameObject segObjGO = Instantiate(AsteroidPrefab);

                            segObjGO.transform.position = segObj._position;

                            NetworkServer.Spawn(segObjGO);

                            // Assign segmentobject to segment object
                            segObjGO.GetComponent<SegmentObjectBehaviour>().Create
                                (segObj);

                            // assign network inst id
                            segObj.netID = segObjGO.GetComponent<NetworkIdentity>().netId;

                            break;
                        }
                    case "_satellites":
                        {
                            // Build the game object in generator
                            GameObject segObjGO = m_objectGenerator.GenerateSatellite
                                    (segObj);

                            // assign network inst id
                            segObj.netID = segObjGO.GetComponent<NetworkIdentity>().netId;

                            // Assign segmentobject to segment object
                            segObjGO.GetComponent<SegmentObjectBehaviour>().Create
                                (segObj);

                            break;
                        }
                    case "_clouds":
                        {
                            // Build the game object in generator
                            GameObject segObjGO = Instantiate(CloudPrefab);

                            segObjGO.transform.position = segObj._position;

                            NetworkServer.Spawn(segObjGO);

                            // Assign segmentobject to segment object
                            segObjGO.GetComponent<SegmentObjectBehaviour>().Create
                                (segObj);

                            // assign network inst id
                            segObj.netID = segObjGO.GetComponent<NetworkIdentity>().netId;

                            break;
                        }
                    case "_debris":
                        {
                            // Build the game object in generator
                            GameObject segObjGO = Instantiate(GravePrefab);

                            segObjGO.transform.position = segObj._position;

                            NetworkServer.Spawn(segObjGO);

                            // Assign segmentobject to segment object
                            segObjGO.GetComponent<SegmentObjectBehaviour>().Create
                                (segObj);

                            // assign network inst id
                            segObj.netID = segObjGO.GetComponent<NetworkIdentity>().netId;

                            break;
                        }
                }

                m_att.SegObjs[i] = segObj;

                yield return null;
            }

            yield return null;
        }

        /// <summary>
        /// Cycles the segment.
        /// enables objects within range
        /// only checks if objects are disabled
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

                if (player == null)
                {
                    yield return null;
                    continue;
                }
                
                Vector3 playerPos = player.transform.position;

                foreach (SegmentObjectData segObj
                         in m_att.SegObjs)
                {
                    GameObject segObjGO = ClientScene.FindLocalObject(segObj.netID);

                    if (segObjGO == null || segObj._visibleDistance <= 0)
                        continue;

                    // If object is within range we will reenable it
                    if (Vector3.Distance(playerPos, segObjGO.transform.position)
                           < segObj._visibleDistance)
                        // Make sure the object is actually disabled
                        if (!isServer && !segObjGO.activeSelf)
                            segObjGO.SetActive(true);
                        else
                        // if server we cant disable object so make it function minimally
                        if (!segObjGO.GetComponent<SegmentObjectBehaviour>().Active)
                            segObjGO.GetComponent<SegmentObjectBehaviour>().ReEnable();
                }
                
                yield return null;
            }
        }

        #endregion
    }
}
