using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Space;

public class PlasmaController : WeaponController
{
    // The three different plasma types behave slightly different
    public enum PlasmaType{DEFAULT, COMPRESSED, FUSION};
    public PlasmaType PType;


    //VFX vars
    private ParticleSystem.Particle[] points;
    public  int pointCount = 50;
    private Vector3 projPosition;
    
    // bullet vars
    private Vector3 origTransPosition;
    public float speed = 3f;
    public float trailLength = 5f;
    float bulletStep;

    //time
    float seconds;
    float currSeconds;

    // Specific vars
    public float radius = 0f;
    public float followRadius = 0f;
    public float followTurnSpeed = 0f;
    
    // Update is called once per frame
    void Update ()
    {
        if (points == null)
            return;

        // if plasmatype is fusion then detect any transforms within a certain radius
        if (PType == PlasmaType.FUSION)
        {
            if(!FusionCurve())
            {
                // move the transform in the bullet direction
                transform.Translate((_data.Direction * speed) * Time.deltaTime);
            }
        } else
        {
            // move the transform in the bullet direction
            transform.Translate((_data.Direction * speed) * Time.deltaTime);
        }
        float travel = ((transform.position - origTransPosition).sqrMagnitude);
        travel *= bulletStep;
        
        if(travel > bulletStep)
            travel = bulletStep;
        
        for (int i = 0; i < pointCount-1; i++) {
            points[i].position = projPosition - _data.Direction*(travel*i);
        }
        GetComponent<ParticleSystem>().SetParticles ( points, points.Length );
        
        currSeconds += Time.deltaTime;
        if (currSeconds >= seconds)
            DestroyProjectile();

        RaycastHit2D[] hitList = Physics2D.RaycastAll(transform.position, -_data.Direction, speed*Time.deltaTime, maskIgnore);
        
        if (hitList.Length != 0)
        {
            foreach(RaycastHit2D hit in hitList)
            {
                if(hit.transform.Equals(_data.Self))
                {
                    continue;
                }

                SoundController.PlaySoundFXAt
                    (transform.position, ImpactSound);

                if(PType == PlasmaType.COMPRESSED)
                {
                    // Compressed plasma will explode on impact
                    Instantiate(Explode, transform.position, Quaternion.identity);
                    RaycastHit2D[] colliderList = Physics2D.CircleCastAll(transform.position, radius, Vector2.up, 0, maskIgnore);
                    foreach (RaycastHit2D hitB in colliderList)
                    {
                        if(hit.transform != null)
                        {
                            HitData hitD = new HitData();
                            hitD.damage = _data.Damage;
                            hitD.hitPosition = hitB.point;
                            hitD.radius = radius;
                            hitD.originID = _data.Self;
                            hitB.transform.gameObject.SendMessage("HitArea", hitD, SendMessageOptions.DontRequireReceiver);
                        }
                        DestroyProjectile();
                        break;
                    }
                }
                else
                {
                    HitData hitD = new HitData();
                    hitD.damage = _data.Damage;
                    hitD.hitPosition = hit.point;
                    hitD.originID = _data.Self;
                    hit.transform.gameObject.SendMessage("Hit", hitD, SendMessageOptions.DontRequireReceiver);
                    DestroyProjectile();
                    Instantiate(Explode, hit.point, 
                            Quaternion.Euler(-_data.Direction));
                    break;
                }
            }
        }
    }

    /// <summary>
    /// curves the plasma bolt to an object.
    /// </summary>
    /// <returns><c>true</c>, if curve was successful, <c>false</c> otherwise.</returns>
    private bool FusionCurve()
    {
        // Get home alignment
        if (_data.Self == null)
        {
            Destroy(gameObject);
            return false;
        }
        string align = NetworkServer.FindLocalObject(_data.Self).GetComponent<ShipAttributes>().AlignmentLabel;
        // use followradius to find an object within range
        RaycastHit2D collider = Physics2D.CircleCast(transform.position, followRadius, Vector2.up, 0, maskIgnore);
        
        if(collider.transform != null)
            if(!collider.transform.Equals(_data.Self))
            {
                // Retreive ship info
                ShipAttributes sa = collider.transform.GetComponent<ShipAttributes>();
                if(sa != null)
                    if(sa.AlignmentLabel == "Enemy" && align != "Enemy" ||
                       sa.AlignmentLabel != "Enemy" && align == "Enemy")
                    {
                        Vector3 newDir = collider.transform.position - transform.position;
                        newDir.z = 0;
                        Vector3 dir = Vector3.Slerp(_data.Direction, newDir, Time.deltaTime * (followTurnSpeed * (10f - newDir.magnitude)));
                        transform.Translate((dir.normalized * speed) * Time.deltaTime);
                        _data.Direction = dir;

                        return true;
                    }
            }
        return false;
    }
    
    /// <summary>
    /// Creates the bullet appearance then.
    /// 
    /// </summary>
    /// <param name="direction">Direction.</param>
    /// <param name="range">Range.</param>
    public override void CreateProjectile(WeaponData data)
    {
        base.CreateProjectile(data);
        Instantiate(Muzzle, transform.position + (_data.Direction * 1f) * Time.deltaTime,
                    Quaternion.LookRotation(_data.Direction));

        transform.Translate(_data.Direction * Time.deltaTime);

        seconds = data.Distance / speed;
        currSeconds = 0;
        
        GetComponent<ParticleSystem>().simulationSpace 
            = ParticleSystemSimulationSpace.Local;
        points = new ParticleSystem.Particle[pointCount];
        
        // First create the main projectile particle
        projPosition = Vector3.zero;
        points[pointCount-1].position = projPosition;
        points[pointCount-1].color = new Color(1f,1f,1f, 1f);
        points[pointCount-1].size = .2f;
        
        // Figure out a bullet step
        bulletStep = trailLength / (pointCount-1);
        
        for (int i = 0; i < pointCount-1; i++) {
            points[i].position = projPosition;
            points[i].color = Color.Lerp(points[pointCount-1].color , new Color(0.66f,0.164f,0.015f, 1f), bulletStep*i);
            points[i].size = points[pointCount-1].size -(points[pointCount-1].size/pointCount)*i;
        }
        GetComponent<ParticleSystem>().SetParticles ( points, points.Length );
        origTransPosition = transform.position;
    }
}

