using System.Collections;
using UnityEngine;

public class PlungerHit : MonoBehaviour
{
    private Rigidbody2D plunger;
    private Animator plungerAnimator;
    private PlungerRotate rotateScript;
    private DestroyAfterTime destroyScript;
    private bool isAttachedToLeak = false;
    public bool isAttachedToLevel = false;
    private bool wasPlugged = false;

    public float reboundForce;
    public float plungerLifeTime;
    [HideInInspector] public Vector2 directionOfTravel;

    private AudioSource audioSource;

    void Start()
    {
        /*
            Get a reference to the Plunger's ridigbody
            Get a reference to the Plunger's Rotate script
            Get a reference to the Plunger's Animator
            Set collisions between the 'Plunger' layer and 'Player' layer to be ignored.
        */

        plunger = GetComponent<Rigidbody2D>();
        rotateScript = GetComponent<PlungerRotate>();
        plungerAnimator = GetComponent<Animator>();

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Plunger"), LayerMask.NameToLayer("Player"));

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("PlungerDrop"), LayerMask.NameToLayer("Level"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("PlungerDrop"), LayerMask.NameToLayer("Leak"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("PlungerDrop"), LayerMask.NameToLayer("PluggedLeak"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("PlungerDrop"), LayerMask.NameToLayer("Plunger"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("PlungerDrop"), LayerMask.NameToLayer("PlungerDrop"));

        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        /*  Tl;Dr:
          
            When a plunger collides with something:
                If it's a leak, stick to it, plug it, and begin the PlungerLifetimeDecay method.
                If it's a level, stick to it, and enable the DestroyAfterTime script.
                If it's a plunger or a pluggedleak, then check to make sure the plunger's not attached to anything before calling the Rebound method.
        */

        int layer = collision.gameObject.layer;
        if(layer == LayerMask.NameToLayer("Leak") || layer == LayerMask.NameToLayer("Level") || layer == LayerMask.NameToLayer("Player"))
        {
            //Playing the 'Boing' sound effect
            audioSource.Play();

            //---These 3 lines halt the plunger and stop it from rotating---
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            plunger.isKinematic = true;
            plunger.velocity = new Vector2(0, 0);
            rotateScript.enabled = false;

            //---This section finds the closest rotate point in the object that the plunger collided with---
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            var rotatePointsObj = collision.gameObject.GetComponent<RotatePointsReferenceScript>().rotatePoints;
            Transform closest = null;
            float lowestLength = 100;
            foreach (Transform point in rotatePointsObj.transform)
            {
                float length = Vector2.SqrMagnitude(transform.position - point.position);
                if (length < lowestLength)
                {
                    closest = point;
                    lowestLength = length;
                }
            }

            //---This section moves the plunger back if it's too close to a rotate point---
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            float distance = Vector2.Distance(closest.position, plunger.transform.position);
            if(distance < 0.935f)
            {
                Vector2 distanceToMove = -directionOfTravel.normalized * distance;
                plunger.transform.position = new Vector3(plunger.transform.position.x + distanceToMove.x,
                                                         plunger.transform.position.y + distanceToMove.y,
                                                         plunger.transform.position.z);
            }

            //---This section rotates the plunger towards the rotate point---
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            plunger.transform.up = closest.position - plunger.transform.position;
            plunger.transform.eulerAngles = new Vector3(0, 0, plunger.transform.eulerAngles.z);

            //---This section makes the plunger a child of the Player if it collided with the player, then disables it's colliders---
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            if(collision.gameObject.tag == "Player")
            {
                GetComponent<Transform>().parent = collision.transform;
                DisablePlungerColliders();
            }

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //---This section moves a plunger a quarter closer to a rotate point provided the object is not a standalone pipe platform---
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            if (collision.gameObject.tag != "Platform" && collision.gameObject.tag != "Player")
            {
                Vector3 diff = new Vector3(((closest.position.x - plunger.transform.position.x) / 4), ((closest.position.y - plunger.transform.position.y) / 4), 0);
                plunger.transform.position = new Vector3((plunger.transform.position.x + diff.x), (plunger.transform.position.y + diff.y), plunger.transform.position.z);
            }

            //---This triggers the animator to play the plunger's hit animation---
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            plungerAnimator.SetTrigger("Hit");

            if (layer == LayerMask.NameToLayer("Level"))
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //---This section runs the code for a plunger hitting a level---
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            {
                isAttachedToLevel = true;
                StartCoroutine(PlungerLevelDecay());
            }
            else if (layer != LayerMask.NameToLayer("Player"))
            {
                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //---This section changes some state variables---
                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                collision.gameObject.layer = LayerMask.NameToLayer("PluggedLeak");
                isAttachedToLeak = true;

                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                //---This section 'plugs' the leak that was collided with---
                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                foreach (Transform child in collision.gameObject.transform)
                {
                    if (child.gameObject.name.Contains("Leak"))
                    {
                        LeakScript childLeakScript = child.gameObject.GetComponent<LeakScript>();
                        if (childLeakScript.phaseTransitionCounter > 1)
                        {
                            childLeakScript.phaseTransitionCounter--;
                        }
                        //child.gameObject.SetActive(false);
                        ParticleSystem.EmissionModule emission = child.GetComponent<ParticleSystem>().emission;
                        if (emission.enabled == true)
                        {
                            emission.enabled = false;
                            wasPlugged = true;
                        }
                        child.GetComponent<LeakScript>().enabled = false;
                        childLeakScript.isPlugged = true;
                    }
                }

                //---This updates gameplay stats---
                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                GameOverUIManager.leaksPlunged++;

                //---This runs the code for the plunger colliding with a leak---
                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                StartCoroutine(PlungerLifetimeDecay(collision));
                
            }
        }
        else if(layer == LayerMask.NameToLayer("Plunger") || layer == LayerMask.NameToLayer("PluggedLeak"))
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //---This section prevents plungers already attached to objects from rebounding---------
        //---And forces plungers that collide with other plungers or plugged leaks to rebound---
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        {
            if (!isAttachedToLeak && !isAttachedToLevel)
            {
                Rebound();
            }
        }

    }

    public void Rebound()
    {
        plunger.gameObject.layer = LayerMask.NameToLayer("PlungerDrop");

        plunger.velocity = new Vector2(0, 0);

        //---This section allows the plunger to be affected by gravity again---
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        plunger.isKinematic = true;
        plunger.Sleep();
        plunger.gravityScale = 1;
        plunger.isKinematic = false;
        plunger.WakeUp();

        //---This adds the rebound force---
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        plunger.AddForce(new Vector2(-directionOfTravel.x * reboundForce, -directionOfTravel.y * reboundForce));

        //---This section marks the gameObject for deletion---
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        EnableDestroyScript();
    }

    private void DisablePlungerColliders()
    {
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //---This section gains a reference to the list of plunger colliders and then disable them---
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        BoxCollider2D[] plungerColliders = GetComponents<BoxCollider2D>();
        foreach (BoxCollider2D collider in plungerColliders)
        {
            collider.enabled = false;
        }
    }

    private void EnableDestroyScript()
    {
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //---This section marks the plunger's gameObject for deletion---
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        destroyScript = GetComponent<DestroyAfterTime>();
        if (destroyScript.enabled == false)
        {
            destroyScript.enabled = true;
        }
    }

    IEnumerator PlungerLifetimeDecay(Collision2D collision)
    {
        yield return new WaitForSeconds(plungerLifeTime);

        //---This section 're-opens' the leak that was collided with---
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        foreach (Transform child in collision.gameObject.transform)
        {
            if (child.gameObject.name.Contains("Leak"))
            {
                //child.gameObject.SetActive(true);
                ParticleSystem.EmissionModule emission = child.GetComponent<ParticleSystem>().emission;
                if (wasPlugged == true)
                {
                    emission.enabled = true;
                }
                child.GetComponent<LeakScript>().enabled = true;
                child.GetComponent<LeakScript>().isPlugged = false;
            }
        }

        //---This changes a state variable---
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        collision.gameObject.layer = LayerMask.NameToLayer("Leak");

        //---This gives the plunger it's movement back and makes it rebound---
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        allowMovementAndRebound();
    }

    IEnumerator PlungerLevelDecay()
    {
        yield return new WaitForSeconds(plungerLifeTime);

        //---This gives the plunger it's movement back and makes it rebound---
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        allowMovementAndRebound();
    }

    void allowMovementAndRebound()
    {
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //---This section gives the plunger it's movement back and makes it rebound---
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        plunger.isKinematic = false;
        rotateScript.enabled = true;
        Rebound();
    }
}