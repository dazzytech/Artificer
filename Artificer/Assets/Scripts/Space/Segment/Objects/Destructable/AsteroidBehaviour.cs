using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
//Artificer
using Data.Space.Library;
using Space.Segment;

namespace Space.Segment
{
    public class AsteroidBehaviour : DestructableObject
    {
        #region ATTRIUTES

        [SyncVar]
        private NetworkInstanceId m_parentID;

        [SyncVar]
        private float m_scale;

        [SerializeField]
        private float m_withinDistance;

        private bool m_running;

        #endregion

        #region MONO BEHAVIOUR

        void Start()
        {
            // Check if we have been intialized by server
            if (!m_parentID.IsEmpty())
                InitializeAsteroid();
            else
                StartCoroutine("PopCheck");
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
            else
                StopCoroutine("PopCheck");
        }

        #endregion

        #region SERVER INTERACTION

        /// <summary>
        /// Given parameters from server
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="parentID"></param>
        [Server]
        public void InitializeParameters(float scale, NetworkInstanceId parentID)
        {
            m_scale = scale;
            m_pieceDensity = 20f * m_scale;
            m_parentID = parentID;

            InitializeAsteroid();
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
        /// Use the parameters to create a working asteroid on
        /// client and server
        /// </summary>
        private void InitializeAsteroid()
        {
            GameObject parent = ClientScene.FindLocalObject(m_parentID);
            if (parent != null)
            {
                transform.parent = parent.transform;

                transform.parent.GetComponent<SegmentObjectBehaviour>().ObjEnable
                    += ParentEnabled;

                transform.parent.GetComponent<SegmentObjectBehaviour>().ObjDisable
                    += ParentDisabled;
            }
            else
                Debug.Log(m_parentID + " NOT FOUND");

            transform.localScale =
                        new Vector3(m_scale,
                                    m_scale, 1f);

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.mass = m_scale;

            // give each asteroid a gentle velocity so it moves
            //rb.AddForce(new Vector2(Random.Range(-1f, 1f),
            //Random.Range(-1f, 1f)), ForceMode2D.Impulse);

            m_maxDensity = m_pieceDensity;
        }

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
            GetComponent<SpriteRenderer>().enabled = true;

            foreach (CircleCollider2D c in GetComponents<CircleCollider2D>())
                c.enabled = true;

            m_running = true;
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
            GetComponent<SpriteRenderer>().enabled = false;
            foreach (CircleCollider2D c in GetComponents<CircleCollider2D>())
                c.enabled = false;

            m_running = false;
        }

        #endregion

        #region COROUTINES

        /// <summary>
        /// Like a standard segment object, enable 
        /// object if in range and disable if not
        /// </summary>
        /// <returns></returns>
        private IEnumerator PopCheck()
        {
            // for now just create infinite loop
            while (true)
            {
                GameObject player = GameObject.FindGameObjectWithTag
                    ("PlayerShip");

                if (player == null)
                {
                    if (m_running)
                        DisableObj();

                    yield return null;
                    continue;
                }

                Vector3 playerPos = player.transform.position;

                Vector3 thisPos = transform.position;

                if (Vector3.Distance(thisPos, playerPos) > m_withinDistance)
                {
                    // we are out of visiblity distance
                    if (m_running)
                        DisableObj();
                }
                else
                {
                    // within visual range
                    if (!m_running)
                        EnableObj();
                }
                yield return null;
            }
        }

        #endregion 
    }
}