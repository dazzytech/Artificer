using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

//Artificer
using Data.Space.Library;
using Space.Segment;
using Space.Ship;
using Space.Ship.Components.Listener;

namespace Space
{
    public class DestructablePieceController : ImpactCollider
    {
        #region ATTRIBUTES

        public float pieceDensity;
        public float maxDensity;

        public float secondsTillRemove = 30f;

        #endregion

        #region MONOBEHAVIOUR

        void Start()
        {
            maxDensity = pieceDensity = (40f * transform.childCount);
        }

        void Update()
        {
           secondsTillRemove -= Time.deltaTime;
           if (secondsTillRemove <= 0)
               Destroy(gameObject);
        }

        #endregion

        #region SERVER MESSAGES

        /// <summary>
        /// Receives parameters from server to
        /// initialize debris and calls client rpc
        /// </summary>
        /// <param name="components"></param>
        /// <param name="playerID"></param>
        [Server]
        public void SetWreckage(int[] components, NetworkInstanceId playerID)
        {
            RpcBuildWreckage(components, playerID);
        }

        #endregion

        #region DEBRIS INITIALIZATION

        /// <summary>
        /// Builds wreckage on each clients using 
        /// parameters. Retieve components and activate them
        /// and place them as wreckage. if ship has no components left
        /// destroy it.
        /// </summary>
        /// <param name="components"></param>
        /// <param name="playerID"></param>
        [ClientRpc]
        public void RpcBuildWreckage(int[] components, NetworkInstanceId playerID)
        {
            GameObject player = ClientScene.FindLocalObject
                (playerID);

            if (player == null)
                return;

            ShipAttributes shipAtts = player.GetComponent<ShipAttributes>();

            if (shipAtts == null)
                return;

            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            foreach (ComponentListener listener in shipAtts.SelectedComponents(components))
            {
                shipAtts.Components.Remove(listener);
                listener.Destroy();
                listener.transform.parent = this.transform;
                rb.mass += listener.Weight;
            }

            if (shipAtts.Components.Count == 0)
                Destroy(player);
        }

        #endregion

        #region IMPACT COLLISION

        /// <summary>
        /// Client function called by server 
        /// to handle projectile hits on our object
        /// process the damage on the local object and then
        /// update the remote clients
        /// </summary>
        /// <param name="hitpoint">The vector 3D point
        /// where the projectile intesected with the colliders </param>
        [ClientRpc]
        public override void RpcHit()
        {
            pieceDensity -= _hitD.damage;

            if (pieceDensity <= 0)
            {
                // for now just destroy
                Destroy(this.gameObject);
            }

        }

        [ClientRpc]
        public override void RpcHitArea()
        {
            // Create ranged based damage in explosion

            // call hit
            RpcHit();
        }

        /*

       public string[] prospect;
        /*
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
        }

        public void HitArea(HitData hit)
        {
            Hit(hit);
        }*/

        #endregion
    }
}

