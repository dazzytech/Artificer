using UnityEngine;
using System.Collections;
// Artificer
using Data.Space;
using Space.Teams;
//using Space.Generator;

namespace Space.Segment
{
    [RequireComponent(typeof(StationAttributes))]
    public class StationController : MonoBehaviour
    {
        #region ATTRIBUTES

        private StationAttributes m_att;

        #endregion

        #region MONO BEHAVIOUR 

        void Awake()
        {
            m_att = GetComponent<StationAttributes>();

            m_att.CurrentIntegrity = m_att.Integrity;
        }

        #endregion

        #region PUBLIC INTERACTION

        public void Initialize()
        {
            // no current reason to initialize yet
        }

        #endregion
        /*
    //private SegmentObject _station;

    public SegmentObject Station
    {
        set { _station = value; }
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if(Enter)
            GameObject.Find("space").SendMessage("StationReached", 
                collider.transform.parent, SendMessageOptions.DontRequireReceiver);
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
    }
    
    public bool Functional
    {
        get{ return _integrity > 0;}
    }

    public float NormalizedHealth
    {
        get { return _integrity / 10000f;}
    }*/
    }
}