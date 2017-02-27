using UnityEngine;
using UnityEngine.Networking;

// Artificer
using System.Collections;

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
            if (isServer)
            {
                hData.damage *= Random.Range(0.5f, 1.0f);

                m_con.ProcessDamage(hData);
            }

            GameObject attacker =
            ClientScene.FindLocalObject(hData.originID);
            attacker.SendMessage("SetCombatant", this.transform);
        }

        #endregion
    }
}
