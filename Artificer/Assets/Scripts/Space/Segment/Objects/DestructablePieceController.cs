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
        [Command]
        public void CmdSetWreckage(int[] components, NetworkInstanceId playerID)
        {
            RpcBuildWreckage(components, playerID);
        }

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
        }

        /*public float pieceDensity;
        public float maxDensity;

        //public float secondsTillRemove = 30f;

        public string[] prospect;
        // Use this for initialization
        void Start()
        {
            maxDensity = pieceDensity = (40f * transform.childCount);
        }

        void Update()
        {
            /*secondsTillRemove -= Time.deltaTime;
            if (secondsTillRemove <= 0)
                Destroy(gameObject);*/
        /*}

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
    }
}

