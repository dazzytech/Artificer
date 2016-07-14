using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using ShipComponents;
using Space;

public class ShieldController : MonoBehaviour
{
    private ShieldParticle _vfx;

    void Awake()
    {
        _vfx = GetComponent<ShieldParticle>();
    }

    public void ConstructShield(float radius)
    {
        _vfx.Radius = radius;
    }

    public void DestroyShield()
    {
        _vfx.BuildShield();
        Invoke("End", .1f);
    }

    public void End()
    {
        gameObject.SetActive(false);
    }

    public void UpdateColour(float perc)
    {
        // change color depending on shield percentage
        _vfx.ParticleColor = new Color(1.0f - perc, perc, 0f);
    }

    public void Hit(HitData hit)
    {
        // Pass hit position onto particle shield
        GameObject aggressor = NetworkServer.FindLocalObject(hit.originID);
        if(aggressor != null)
            _vfx.Impact(new Vector3(aggressor.transform.position.x,
                aggressor.transform.position.y));
    }

    public void HitArea(HitData hit)
    {
        Hit(hit);
    }
}

