using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// Prefab objects for wreckage objects
/// Retreives a list of IDs and creates wreckage
/// components using those components
/// </summary>
namespace Space.Segment
{
    public class WreckageBehaviour : SegmentObject
    {
        #region SERVER INTERACTION

        /// <summary>
        /// Copies components into storage and begins the wreckage
        /// creation process.
        /// </summary>
        /// <param name="componentList"></param>
        public override void InitializeParameters(NetworkInstanceId parentID)
        {
            m_maxDensity = m_pieceDensity = 
                UnityEngine.Random.Range(5000f, 10000f);

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.mass = 4;

            base.InitializeParameters(parentID);
        }

        #endregion
    }
}