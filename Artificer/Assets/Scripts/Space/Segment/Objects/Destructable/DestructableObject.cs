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

        private void Start()
        {
            // retreive texture from sprite
            // renderer
            SpriteRenderer m_objSprite = GetComponent<SpriteRenderer>();
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
            m_pieceDensity -= hData.damage;



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