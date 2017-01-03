using UnityEngine;
using UnityEngine.Networking;

// Artificer
using System.Collections;
using Space.GameFunctions;

namespace Space.Segment
{
    /// <summary>
    /// Extends the impact collider on the station 
    /// responsible for resolving impacts on stations
    /// </summary>
    [RequireComponent(typeof(StationAttributes))]
    public class StationImpactCollider : ImpactCollider
    {
        #region ATTRIBUTES

        private StationController m_con;

        #endregion

        #region MONO BEHAVIOUR 

        void Awake()
        {
            m_con = GetComponent<StationController>();
        }

        #endregion

        #region IMPACT COLLISION

        public void ApplyDamage(HitData hData)
        {
            /*pieceDensity -= hData.damage;

            if (pieceDensity <= 0)
            {
                // this will work cause host
                NetworkServer.UnSpawn(this.gameObject);

                // for now just destroy
                Destroy(this.gameObject);
            }*/

            m_con.ProcessDamage(hData);
        }

        #endregion
    }
}
