using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
//Artificer
using Data.Space.Library;

public class AsteroidBehaviour : NetworkBehaviour
{

    #region ATTRIUTES

    [SyncVar]
    private float _rockDensity;
    private float _maxDensity;
    [SyncVar]
    private NetworkInstanceId _parentID;
    [SyncVar]
    private float _scale;

    #endregion

    #region MONO BEHAVIOUR

    void Start()
    {
        // Check if we have been intialized by server
        if (!_parentID.IsEmpty())
            InitializeAsteroid();
        //anim = GetComponent<Animator>();
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

        transform.localScale =
                    new Vector3(_scale,
                                _scale, 1f);

        GetComponent<Rigidbody2D>().mass = _scale;

        _maxDensity = _rockDensity;
    }

    #endregion

    //public string[] prospect;         Sync list string
    /*
    protected Animator anim;

    

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
            other.gameObject.SendMessage("Hit", hitD, SendMessageOptions.DontRequireReceiver);
        }
	}

    public void HitArea(HitData hit)
    {
        Hit(hit);
    }

    public void Hit(HitData hit)
    {
        rockDensity -= hit.damage;

        float dmgPerc = hit.damage / maxDensity;

        int numOfRocks = Mathf.CeilToInt(
            (maxDensity*dmgPerc)*0.2f) 
            + Random.Range(0, 4);
        
        for(int i = 0; i < numOfRocks; i++)
        {
            GameObject rock = new GameObject();
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

        if (rockDensity <= 0)
        {
            // for now just destroy
            Destroy(this.gameObject);
        }
    }*/
}

