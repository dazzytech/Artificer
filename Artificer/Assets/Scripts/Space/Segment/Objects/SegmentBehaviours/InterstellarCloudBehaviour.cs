using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Data.Space;

namespace Space.Segment
{
    public class InterstellarCloudBehaviour : NetworkBehaviour
    {
        #region ATTRIUTES

        [SyncVar]
        private NetworkInstanceId _parentID;

        #endregion

        #region MONO BEHAVIOUR

        void Start()
        {
            // Check if we have been intialized by server
            if (!_parentID.IsEmpty())
                InitializeCloud();
        }

        void OnDestroy()
        {
            if (transform.parent != null)
            {
                transform.parent.GetComponent<SegmentObjectBehaviour>().ObjEnable
                    -= EnableObj;

                transform.parent.GetComponent<SegmentObjectBehaviour>().ObjDisable
                    -= DisableObj;
            }
        }

        void OnDisable()
        {
            DisableObj();
        }

        void OnEnable()
        {
            EnableObj();
        }

        #endregion

        #region SERVER INTERACTION

        /// <summary>
        /// Given parameters from server
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="parentID"></param>
        [Server]
        public void InitializeParameters(NetworkInstanceId parentID)
        {
            _parentID = parentID;

            InitializeCloud();
        }

        #endregion

        #region GAME OBJECT UTILITIES

        /// <summary>
        /// Use the parameters to create a working asteroid on
        /// client and server
        /// </summary>
        private void InitializeCloud()
        {
            GameObject parent = ClientScene.FindLocalObject(_parentID);
            if (parent != null)
            {
                transform.parent = parent.transform;

                transform.parent.GetComponent<SegmentObjectBehaviour>().ObjEnable
                    += EnableObj;

                transform.parent.GetComponent<SegmentObjectBehaviour>().ObjDisable
                    += DisableObj;
            }
            else
                Debug.Log(_parentID + " NOT FOUND");
           
            GetComponent<ParticleSystem>().Play();
        }

        private void EnableObj()
        {
            GetComponent<NetworkTransform>().enabled = true;
            GetComponent<ParticleSystem>().Play();
        }

        private void DisableObj()
        {
            // Parent is enabled but we are not
            GetComponent<NetworkTransform>().enabled = false;
            GetComponent<ParticleSystem>().Stop();
        }

        #endregion
    }
}
