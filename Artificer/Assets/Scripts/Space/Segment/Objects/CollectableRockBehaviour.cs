using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// Artificer
using Data.Shared;
using Data.Space.Library;

public class CollectableRockBehaviour : MonoBehaviour
{
    Dictionary<MaterialData, float> material;

    float destroyTime = 40f;

    // Use this for initialization
    void Start()
    {
        Invoke("Die", destroyTime);
    }

    public void PopulateRandom()
    {
        material = new Dictionary<MaterialData, float>();
        for (int i = 0; i < Random.Range(1, 3); i++)
        {
            float kilo = Random.Range(.5f, .7f);
            MaterialData mat = ElementLibrary.ReturnRandomElement();
            if(material.ContainsKey(mat))
                material[mat] += kilo;
            else
                material.Add(mat, kilo);
        }
    }

    public void PopulateWeighted(string[] prospect)
    {
        material = new Dictionary<MaterialData, float>();
        for (int i = 0; i < Random.Range(1, 3); i++)
        {
            float kilo = Random.Range(.5f, .7f);
            MaterialData mat = 
                ElementLibrary.ReturnRandomElementWeighted(prospect);
            if(material.ContainsKey(mat))
                material[mat] += kilo;
            else
                material.Add(mat, kilo);
        }
    }
	
    // Update is called once per frame
    void Update()
    {
	    
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.tag == "Body")
        {
            col.transform.parent.
                gameObject.SendMessage("AddMaterial", material,
                       SendMessageOptions.DontRequireReceiver);

            GameObject.Find("space").SendMessage("MaterialCollected", material,
                SendMessageOptions.DontRequireReceiver);

            Destroy(this.gameObject);
        }
    }

    void Die()
    {
        Destroy(this.gameObject);
    }
}

