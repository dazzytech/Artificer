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

        [SyncVar]
        private string _texturePath;

        public float PopDistance;

        [SyncVar]
        SegmentObject _segObject;

        #endregion

        #region MONO BEHAVIOUR

        void Start()
        {
            if (_texturePath != "")
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
        /// assigns texture to object
        /// </summary>
        /// <param name="newTexName"></param>
        [Server]
        public void AssignTexture(string newTexName)
        {
            if (newTexName == null)
                return;                     //error check

            _texturePath = newTexName;

            Render();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="popDistance"></param>
        /// <param name="Obj"></param>
        [Server]
        public void Create(SegmentObject Obj)
        {
            _segObject = Obj;
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

                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<BoxCollider2D>().enabled = false;
                GetComponent<NetworkTransform>().enabled = false;

                StopCoroutine("PopCheck");

                this.enabled = false;
            }
        }

        #endregion

        #region TEXTURE UTILITIES

        /// <summary>
        /// retreive texture for our object and render it
        /// </summary>
        private void Render()
        {
            if (GetComponent<SpriteRenderer>() == null)
                return;                     //error check

            Sprite img = Resources.Load(_texturePath, typeof(Sprite)) as Sprite;
            GetComponent<SpriteRenderer>().sprite = img;

            transform.parent = GameObject.Find("_satellites").transform;
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

