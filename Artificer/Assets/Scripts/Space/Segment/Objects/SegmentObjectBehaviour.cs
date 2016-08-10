using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Data.Space;

namespace Space.Segment
{
    /// <summary>
    /// 
    /// </summary>
    public class SegmentObjectBehaviour : NetworkBehaviour
    {
        #region EVENTS

        // Some objs will want to know when said obj 
        // is destroyed
        public delegate void ObjEvent(SegmentObject segObj);
        public static event ObjEvent Destroyed;

        #endregion

        #region ATTRIBUTES

        public float PopDistance;

        [SyncVar]
        SegmentObject _segObject;

        #endregion

        #region MONO BEHAVIOUR

        void Start()
        {
            Position();

            if (_segObject._texturePath != "")
                Render();
        }

        void OnDisable()
        {
            if(Destroyed != null)
                Destroyed(_segObject);
            StopCoroutine("PopCheck");
        }

        void OnEnable()
        {
            StartCoroutine("PopCheck");
        }

        #endregion

        #region SERVER INTERACTION

        /// <summary>
        /// Passed the object data
        /// </summary>
        /// <param name="popDistance"></param>
        /// <param name="Obj"></param>
        [Server]
        public void Create(SegmentObject Obj)
        {
            _segObject = Obj;

            Position();

            Render();
        }

        #endregion

        #region INTERNAL FUNCTION

        /// <summary>
        /// disable if client, otherwise hide as many processes as possible
        /// </summary>
        private void DisableObj()
        {
            if (!isServer)
            {
                if (Destroyed != null)
                    Destroyed(_segObject);

                this.gameObject.SetActive(false);

                StopCoroutine("PopCheck");
            }
            else
            {
                // think of asomething for localhost

                if (Destroyed != null)
                    Destroyed(_segObject);

                if(GetComponent<SpriteRenderer>() != null)
                    GetComponent<SpriteRenderer>().enabled = false;
                if(GetComponent<BoxCollider2D>() != null)
                    GetComponent<BoxCollider2D>().enabled = false;
                GetComponent<NetworkTransform>().enabled = false;

                StopCoroutine("PopCheck");

                this.enabled = false;
            }
        }

        #endregion

        #region GAMEOBJECT UTILITIES

        /// <summary>
        /// retreive texture for our object and render it
        /// </summary>
        private void Render()
        {
            if (GetComponent<SpriteRenderer>() == null)
                return;                     //error check

            Sprite img = Resources.Load(_segObject._texturePath, typeof(Sprite)) as Sprite;
            GetComponent<SpriteRenderer>().sprite = img;
        }

        /// <summary>
        /// Place in position and within heirachy 
        /// </summary>
        private void Position()
        {
            transform.parent = GameObject.Find(_segObject._type).transform;
            transform.name = _segObject._name;
            transform.tag = _segObject._tag;
        }

        #endregion

        #region COROUTINES

            /// <summary>
            /// disable this object if far away
            /// </summary>
            /// <returns></returns>
            IEnumerator PopCheck()
        {
            // for now just create infinite loop
            for (;;)
            {
                GameObject player = GameObject.FindGameObjectWithTag
                    ("PlayerShip");

                if (player == null)
                {
                    yield return null;
                    continue;
                }

                Vector3 playerPos = player.transform.position;

                Vector3 thisPos = transform.position;

                if (Vector3.Distance(thisPos, playerPos) > PopDistance)
                {
                    DisableObj();
                }
                yield return null;
            }
        }

        #endregion
    }
}

