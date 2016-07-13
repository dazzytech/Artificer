using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Data.Space;

namespace Space.Segment
{
    /// <summary>
    /// Segment object.
    /// attached to every object
    /// added using the segment generator
    /// deletes self when out of popdistance with 
    /// player
    /// </summary>
    public class SegmentObjectBehaviour : NetworkBehaviour
    {
        /*float _popDistance;
        SegmentObject SegObject;

        // Some objs will want to know when said obj 
        //is destroyed
        public delegate void ObjEvent(SegmentObject segObj);
        public static event ObjEvent Destroyed;

        void OnDisable()
        {
            Destroyed(SegObject);
            StopCoroutine("PopCheck");
        }

        public void Start()
        {
            StartCoroutine("PopCheck");
        }

        public void Create(float popDistance, SegmentObject Obj)
        {
            SegObject = Obj;

            _popDistance = popDistance;
        }



        IEnumerator PopCheck()
        {
            // for now just create infinite loop
            for(;;)
            {
                yield return null;

                GameObject player = GameObject.FindGameObjectWithTag 
                    ("PlayerShip");

                if(player == null)
                    continue;

                Vector3 playerPos = player.transform.position;

                Vector3 thisPos = transform.position;

                if(Vector3.Distance(thisPos, playerPos) > _popDistance 
                   && Destroyed != null)
                {
                    Destroyed(SegObject);

                    Destroy(this.gameObject);

                    StopCoroutine("PopCheck");
                }
                yield return null;
            }
        }*/
    }
}

