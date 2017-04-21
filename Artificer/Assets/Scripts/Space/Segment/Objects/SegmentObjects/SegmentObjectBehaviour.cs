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
        public delegate void ObjEvent(SegmentObjectData segObj);
        public static event ObjEvent Destroyed;

        public delegate void ObjState();
        public event ObjState ObjEnable;
        public event ObjState ObjDisable;

        #endregion

        #region ATTRIBUTES

        [SyncVar]
        SegmentObjectData _segObject;

        [SerializeField]
        bool _physicalObject;

        public bool Active;

        #endregion

        #region MONO BEHAVIOUR

        protected virtual void Start()
        {
            // move location and place within parent
            Position();

            // If this isn't a container, render object
            // and begin distance checking
            if (_physicalObject)
            {
                if (_segObject._texturePath != "")
                    Render();

            }

            // disable as we wont have spawned
            DisableObj();
        }

        void OnDestroy()
        {
            if(Destroyed != null)
                Destroyed(_segObject);
        }

        #endregion

        #region SERVER INTERACTION

        /// <summary>
        /// Passed the object data
        /// </summary>
        /// <param name="popDistance"></param>
        /// <param name="Obj"></param>
        [Server]
        public void Create(SegmentObjectData Obj)
        {
            _segObject = Obj;

            Position();

            if (_physicalObject)
            {
                if (_segObject._texturePath != "")
                    Render();

            }
        }

        #endregion

        #region ENABLE & DISABLE

        /// <summary>
        /// Only called on server to reenable features
        /// </summary>
        public void ReEnable()
        {
            Active = true;

            if (_physicalObject)
            {
                // Only activate object if physical
                if (GetComponent<SpriteRenderer>() != null)
                    GetComponent<SpriteRenderer>().enabled = true;

                if (GetComponent<NetworkTransform>() != null)
                    GetComponent<NetworkTransform>().enabled = true;

                if (GetComponent<Collider2D>() != null)
                    GetComponent<Collider2D>().enabled = true;
            }

            StartCoroutine("PopCheck");

            // TODO: THIS SHOULDN'T DISABLE ASTEROIDS WHEN THEY GET THEIR OWN DISTANCE
            if (ObjEnable != null)
                ObjEnable();
        }

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
            }
            else
            {
                // think of asomething for localhost
                // no longer needed when object isnt destroyed

                Active = false;

                if (_physicalObject)
                {
                    // Only disable this object if it is physical
                    if (GetComponent<SpriteRenderer>() != null)
                        GetComponent<SpriteRenderer>().enabled = false;
                    if (GetComponent<BoxCollider2D>() != null)
                        GetComponent<BoxCollider2D>().enabled = false;
                    if (GetComponent<NetworkTransform>() != null)
                        GetComponent<NetworkTransform>().enabled = false;
                }

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

