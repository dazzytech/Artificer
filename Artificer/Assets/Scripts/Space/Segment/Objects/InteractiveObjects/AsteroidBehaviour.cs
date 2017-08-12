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

        #region PUBLIC INTERACTION

        /// <summary>
        /// Given parameters from server
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="parentID"></param>
        [Server]
        public override void InitializeParameters(NetworkInstanceId parentID)
        {
            float scale = Random.Range(3f, 10f);
            m_scale = scale;
            m_pieceDensity = 200f * m_scale;

            transform.localScale =
                        new Vector3(m_scale,
                                    m_scale, 1f);

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.mass = m_scale;

            base.InitializeParameters(parentID);
        }

        #endregion

        #region PRIVATE UTILITIES

        protected override void InitializeSegmentObject()
        {
            base.InitializeSegmentObject();

            transform.localScale =
                        new Vector3(m_scale,
                                    m_scale, 1f);
        }

        #endregion
    }
}