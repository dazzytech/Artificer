using UnityEngine;
using System.Collections;

/// <summary>
/// Shield particle system.
/// creates a sheild of a defined radius 
/// placing particles in a circular area
/// </summary>
public class ShieldParticle : MonoBehaviour {

    private ParticleSystem.Particle[] points;

    // Dimesions and particle count
    private int NumParticles = 360;
    private int FadeParticles = 360;
    public float Radius = 2f;

    [HideInInspector]
    public Color ParticleColor;

    void Awake()
    {
        points = new ParticleSystem.Particle[NumParticles + FadeParticles];
    }

    void OnEnable()
    {
        ParticleColor = new Color(0f, 1f, 0f);
        BuildShield();
    }
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        transform.up = Vector3.up;
	}

    void LateUpdate () 
    {
        if (points != null)
        {
            UpdateParticles();
            UpdateFadeParticles();
        }
    }

    public void Impact(Vector3 position)
    {
        float angle = (int)Angle(transform, position);
        angle += 90;
        // Display particles for 15 degrees each direction
        for (int i = (int)angle - 30; i < (int)angle + 30; i++)
        {
            int index = i;
            if(i < 0)
                index += 360;

            if(i >= 360)
                index -= 360;

            //print(index);

            points[index].size = .2f;
            points[index].color = ParticleColor;
        }
        GetComponent<ParticleSystem>().SetParticles(points, points.Length);
    }

    private void UpdateParticles()
    {
        for (int i = 0; i < NumParticles; i++)
        {
            if(points[i].size > 0.01f)
            {
                points[i].size *= 0.98f;
                //Vector3 col = new Vector3(points[i].color.r*.98f, points[i].color.g*.98f, points[i].color.b*.98f);

                //points[i].color = new Color(col.x, col.y, col.z);
            }
            else
            {
                // dissapear
                points[i].size = 0;
            }
        }
        GetComponent<ParticleSystem>().SetParticles(points, points.Length);
    }



    /// <summary>
    /// Builds the shield.
    /// initializes the particles in a circle
    /// </summary>
    public void BuildShield()
    {
        for(int i = 0; i < NumParticles; i++)
        {
            Vector2 position = new Vector2(Radius * Mathf.Cos(Mathf.Deg2Rad * (i - 90)), Radius * Mathf.Sin(Mathf.Deg2Rad * (i - 90)));

            points[i].position = position;
            points[i].size = .1f;
            points[i].color = ParticleColor;
        }

        GetComponent<ParticleSystem>().SetParticles ( points, points.Length );
    }

    /// <summary>
    /// Updates the fade particles.
    /// fade inwards
    /// </summary>
    private void UpdateFadeParticles()
    {
        for (int i = NumParticles, index = 0; index < NumParticles; i++, index++)
        {
            if (points [index].size > 0.01f)
            {
                if (points [i].size < 0.05f)
                {
                    CreateFadeParticle(i, index);
                }

                points [i].position = Vector2.Lerp(points [i].position, new Vector2(0, 0), Random.Range(0.0001f, 0.1f));
                points [i].size *= Random.Range(0.8f, 0.98f);
                Vector3 col = new Vector3(points[i].color.r*.98f, points[i].color.g*.98f, points[i].color.b*.98f);
                
                points[i].color = new Color(col.x, col.y, col.z);
                GetComponent<ParticleSystem>().SetParticles(points, points.Length);
            }
            else
            {
                points [i].size = 0;
            }
        }
    }

    private void CreateFadeParticle(int index, int degree)
    {
        points [index].position 
            = new Vector2(Radius * Mathf.Cos(Mathf.Deg2Rad * (degree - 90)), 
                          Radius * Mathf.Sin(Mathf.Deg2Rad * (degree - 90)));
        points [index].size = points[degree].size;
        points[index].color = points[degree].color;
    }

    /// <summary>
    /// Finds the angle difference between the two objects.
    /// </summary>
    /// <returns>The angle difference.</returns>
    /// <param name="trans">Trans.</param>
    /// <param name="dest">Destination.</param>
    float Angle(Transform trans, Vector3 dest)
    {
        Vector2 pos = trans.position;
        float angle = Mathf.Atan2(dest.y-pos.y, dest.x-pos.x)*180 / Mathf.PI;
        return angle;
    }
}
