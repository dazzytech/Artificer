using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Data.Space;
using System.Collections.Generic;

namespace Space.Segment
{
    public struct BroadCollisionTest
    {
        public List<SegmentObject> Objects; 
    }

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
        protected SegmentObjectData m_segObject;

        [SerializeField]
        public bool PhysicalObject;

        [HideInInspector]
        public bool Active;

        /// <summary>
        /// Broad Spectrum Collision Test
        /// first test collision on larger scale
        /// </summary>
        private BroadCollisionTest[,] m_BSCT;

        [SyncVar]
        protected int m_xSize;
        [SyncVar]
        protected int m_ySize;

        /// <summary>
        /// Broad Scale Multiplier
        /// percentage to divade regions into 
        /// </summary>
        private float m_bSM = 0.01f;

        #endregion

        #region MONO BEHAVIOUR

        protected virtual void Start()
        {
            // disable as we wont have spawned
            DisableObj();

            // move location and place within parent
            Position();

            // If this isn't a container, render object
            // and begin distance checking
            if (PhysicalObject)
                if (m_segObject._texturePath != "")
                    Render();

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

        #region PUBLIC INTERACTION

        #region SERVER INTERACTION

        /// <summary>
        /// Pass object to this container
        /// create child prefabs if applicable
        /// </summary>
        /// <param name="popDistance"></param>
        /// <param name="Obj"></param>
        [Server]
        public virtual void Create(SegmentObjectData Obj)
        {
            m_segObject = Obj;

            if (!PhysicalObject)
            {
                // Create boundaries based on
                // distance
                m_xSize = Mathf.CeilToInt(Obj._size.x * m_bSM);
                m_ySize = Mathf.CeilToInt(Obj._size.y * m_bSM);

                for(int i = 0; i < m_segObject._count; i++)
                {
                    BuildObject();
                }
            }
        }

        public void AddObject(SegmentObject Obj)
        {
            if(m_BSCT == null)
            {
                m_BSCT = new BroadCollisionTest[m_xSize, m_ySize];

                for (int x = 0; x < m_xSize; x++)
                    for (int y = 0; y < m_ySize; y++)
                        m_BSCT[x, y].Objects = new List<SegmentObject>();
            }

            // Create boundaries based on
            // distance
            int newX = Mathf.FloorToInt((Obj.transform.position.x - transform.position.x) * 0.01f);
            int newY = Mathf.FloorToInt((Obj.transform.position.y - transform.position.y) * 0.01f);

            m_BSCT[newX, newY].Objects.Add(Obj);
        }

        #endregion

        #region ENABLE & DISABLE

        /// <summary>
        /// Only called on server to reenable features
        /// </summary>
        public void ReEnable()
        {
            Active = true;

            if (PhysicalObject)
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

            if (ObjEnable != null)
                ObjEnable();
        }

        /// <summary>
        /// disable if client, otherwise hide as many processes as possible
        /// </summary>
        public void DisableObj()
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

                if (PhysicalObject)
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

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Builds an object of the given
        /// prefab type using segment data
        /// </summary>
        [Server]
        protected virtual void BuildObject()
        {
            // Init and set parent of object
            GameObject subObj = (GameObject)Instantiate
                (Resources.Load(m_segObject._prefabPath));
            subObj.transform.parent = transform;

            // Give a random location and size;
            Vector2 location = new Vector2
                (Random.Range(0f, m_segObject._size.x),
                 Random.Range(0f, m_segObject._size.y));

            // If we have a bounds then keep within them
            if (m_segObject._border != null)
                while (!Math.IsPointInPolygon
                   (m_segObject._position + location, m_segObject._border))
                        location = new Vector2
                            (Random.Range(0f, m_segObject._size.x),
                             Random.Range(0f, m_segObject._size.y));

            // position with our parameter
            subObj.transform.localPosition = location;

            // Spawn on network and init object
            NetworkServer.Spawn(subObj);
            subObj.GetComponent<SegmentObject>().
                InitializeParameters(netId);
        }

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

        private void Update()
        {
            if (m_segObject._border != null)
            {
                for (int i = 0; i < m_segObject._border.Length; i++)
                {
                    int a = i == m_segObject._border.Length - 1 ? 0 : i + 1;
                    Debug.DrawLine(m_segObject._border[i], m_segObject._border[a], Color.red);
                }
            }
        }

        #endregion

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
                    DisableObj();

                    yield return null;
                    continue;
                }

                Vector3 playerPos = player.transform.position;

                Vector2 thisPos = transform.position;

                // fix
                Vector2 centerPos = !PhysicalObject? thisPos + m_segObject._size * .5f : thisPos;

                if (Vector3.Distance(thisPos, playerPos) > m_segObject._visibleDistance)
                {
                    DisableObj();
                }
                else
                {
                    if (m_BSCT != null)
                    {
                        int x = Mathf.FloorToInt((playerPos.x - thisPos.x) * m_bSM);
                        int y = Mathf.FloorToInt((playerPos.y - thisPos.y) * m_bSM);

                        for (int dx = -1; dx <= 1; dx++)
                        {
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                if (x + dx >= 0 && x + dx < m_xSize &&
                                    y + dy >= 0 && y + dy < m_ySize)
                                {
                                    foreach (SegmentObject obj in m_BSCT[x + dx, y + dy].Objects)
                                        if (!obj.Running)
                                            obj.ParentEnabled(playerPos);
                                }
                                yield return null;
                            }
                            yield return null;
                        }
                    }
                }
                yield return null;
            }
        }

        #endregion

    }
}

