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

        public delegate void ObjState();
        public event ObjState ObjEnable;
        public event ObjState ObjDisable;

        #endregion

        #region ATTRIBUTES

        [SyncVar]
        SegmentObject _segObject;

        #endregion

        #region MONO BEHAVIOUR

        void Start()
        {
            Position();

            if (_segObject._texturePath != "")
                Render();

            if (_segObject._visibleDistance > 0)
                StartCoroutine("PopCheck");
        }

        void OnDisable()
        {
            if(Destroyed != null)
                Destroyed(_segObject);

            if (_segObject._visibleDistance > 0)
                StopCoroutine("PopCheck");
        }

        void OnEnable()
        {
            if(_segObject._visibleDistance > 0)
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

            if (_segObject._visibleDistance > 0)
                StartCoroutine("PopCheck");

            //DisableObj();
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Only called on server to reenable features
        /// </summary>
        public void Reenable()
        {
            if (GetComponent<SpriteRenderer>() != null)
                GetComponent<SpriteRenderer>().enabled = true;
            if (GetComponent<BoxCollider2D>() != null)
                GetComponent<BoxCollider2D>().enabled = true;
            if (GetComponent<NetworkTransform>() != null)
                GetComponent<NetworkTransform>().enabled = true;

            if (ObjEnable != null)
                ObjEnable();

            StartCoroutine("PopCheck");
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

                if (ObjDisable != null)
                    ObjDisable();
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
                if (GetComponent<NetworkTransform>() != null)
                    GetComponent<NetworkTransform>().enabled = false;

                StopCoroutine("PopCheck");

                if (ObjDisable != null)
                    ObjDisable();
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
            transform.position = _segObject._position;
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

                if (Vector3.Distance(thisPos, playerPos) > _segObject._visibleDistance)
                {
                    DisableObj();
                }
                yield return null;
            }
        }

        #endregion
    }
}

