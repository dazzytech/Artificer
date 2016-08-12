using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
//Artificer
using Data.Space.Library;
using Space.Segment;

public class AsteroidBehaviour : ImpactCollider
{

    #region ATTRIUTES

    [SyncVar]
    private float _rockDensity;
    private float _maxDensity;
    [SyncVar]
    private NetworkInstanceId _parentID;
    [SyncVar]
    private float _scale;

    public float withinDistance;
    public bool running;

    protected Animator anim;

    //public string[] prospect;         Sync list string

    #endregion

    #region MONO BEHAVIOUR

    void Start()
    {
        // Check if we have been intialized by server
        if (!_parentID.IsEmpty())
            InitializeAsteroid();
    }

    void OnDestroy()
    {
        if (transform.parent != null)
        {
            transform.parent.GetComponent<SegmentObjectBehaviour>().ObjEnable
                -= EnableObj;

            transform.parent.GetComponent<SegmentObjectBehaviour>().ObjDisable
                -= DisableObj;
        }
    }

    #endregion

    #region SERVER INTERACTION

    /// <summary>
    /// Given parameters from server
    /// </summary>
    /// <param name="scale"></param>
    /// <param name="parentID"></param>
    [Server]
    public void InitializeParameters(float scale, NetworkInstanceId parentID)
    {
        _scale = scale;
        _rockDensity = 20f * _scale;
        _parentID = parentID;

        InitializeAsteroid();
    }

    #endregion

    #region GAME OBJECT UTILITIES

    /// <summary>
    /// Use the parameters to create a working asteroid on
    /// client and server
    /// </summary>
    private void InitializeAsteroid()
    {
        transform.parent = ClientScene.FindLocalObject(_parentID).transform;

        transform.parent.GetComponent<SegmentObjectBehaviour>().ObjEnable
            += EnableObj;

        transform.parent.GetComponent<SegmentObjectBehaviour>().ObjDisable
            += DisableObj;

        transform.localScale =
                    new Vector3(_scale,
                                _scale, 1f);

        GetComponent<Rigidbody2D>().mass = _scale;

        _maxDensity = _rockDensity;

        //StartCoroutine("PopCheck");
    }

    private void EnableObj()
    {
        if (this == null)
        {
            return;
        }

        // Parent isnt enabled but we are 
        GetComponent<NetworkTransform>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;

        foreach (CircleCollider2D c in GetComponents<CircleCollider2D>())
            c.enabled = true;

        running = true;
        //StartCoroutine("PopCheck");
    }

    private void DisableObj()
    {
        if (this == null)
        {
            return;             //fix for something not understood
        }

        // Parent is enabled but we are not
        GetComponent<NetworkTransform>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        foreach (CircleCollider2D c in GetComponents<CircleCollider2D>())
            c.enabled = false;

        running = false;
    }

    #endregion

    #region COROUTINES

   /* private IEnumerator PopCheck()
    {
        // for now just create infinite loop
        while (running)
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

            if (Vector3.Distance(thisPos, playerPos) > withinDistance)
            {
                // Parent isnt enabled but we are 
                GetComponent<NetworkTransform>().enabled = false;
                GetComponent<SpriteRenderer>().enabled = false;

                foreach (CircleCollider2D c in GetComponents<CircleCollider2D>())
                    c.enabled = false;
            }
            else
            {
                if(!GetComponent<NetworkTransform>().enabled)
                {
                    // Parent is enabled but we are not
                    GetComponent<NetworkTransform>().enabled = true;
                    GetComponent<SpriteRenderer>().enabled = true;
                    foreach (CircleCollider2D c in GetComponents<CircleCollider2D>())
                        c.enabled = true;
                }
            }
            yield return null;
        }
    }*/

    #endregion

    #region IMPACT COLLISION

    void OnCollisionEnter2D(Collision2D other)
	{
        if (other.transform.gameObject.tag != "Station")
        {
            Vector2 dir = transform.position - other.transform.position;
            dir = other.transform.InverseTransformDirection(dir);
            float magnitude = dir.sqrMagnitude;
            GetComponent<Rigidbody2D>().AddForce(dir * magnitude, ForceMode2D.Force);

            CmdAsteroidDmg(other);
        }
	}

    public void CmdAsteroidDmg(Collision2D other)
    {
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

    [ClientRpc]
    public override void RpcHitArea()
    {
        RpcHit();
    }

    [ClientRpc]
    public override void RpcHit()
    {
        if (!isServer)
            return;

        _rockDensity -= _hitD.damage;

        float dmgPerc = _hitD.damage / _maxDensity;

        int numOfRocks = Mathf.CeilToInt(
            (_maxDensity*dmgPerc)*0.2f) 
            + Random.Range(0, 4);
        
        for(int i = 0; i < numOfRocks; i++)
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
            col.isTrigger = true;*/
        }

        Vector3 scale = this.transform.localScale * 0.9f;
        this.transform.localScale = scale;

        if (_rockDensity <= 0)
        {
            NetworkServer.UnSpawn(this.gameObject);

            // for now just destroy
            Destroy(this.gameObject);
        }
    }

    #endregion
}

