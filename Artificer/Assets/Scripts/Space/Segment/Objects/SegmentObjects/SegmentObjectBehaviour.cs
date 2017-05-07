using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Data.Space;

namespace Space.Segment
{
    /// <summary>
    /// Base class for objects that are 
    /// spawned within 
    /// </summary>
    public class SegmentObjectBehaviour : NetworkBehaviour
    {
        #region EVENTS

        #region OBJECT

        // Objects such as map controller will
        // listen for the creation and destruction of these object
        public delegate void ObjEvent(SegmentObjectData segObj, Transform obj);
        public static event ObjEvent Created;
        public static event ObjEvent Destroyed;

        #endregion

        #region STATE

        // Child objects listen to this for their active state
        public delegate void ObjState();
        public event ObjState ObjEnable;
        public event ObjState ObjDisable;

        #endregion

        #endregion

        #region ATTRIBUTES

        [SyncVar]
        private SegmentObjectData m_segObject;

        [SerializeField]
        bool m_physicalObject;

        public bool Active;

        #endregion

        #region MONO BEHAVIOUR

        protected virtual void Start()
        {
            // move location and place within parent
            Position();

            // If this isn't a container, render object
            // and begin distance checking
            if (m_physicalObject)
            {
                if (m_segObject._texturePath != "")
                    Render();

            }

            // disable as we wont have spawned
            DisableObj();

            // Alert that create have created this object
            if (Created != null)
                Created(m_segObject, transform);
        }

        void OnDestroy()
        {
            if(Destroyed != null)
                Destroyed(m_segObject, transform);
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
            m_segObject = Obj;

            Position();

            if (m_physicalObject)
            {
                if (m_segObject._texturePath != "")
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

            if (m_physicalObject)
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
                    Destroyed(m_segObject, transform);

                this.gameObject.SetActive(false);
            }
            else
            {
                // think of asomething for localhost
                // no longer needed when object isnt destroyed

                Active = false;

                if (m_physicalObject)
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

            Sprite img = Resources.Load(m_segObject._texturePath, typeof(Sprite)) as Sprite;
            GetComponent<SpriteRenderer>().sprite = img;
        }

        /// <summary>
        /// Place in position and within heirachy 
        /// </summary>
        private void Position()
        {
            transform.parent = GameObject.Find(m_segObject._type).transform;
            transform.name = m_segObject._name;
            transform.tag = m_segObject._tag;
            transform.position = m_segObject._position;
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

                if (Vector3.Distance(thisPos, playerPos) > m_segObject._visibleDistance)
                {
                    DisableObj();
                }
                yield return null;
            }
        }

        #endregion
    }
}

