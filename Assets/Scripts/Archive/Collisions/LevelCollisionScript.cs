using UnityEngine;

public class LevelCollisionScript : MonoBehaviour
{
    void OnParticleCollision(GameObject other)
    {
        //other.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
        //other.transform.position = new Vector3(other.transform.position.x, other.transform.position.y - 3, other.transform.position.z);
        ////var particle = other.GetComponent<ParticleSystem.Particle>();
        /*if (!GetComponent<Collider2D>().OverlapPoint(particle.position - (particle.velocity * Time.deltaTime)))
        {
            particle.velocity = new Vector3(0, 0, 0);
        }*/

        /*Destroy(other);
        if (!GetComponent<Collider2D>().OverlapPoint((Vector2)other.transform.position - (other.GetComponent<Rigidbody2D>().velocity * Time.deltaTime)))
        {
            other.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
        }*/
        ParticleSystem.Particle[] particleList = new ParticleSystem.Particle[other.GetComponent<ParticleSystem>().particleCount];
        int activeParticles = other.GetComponent<ParticleSystem>().GetParticles(particleList);
        /*foreach (ParticleSystem.Particle particle in particleList)
        {
            //if (GetComponent<Collider2D>().OverlapPoint(particle.transform.position
            if (GetComponent<Collider2D>().OverlapPoint((Vector2)particle.position))
            {
                if (!GetComponent<Collider2D>().OverlapPoint((Vector2)particle.position - (Vector2)(particle.velocity * Time.deltaTime)))
                {
                    particle.velocity = new Vector3(0, 0, 0);
                }
            }
        }*/
        for (int i = 0; i < particleList.Length; i++)
        {
            if (GetComponent<Collider2D>().OverlapPoint((Vector2)particleList[i].position))
            {
                if (!GetComponent<Collider2D>().OverlapPoint((Vector2)particleList[i].position - (Vector2)(particleList[i].velocity * Time.deltaTime)))
                {
                    particleList[i].velocity = new Vector3(0, 0, 0);
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        print(collision);
    }
}