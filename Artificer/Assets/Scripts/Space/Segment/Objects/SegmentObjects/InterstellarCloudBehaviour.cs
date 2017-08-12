using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Data.Space;

namespace Space.Segment
{
    public class InterstellarCloudBehaviour : SegmentObject
    {
        #region MONO BEHAVIOUR

        void Start()
        {
            // Check if we have been intialized by server
            if (!m_parentID.IsEmpty())
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
        public override void InitializeParameters(NetworkInstanceId parentID)
        {
            m_parentID = parentID;

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
            m_parent = ClientScene.FindLocalObject(m_parentID);

            if (m_parent != null)
            {
                transform.parent = m_parent.transform;

                Parent.ObjEnable
                    += EnableObj;

                Parent.ObjDisable
                    += DisableObj;
            }
            else
                Debug.Log(m_parentID + " NOT FOUND");
           
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
