using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace Space.Segment
{
    public class AsteroidBehaviour : SegmentObject
    {
        #region ATTRIUTES

        [SyncVar]
        private float m_scale;

        #endregion

        #region MONO BEHAVIOUR

        private void Start()
        {
            transform.localScale =
                        new Vector3(m_scale,
                                    m_scale, 1f);
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
            m_pieceDensity = 200f * m_scale;

            m_parentID = parentID;

            transform.localScale =
                        new Vector3(m_scale,
                                    m_scale, 1f);

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.mass = m_scale;

            InitializeSegmentObject();
        }

        #endregion
    }
}