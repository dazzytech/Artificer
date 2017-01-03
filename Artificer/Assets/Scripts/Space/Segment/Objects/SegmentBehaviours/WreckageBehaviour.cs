using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using Space.GameFunctions;

using Data.Space;
using Networking;

/// <summary>
/// Prefab objects for wreckage objects
/// Retreives a list of IDs and creates wreckage
/// components using those components
/// </summary>
namespace Space.Segment
{
    public class WreckageBehaviour : ImpactCollider
    {
        [SyncVar]
        WreckageData wreckage = new WreckageData();
        [SyncVar]
        public float pieceDensity;
        public float maxDensity;
        [SyncVar]
        private NetworkInstanceId _parentID;

        #region MONO BEHAVIOUR

        void Start()
        {
            // If this wreckage has already been assigned then build it
            if (wreckage.components != null)
                BuildWreckage();

            maxDensity = pieceDensity = (40f * transform.childCount);

            GetComponent<Rigidbody2D>().mass = transform.childCount;
        }

        void OnDestroy()
        {
            // Release the event listener
            if (transform.parent != null)
            {
                transform.parent.GetComponent<SegmentObjectBehaviour>().ObjEnable
                    -= ParentEnabled;

                transform.parent.GetComponent<SegmentObjectBehaviour>().ObjDisable
                    -= ParentDisabled;
            }
        }

        #endregion

        #region SERVER INTERACTION

        /// <summary>
        /// Copies components into storage and begins the wreckage
        /// creation process.
        /// </summary>
        /// <param name="componentList"></param>
        public void InitializeWreckage(WreckageData wreck, NetworkInstanceId parentID)
        {
            wreckage = wreck;

            _parentID = parentID;

            if (wreckage.components == null)
            { 
                NetworkServer.UnSpawn(gameObject);
                Destroy(gameObject);
                return;
            }

            maxDensity = pieceDensity = (40f * transform.childCount);
        }

        #endregion

        #region EVENT LISTENER

        /// <summary>
        /// start running coroutine when parent within range
        /// </summary>
        private void ParentEnabled()
        {
            EnableObj();
        }

        /// <summary>
        /// stop running coroutine when parent out of range
        /// </summary>
        private void ParentDisabled()
        {
            DisableObj();
        }

        #endregion

        #region GAME OBJECT UTILITIES

        /// <summary>
        /// Server host, puts asteroid at normal functioning
        /// </summary>
        private void EnableObj()
        {
            if (this == null)
            {
                return;
            }

            // Parent isnt enabled but we are 
            GetComponent<NetworkTransform>().enabled = true;

            foreach (Transform t in transform)
                if(t != transform)
                    t.gameObject.SetActive(true);
        }

        /// <summary>
        ///  Server host, puts asteroid at minimal functioning
        /// </summary>
        private void DisableObj()
        {
            if (this == null)
            {
                return;             //fix for something not understood
            }

            // Parent is enabled but we are not
            GetComponent<NetworkTransform>().enabled = false;

            foreach (Transform t in transform)
                if (t != transform)
                    t.gameObject.SetActive(false);
        }

        #endregion

        #region WRECKAGE CREATION

        private List<int> addedIDs;

        /// <summary>
        /// Generate 
        /// </summary>
        private void BuildWreckage()
        {
            GameObject parent = ClientScene.FindLocalObject(_parentID);
            if (parent != null)
            {
                transform.parent = parent.transform;

                transform.parent.GetComponent<SegmentObjectBehaviour>().ObjEnable
                    += ParentEnabled;

                transform.parent.GetComponent<SegmentObjectBehaviour>().ObjDisable
                    += ParentDisabled;
            }
            else
                Debug.Log(_parentID + " NOT FOUND");

            // create list for storing info
            addedIDs = new List<int>();

            // POPULATE EMPTY SHIP
            // Add head 
            // Create game object
            GameObject headGO =
                Instantiate(Resources.Load
                            ("Space/Wreckage/" + wreckage.components[0].Name))
                    as GameObject;

            // set transform
            headGO.transform.SetParent(this.transform, false);
            headGO.transform.localPosition = Vector3.zero;

            addedIDs.Add(wreckage.components[0].InstanceID);

            // build the body around this
            BuildConnectedPieces
                (wreckage.components[0], headGO.transform);

            DisableObj();
        }

