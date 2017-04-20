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
    public class DestructableObject : ImpactCollider
    {
        #region ATTRIBUTES

        [SyncVar]
        protected float m_pieceDensity;
        protected float m_maxDensity;

        // Reference to our texture
        private SpriteRenderer m_objSprite;

        #endregion

        #region MONO BEHAVIOUR

        private void Awake()
        {
            // retreive texture from sprite
            // renderer
            m_objSprite = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Apply damage to any object it
        /// hits and bumps it away
        /// </summary>
        /// <param name="other"></param>
        void OnCollisionEnter2D(Collision2D other)
        {
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

        #region VIRTUAL METHODS

        public void ApplyDamage(HitData hData)
        {
            if(isServer)
                m_pieceDensity -= hData.damage;

            if (m_objSprite != null && _hitD.damageTex != null)
            {
                // Ref to our texture rect for size
                Rect size = m_objSprite.sprite.rect;

                // Store scales
                float scaleX = transform.lossyScale.x;
                float scaleY = transform.lossyScale.y;

                // Create our mask
                Texture2D mask = Instantiate(Resources.Load("Textures/Damage/" + _hitD.damageTex,
                    typeof(Texture2D)) as Texture2D);

                TextureScale.Bilinear(mask,(int)(mask.width / scaleX), 
                     (int)(mask.height / scaleY));

                // convert hit point from center
                Vector2 localHit = (_hitD.hitPosition - transform.position);

                // scale to current size
                localHit.x /= scaleX;
                localHit.y /= scaleY;

                localHit *= m_objSprite.sprite.pixelsPerUnit;

                // origin top left
                localHit.x += size.width * .5f;
                localHit.y += size.height * .5f;

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
                // this will work cause host
                NetworkServer.UnSpawn(this.gameObject);

                // for now just destroy
                Destroy(this.gameObject);
            }
        }

        #endregion

        /*
        protected Animator anim;
        public string[] prospect;         Sync list string
        
        public void Hit(HitData hit)
        {
            pieceDensity -= hit.damage;

            if (pieceDensity <= 0)
            {
                // for now just destroy
                Destroy(this.gameObject);

                int numOfRocks = Mathf.CeilToInt(maxDensity*0.02f);

                for(int i = 0; i < numOfRocks; i++)
                {
                    GameObject rock = new GameObject();
                    SpriteRenderer rockSprite = rock.AddComponent<SpriteRenderer>();
                    rockSprite.sprite = (Sprite)Resources.Load("Textures/wreck", typeof(Sprite));
                    rock.transform.localScale = new Vector3(.5f, .5f, 1f);
                    rock.transform.position = transform.position;
                    rock.layer = 8;

                    CollectableRockBehaviour behaviour = 
                        rock.AddComponent<CollectableRockBehaviour>();
                    behaviour.PopulateWeighted(prospect);

                    Rigidbody2D rb = rock.AddComponent<Rigidbody2D>();
                    rb.AddForce(Random.insideUnitCircle*20f, ForceMode2D.Force);
                    rb.gravityScale = 0;
                    BoxCollider2D col = rock.AddComponent<BoxCollider2D>();
                    Vector3 size = new Vector3(.5f, .5f);
                    col.size = size;
                    col.isTrigger = true;
                }        
            }
        }*/

        /*public void ApplyDamage(HitData hData)
        {
            _hitD = hData;

            _rockDensity -= _hitD.damage;

            float dmgPerc = _hitD.damage / _maxDensity;

            int numOfRocks = Mathf.CeilToInt(
                (_maxDensity * dmgPerc) * 0.2f)
                + Random.Range(0, 4);

            for (int i = 0; i < numOfRocks; i++)
            {
                /*GameObject rock = new GameObject();
                SpriteRenderer rockSprite = rock.AddComponent<SpriteRenderer>();
                rockSprite.sprite = GetComponent<SpriteRenderer>().sprite;
                rock.transform.localScale = new Vector3(.5f, .5f, 1f);
                rock.transform.position = transform.position;
                rock.layer = 8;

                CollectableRockBehaviour behaviour = 
                rock.AddComponent<CollectableRockBehaviour>();
                behaviour.PopulateWeighted(prospect);

                Rigidbody2D rb = rock.AddComponent<Rigidbody2D>();
                rb.AddForce(Random.insideUnitCircle*20f, ForceMode2D.Force);
                rb.gravityScale = 0;

                BoxCollider2D col = rock.AddComponent<BoxCollider2D>();
                Vector3 size = new Vector3(.5f, .5f);
                col.size = size;
                col.isTrigger = true;
            }

            Vector3 scale = this.transform.localScale * 0.9f;
            this.transform.localScale = scale;

            if (_rockDensity <= 0)
            {
                // this will work cause host
                NetworkServer.UnSpawn(this.gameObject);

                // for now just destroy
                Destroy(this.gameObject);
            }
        }*/
    }
}