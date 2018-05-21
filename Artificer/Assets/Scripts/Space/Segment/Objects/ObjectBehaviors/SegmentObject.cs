using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Space.Segment
{
    /// <summary>
    /// Base object behaviours for 
    /// objects in space that have a physical presence
    /// and reacts to physical interaction
    /// </summary>
    public class SegmentObject : ImpactCollider
    {
        #region ATTRIBUTES

        #region INTEGRITY

        [SyncVar]
        protected float m_pieceDensity;
        protected float m_maxDensity;

        #endregion

        #region SEGMENT OBJECT

        private bool m_running;

        [SerializeField]
        private float m_withinDistance = 100f;

        [SyncVar]
        protected NetworkInstanceId m_parentID;

        protected GameObject m_parent;

        #endregion

        #region DESTRUCTABLE

        // Reference to our texture
        private SpriteRenderer m_objSprite;

        [SerializeField]
        // Will be set by child objects
        private int m_itemIndex = -1;

        [SerializeField]
        protected Texture2D[] m_fragments;

        #endregion

        #endregion

        #region ACCESSORS

        /// <summary>
        /// Returns the item yield of the segment 
        /// object
        /// </summary>
        public int Index
        {
            get { return m_itemIndex; }
        }

        /// <summary>
        /// Returns in the object is 
        /// determining distance
        /// </summary>
        public bool Running
        {
            get { return m_running; }
        }

        public SegmentObjectBehaviour Parent
        {
            get
            {
                if(m_parent != null)
                    return m_parent.GetComponent<SegmentObjectBehaviour>();

                return null;
            }
        }

        #endregion

        #region MONO BEHAVIOUR

        private void Start()
        {
            // retreive texture from sprite
            // renderer
            m_objSprite = GetComponent<SpriteRenderer>();

            // If we have a parent then determine 
            // visibility on parent else check visibility 
            // manually
            if (!m_parentID.IsEmpty())
                InitializeSegmentObject();
            else
                StartCoroutine("PopCheck");
        }

        void OnDestroy()
        {
            // Release the event listener
            if (transform.parent == null)
                StopCoroutine("PopCheck");
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if(Parent != null)
                if (!Parent.PhysicalObject)
                    return;

            /// Apply damage to any object it
            /// hits and bumps it away
            if (other.transform.gameObject.tag != "Station")
            {
                Vector2 dir = transform.position - other.transform.position;
                dir = other.transform.InverseTransformDirection(dir);
                float magnitude = dir.sqrMagnitude;
                GetComponent<Rigidbody2D>().AddForce(dir * magnitude, ForceMode2D.Force);

                HitData hitD = new HitData();
                hitD.damage = 50f;
                hitD.hitPosition = other.contacts[0].point;
                hitD.originID = this.netId;

                // retrieve impact controller
                // and if one exists make ship process hit
                ImpactCollider IC = other.transform.GetComponent<ImpactCollider>();

                if (IC != null)
                    IC.Hit(hitD);
            }
        }

        #endregion

        #region PUBLIC INTERACTION

        /// <summary>
        /// Called by parent to init data
        /// </summary>
        /// <param name="parentID"></param>
        [Server]
        public virtual void InitializeParameters(NetworkInstanceId parentID)
        {
            m_parentID = parentID;

            InitializeSegmentObject();
        }

        public void ApplyDamage(HitData hData)
        {
            if (isServer)
            {
                m_pieceDensity -= hData.damage;

                Vector2 dir = transform.position - m_hitD.hitPosition;
                dir = transform.InverseTransformDirection(dir);
                float magnitude = hData.damage;
                GetComponent<Rigidbody2D>().AddForce(dir * magnitude, ForceMode2D.Force);
            }

            if (m_objSprite != null && m_hitD.damageTex != null
                && m_hitD.damageTex != "")
            {
                // Ref to our texture rect for size
                Rect size = m_objSprite.sprite.rect;

                // Store scales
                float scaleX = transform.lossyScale.x;
                float scaleY = transform.lossyScale.y;

                // Create our mask
                Texture2D mask = Instantiate(Resources.Load("Textures/Damage/" + m_hitD.damageTex,
                    typeof(Texture2D)) as Texture2D);

                TextureScale.Bilinear(mask,(int)(mask.width / scaleX), 
                     (int)(mask.height / scaleY));

                // convert hit point from center
                Vector2 localHit = (m_hitD.hitPosition - transform.position);

                if (m_fragments.Length > 0)
                    DisperseFragments(m_hitD.hitPosition, localHit.normalized);

                // scale to current size
                localHit.x /= scaleX;
                localHit.y /= scaleY;

                localHit *= m_objSprite.sprite.pixelsPerUnit;

                // origin top left
                localHit.x += size.width * .5f;
                localHit.y += size.height * .5f;

                localHit = Math.RotateAroundPoint(localHit, size.center,
                    Quaternion.Euler(0,0,360 - transform.eulerAngles.z));

                float left = localHit.x - mask.width * .5f;
                float right = left + mask.width;
                float up = localHit.y + mask.height * .5f;
                float down = up - mask.height;
                
                // CREATE NEW TEXTURE

                // Create our new texture with the dimensions
                // of out sprite
                Texture2D newAtlas = new Texture2D
                    ((int)size.width, (int)size.height,
                    TextureFormat.RGBA32, false);

                // DRAW NEW TEXTURE

                for (int x = 0; x < size.width; x++)
                    for (int y = 0; y < size.height; y++)
                    {
                        if (x > left && x < right &&
                            y > down && y < up)
                        {
                            if (mask.GetPixel
                               (x - (int)left,
                                y - (int)down).a > 0f)
                            {
                                Color pix = m_objSprite.sprite.texture.GetPixel(x, y);
                                pix.a -= mask.GetPixel
                                   (x - (int)left,
                                    y - (int)down).a;

                                newAtlas.SetPixel(x, y, pix);
                                continue;
                            }
                        }
                        
                        newAtlas.SetPixel(x, y, m_objSprite.sprite.texture.GetPixel(x, y));
                    }

                newAtlas.Apply();

                m_objSprite.sprite = Sprite.Create(newAtlas,
                    size, new Vector2(.5f,.5f));
            }

            if (m_pieceDensity <= 0)
            {
                DestroyObject();
            }
        }

        /// <summary>
        /// start running coroutine when parent within range
        /// </summary>
        public void ParentEnabled(Vector2 playerPos)
        {
            if (Vector3.Distance(transform.position, playerPos) <= m_withinDistance)
                StartCoroutine("PopCheck");
        }

        #endregion

        #region PRIVATE UTILITIES

        /// <summary>
        /// Use the parameters to create a working asteroid on
        /// client and server
        /// </summary>
        protected virtual void InitializeSegmentObject()
        {
            DisableObj();

            m_parent = ClientScene.FindLocalObject(m_parentID);
            if (m_parent != null)
            {
                transform.parent = m_parent.transform;

                Parent.AddObject(this);
            }
            else
                Debug.Log(m_parentID + " NOT FOUND");

            m_maxDensity = m_pieceDensity;
        }

        /// <summary>
        /// Destroys the SegmentObject 
        /// but can be overridden to
        /// change behaviour
        /// </summary>
        protected virtual void DestroyObject()
        {
            // this will work cause host
            NetworkServer.UnSpawn(this.gameObject);

            // for now just destroy
            Destroy(this.gameObject);
        }

        /// <summary>
        /// Ejects a number of fragments when
        /// damaged. can be overridden to
        /// change behaviour
        /// </summary>
        /// <param name="point"></param>
        /// <param name="dir"></param>
        protected virtual void DisperseFragments(Vector2 point, Vector2 dir)
        {
            int numOfRocks = Mathf.CeilToInt(m_hitD.damage * 0.02f);

            for (int i = 0; i < numOfRocks; i++)
            {
                GameObject fragment = new GameObject();
                fragment.transform.position = point;
                fragment.layer = 8;

                Texture2D fragmentTexture = m_fragments[Random.Range(0, m_fragments.Length)];
                SpriteRenderer fragmentSprite = fragment.AddComponent<SpriteRenderer>();
                fragmentSprite.sprite = Sprite.Create(fragmentTexture, 
                    new Rect(0f,0f,fragmentTexture.width,fragmentTexture.height), new Vector2(.5f,.5f));
                

                // Set which item is yielded on pickup
                Debris behaviour =
                    fragment.AddComponent<Debris>();
                    behaviour.Initialize();
                
                Rigidbody2D rb = fragment.AddComponent<Rigidbody2D>();
                rb.AddForce(Random.insideUnitCircle * 20f, ForceMode2D.Force);
                rb.gravityScale = 0;

                BoxCollider2D col = fragment.AddComponent<BoxCollider2D>();
                Vector3 size = new Vector3(.5f, .5f);
                col.size = size;
                col.isTrigger = true;
            }
        }

        /// <summary>
        /// Server host, puts asteroid at normal functioning
        /// </summary>
        private void EnableObj()
        {
            if (this == null)
            {
                return;
            }

            // Parent isnt enabled but we are 
            GetComponent<NetworkTransform>().enabled = true;
            GetComponent<SpriteRenderer>().enabled = true;

            foreach (Collider2D c in GetComponents<Collider2D>())
                c.enabled = true;

            m_running = true;
        }

        /// <summary>
        ///  Server host, puts asteroid at minimal functioning
        /// </summary>
        private void DisableObj()
        {
            if (this == null)
            {
                return;             //fix for something not understood
            }

            // Parent is enabled but we are not
            GetComponent<NetworkTransform>().enabled = false;

            GetComponent<SpriteRenderer>().enabled = false;

            foreach (Collider2D c in GetComponents<Collider2D>())
                c.enabled = false;

            m_running = false;
        }

        #endregion

        #region COROUTINES

        /// <summary>
        /// Like a standard segment object, enable 
        /// object if in range and disable if not
        /// </summary>
        /// <returns></returns>
        private IEnumerator PopCheck()
        {
            // for now just create infinite loop
            while (true)
            {

                if (SystemManager.Space.Ship == null)
                {
                    if (m_running)
                        DisableObj();

                    yield return null;
                    continue;
                }

                Vector3 playerPos = SystemManager.Space.Ship.transform.position;

                Vector3 thisPos = transform.position;

                if (Vector3.Distance(thisPos, playerPos) > m_withinDistance)
                {
                    // we are out of visiblity distance
                    if (m_running)
                        DisableObj();
                }
                else
                {
                    // within visual range
                    if (!m_running)
                        EnableObj();
                }
                yield return null;
            }
        }

        #endregion 
    }
}