        /// <summary>
        /// Builds the connected pieces.
        /// Takes a piece and constructs all 
        /// connected pieces and 
        /// works recursively
        /// </summary>
        /// <param name="component">Component.</param>
        /// <param name="componentTransform">Component transform.</param>
        /// <param name="ship">Ship.</param>
        public void BuildConnectedPieces
            (Data.Shared.Component component, Transform componentTransform)
        {
            Vector3 otherPos;
            Vector3 thisPos;

            Data.Shared.Socket[] socketList = component.sockets;

            // no sockets = no body
            if (socketList == null)
                return;

            foreach (Data.Shared.Socket socket in socketList)
            {
                // Get position of this socket
                // through the components transform
                Transform thisTrans = componentTransform.Find
                    (String.Format
                     ("socket_{0}", socket.SocketID));

                // test we successfully found the socket 
                if (thisTrans == null)
                {
                    Debug.Log("Wreckage Generator - " +
                            "BuildConnectedPieces:" +
                              "socket Transform not found - "
                              + socket.SocketID);
                    return;
                }

                thisPos =
                    componentTransform.Find
                        (String.Format
                         ("socket_{0}", socket.SocketID)).position;

                // find the second piece through the socket
                Data.Shared.Component piece = wreckage.GetComponent(socket.OtherID);
                if (piece.Path == "")
                {
                    Debug.Log("Wreckage Generator - " +
                        "BuildConnectedPieces: other " +
                        "socket not found!");
                    return;
                }

                // test we haven't already added this piece.
                // stops unending loops
                if (addedIDs.Contains(piece.InstanceID))
                    continue;

                // create the piece
                GameObject pieceGO = null;

                // Test if we actually successfully created the new piece
                if (Resources.Load
                    ("Space/Wreckage/" + piece.Name) == null)
                {
                    Debug.Log("Wreckage Generator - " +
                              "BuildConnectedPieces: other " +
                              "Could not find: " + piece.Name);
                    return;
                }

                //Add the component piece to the game world
                pieceGO =
                    Instantiate(Resources.Load
                                ("Space/Wreckage/" + piece.Name))
                            as GameObject;

                // Set the direction of the new piece
                Vector3 dirEuler = new Vector3(0, 0, 0);
                switch (piece.Direction)
                {
                    case "up":
                        dirEuler.z = 0f; break;
                    case "down":
                        dirEuler.z = 180f; break;
                    case "left":
                        dirEuler.z = 90; break;
                    case "right":
                        dirEuler.z = 270f; break;
                }

                // Apply direction to obj and sockets
                pieceGO.transform.eulerAngles = dirEuler;

                // make child of ship
                pieceGO.transform.SetParent(this.transform, false);

                // find position of other piece and then
                // snap the pieces together
                otherPos = pieceGO.transform.Find
                    (String.Format
                     ("socket_{0}", socket.OtherLinkID)).position;

                Vector3 snapDistance = otherPos - thisPos;

                pieceGO.transform.position -= snapDistance;

                addedIDs.Add(piece.InstanceID);

                BuildConnectedPieces
                    (piece, pieceGO.transform);
            }
        }

        #endregion

        #region IMPACT COLLIDER

        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.transform.gameObject.tag != "Station")
            {
                Vector2 dir = transform.position - other.transform.position;
                dir = other.transform.InverseTransformDirection(dir);
                float magnitude = dir.sqrMagnitude;
                GetComponent<Rigidbody2D>().AddForce(dir * magnitude, ForceMode2D.Force);

                HitData hitD = new HitData();
                hitD.damage = 50f;
                hitD.hitPosition = other.contacts[0].point;
                hitD.originID = this.netId;

                // retrieve impact controller
                // and if one exists make ship process hit
                ImpactCollider IC = other.transform.GetComponent<ImpactCollider>();

                if (IC != null)
                    IC.Hit(hitD);
            }
        }

        [ClientRpc]
        public override void RpcHitArea()
        {
            RpcHit();
        }

        public void ApplyDamage(HitData hData)
        {
            pieceDensity -= hData.damage;

            if (pieceDensity <= 0)
            {
                // this will work cause host
                NetworkServer.UnSpawn(this.gameObject);

                // for now just destroy
                Destroy(this.gameObject);
            }

        }

        #endregion
    }
}