using UnityEngine;

public class WaterCollisionScript : MonoBehaviour
{
    void Update()
    {
        if (transform.position.y < 0)
        {
            GameObject[] allObjects = FindObjectsOfType(typeof(GameObject)) as GameObject[];
            foreach (GameObject particleSystem in allObjects)
            {
                if (particleSystem.layer == LayerMask.NameToLayer("Leak"))
                {
                    ParticleSystem.Particle[] particleList = new ParticleSystem.Particle[particleSystem.GetComponent<ParticleSystem>().particleCount];
                    int activeParticles = particleSystem.GetComponent<ParticleSystem>().GetParticles(particleList);
                    for (int i = 0; i < activeParticles; i++)
                    {
                        if (GetComponent<Collider2D>().OverlapPoint(particleList[i].position))
                        {
                            var waterPosition = transform.position;
                            if (particleList[i].startSize == particleSystem.GetComponent<ParticleSystem>().GetComponent<LeakScript>().dropletSize * particleSystem.GetComponent<ParticleSystem>().GetComponent<LeakScript>().bigDropletSizeIncrease)
                            {
                                waterPosition.y += particleSystem.GetComponent<ParticleSystem>().GetComponent<LeakScript>().waterIncrease * particleSystem.GetComponent<ParticleSystem>().GetComponent<LeakScript>().bigDropletSizeIncrease;
                            }
                            else
                            {
                                waterPosition.y += particleSystem.GetComponent<ParticleSystem>().GetComponent<LeakScript>().waterIncrease;
                            }
                            transform.position = waterPosition;
                            particleList[i].remainingLifetime = 0;
                        }
                    }
                    particleSystem.GetComponent<ParticleSystem>().SetParticles(particleList, particleSystem.GetComponent<ParticleSystem>().particleCount);
                }
            }
        }
    }
}