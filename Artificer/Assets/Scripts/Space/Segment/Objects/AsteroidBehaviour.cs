using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
//Artificer
using Data.Space.Library;

public class AsteroidBehaviour : NetworkBehaviour
{
    /*public float rockDensity; 
    protected float maxDensity;
    public string[] prospect;

    protected Animator anim;

    void Start()
    {
        maxDensity = rockDensity;
        anim = GetComponent<Animator>();
    }

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

