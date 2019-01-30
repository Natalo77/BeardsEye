using UnityEngine;

public class SteamCollisionScript : MonoBehaviour
{
    //Initializing variables that will store aspects of the steam
    [SerializeField]                                                                                   private float steamParticleLifetime;
    [SerializeField]                                                                                   private float steamSpeed;
    [SerializeField]                                                                                   private float steamParticleSize;
    [SerializeField]                                                                                   private Color steamColour;
    [SerializeField] [Tooltip ("The distance the steam can travel before dissapearing")]               private float steamRadius;
    [SerializeField] [Tooltip ("The arc that steam can travel in")]                                    private float steamArc;
    [SerializeField] [Tooltip ("The gradient (colour & opacity) of the steam")]                        private Gradient steamGradient;
    [SerializeField] [Tooltip ("The upwards force that the steam has on the player when above water")] private float verticalSteamForce;
    //[SerializeField] [Tooltip("The angle that steam travels in")] private float steamRotation;

    [Space(5)]

    //Initializing variables that will store aspects of the bubbles
    [SerializeField]                                                                                   private float bubbleParticleLifetime;
    [SerializeField]                                                                                   private float bubbleSpeed;
    [SerializeField]                                                                                   private float bubbleParticleSize;
    [SerializeField]                                                                                   private Color bubbleColour;
    [SerializeField] [Tooltip ("The distance the bubbles can travel before dissapearing")]             private float bubbleRadius;
    [SerializeField] [Tooltip ("The arc that bubbles can travel in")]                                  private float bubbleArc;
    [SerializeField] [Tooltip ("The gradient (colour & opacity) of the bubbles")]                      private Gradient bubbleGradient;
    [SerializeField] [Tooltip ("The upwards force that the bubbles has on the player when submerged")] private float verticalBubbleForce;
    //[SerializeField] [Tooltip("The angle that bubbles travel in")] private float bubbleRotation;

    //Initializing variables that will store aspects of the particle system
    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.ShapeModule shapeModule;
    private ParticleSystem.ColorOverLifetimeModule colourModule;

    //Initializing submerged, which keeps track of whether or not the particle system is underwater
    private bool submerged;

    //Validating the inputted inspector values
    void OnValidate()
    {
        if (steamParticleLifetime < 0.0001)
        {
            steamParticleLifetime = 0.0001f;
        }
        if (steamParticleSize < 0)
        {
            steamParticleSize *= -1;
        }
        if (steamRadius < 0.01)
        {
            steamRadius = 0.01f;
        }
        if (steamArc < 0)
        {
            steamArc *= -1;
        }

        if (bubbleParticleLifetime < 0.0001)
        {
            bubbleParticleLifetime = 0.0001f;
        }
        if (bubbleParticleSize < 0)
        {
            bubbleParticleSize *= -1;
        }
        if (bubbleRadius < 0.01)
        {
            bubbleRadius = 0.01f;
        }
        if (bubbleArc < 0)
        {
            bubbleArc *= -1;
        }
    }

    void Start()
    {
        ParticleSystem system = GetComponent<ParticleSystem>();
        mainModule = system.main;
        shapeModule = system.shape;
        colourModule = system.colorOverLifetime;
    }

    /*Iterating through the colliders of all water objects, and checking if the particle system is inside any of them.
    If it is, 'submerged' is set to true, the aspects of the particle system are set to those of 'bubbles',
    and Update() stops until the next frame (thanks to 'return;').
    If it isn't, 'submerged' is set to false, and the aspects of the particle system are set to those of 'steam'.*/
    void Update()
    {
        foreach (GameObject water in GameObject.FindGameObjectsWithTag("Water"))
        {
            foreach (Collider2D waterCollider in water.GetComponents<Collider2D>())
            {
                if (waterCollider.OverlapPoint(transform.position))
                {
                    submerged = true;
                    mainModule.startLifetime = bubbleParticleLifetime;
                    mainModule.startSpeed = bubbleSpeed;
                    mainModule.startSize = bubbleParticleSize;
                    mainModule.startColor = bubbleColour;
                    shapeModule.radius = bubbleRadius;
                    shapeModule.arc = bubbleArc;
                    //shapeModule.rotation = new Vector3(shapeModule.rotation.x, shapeModule.rotation.y, bubbleRotation);
                    colourModule.color = new ParticleSystem.MinMaxGradient(bubbleGradient);
                    return;
                }
                else
                {
                    submerged = false;
                    mainModule.startLifetime = steamParticleLifetime;
                    mainModule.startSpeed = steamSpeed;
                    mainModule.startSize = steamParticleSize;
                    mainModule.startColor = steamColour;
                    shapeModule.radius = steamRadius;
                    shapeModule.arc = steamArc;
                    //shapeModule.rotation = new Vector3(shapeModule.rotation.x, shapeModule.rotation.y, steamRotation);
                    colourModule.color = new ParticleSystem.MinMaxGradient(steamGradient);
                }
            }
        }
    }

    /*Adding a force (power is based on whether the particle system is underwater or not)
    to the player's Y-axis on collision with a particle from the particle system*/
    void OnParticleCollision(GameObject other)
    {
        Rigidbody2D rigidbody = other.GetComponent<Rigidbody2D>();
        if (submerged)
        {
            //rigidbody.AddForce(new Vector2(rigidbody.velocity.x, verticalBubbleForce));
            rigidbody.AddForce((other.transform.position - transform.position).normalized * verticalBubbleForce);
        }
        else
        {
            //rigidbody.AddForce(new Vector2(rigidbody.velocity.x, verticalSteamForce));
            rigidbody.AddForce((other.transform.position - transform.position).normalized * verticalSteamForce);
        }
    }
}