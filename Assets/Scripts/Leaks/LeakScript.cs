using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Manages an individual particle system
Responsible for cycling through the system's phases and performing actions upon collisions*/
public class LeakScript : MonoBehaviour
{
    //Initializing variables that will store aspects of the particle system and sprite sheet
    private ParticleSystem leak;
    private ParticleSystem.MainModule leakMain;
    private SpriteRenderer leakSpriteRenderer;
    [SerializeField] [Tooltip("Frames for each phase go in here")] private Sprite[] leakSprite;

    //Initializing variables that will store values for each phase
    private const string phaseTooltipString = "Size: 4\nElement 0: The number of seconds the phase lasts\nElement 1: MainModule - 'Duration'\nElement 2: MainModule - 'Start Speed'\nElement 3: MainModule - 'Simulation Speed'";
    [Space(5)]
    [SerializeField] [Tooltip(phaseTooltipString)] private float[] phaseOne;
    [SerializeField] [Tooltip(phaseTooltipString)] private float[] phaseTwo;
    [SerializeField] [Tooltip(phaseTooltipString)] private float[] phaseThree;
    [SerializeField] [Tooltip(phaseTooltipString)] private float[] phaseFour;
    [SerializeField] [Tooltip(phaseTooltipString)] private float[] phaseFive;
    [SerializeField] [Tooltip(phaseTooltipString)] private float[] phaseSix;
    [SerializeField] [Tooltip(phaseTooltipString)] private float[] phaseSeven;
    [SerializeField] [Tooltip(phaseTooltipString)] private float[] phaseEight;
    private float[][] phases;
    /*[HideInInspector]*/ public int phaseTransitionCounter;
    [HideInInspector] public bool isPlugged = false;

    //Initializing variables that will store values for the regular and big droplets
    [HideInInspector] public float dropletSize;
    [Space(5)]
    //[SerializeField] [Tooltip("How many times bigger a big droplet is than a regular droplet")] private float bigDropletSizeIncrease;
    [Tooltip("How many times bigger a big droplet is than a regular droplet")] public float bigDropletSizeIncrease;
    [SerializeField] [Range(0.0f, 100.0f)] [Tooltip("The percentage chance of a big droplet spawning")] private float bigDropletChance;
    [SerializeField] [Tooltip("How many seconds before a big droplet has another chance of spawning")] private float bigDropletChanceDelay;
    private float dynamicBigDropletChanceDelay;
    private float bigDropletDelayTimer;

    //Initializing waterIncrease that will store how much the water level increases by per droplet (scales with bigDropletSizeIncrease)
    [Space(5)]
    //[SerializeField] [Tooltip("How much the water level increases by per droplet\n(scales with 'Big Droplet Size Increase')")] private float waterIncrease;
    [Tooltip("How much the water level increases by per droplet\n(scales with 'Big Droplet Size Increase')")] public float waterIncrease;

    //Initializing variables that will store values related to the player 'slipping' on collision with droplets
    [Space(5)]
    [SerializeField] [Tooltip("How long the player 'slips' for on collision with a droplet")] public float slipTime;
    [SerializeField] [Tooltip("How long after 'slipping' the player is immune from 'slipping' for")] public float slipCooldownTime;

    //Initializing the audio source
    private AudioSource audioSource;

    //reference to player animator.
    public GameObject player;
    private Animator animator;

    //Validating the inputted inspector values
    void OnValidate()
    {
        foreach (float[] property in new float[][]{phaseOne, phaseTwo, phaseThree, phaseFour, phaseFive, phaseSix, phaseSeven, phaseEight})
        {
            for (int i = 0; i < property.Length; i++)
            {
                if (property[i] < 0)
                {
                    property[i] *= -1;
                }
            }
        }
        if (bigDropletSizeIncrease < 0)
        {
            bigDropletSizeIncrease *= -1;
        }
        if (bigDropletSizeIncrease < 1)
        {
            bigDropletSizeIncrease = 1;
        }
        if (bigDropletChanceDelay < 0)
        {
            bigDropletChanceDelay *= -1;
        }
        if (waterIncrease < 0)
        {
            waterIncrease *= -1;
        }
        if (slipTime < 0)
        {
            slipTime *= -1;
        }
        if (slipCooldownTime < 0)
        {
            slipCooldownTime *= -1;
        }
    }

    void Start()
    {
        //leakManager = GetComponentInParent<LeakManagerScript>();
        //slipTime = leakManager.slipTime;
        //slipCooldownTime = leakManager.slipCooldownTime;
        leak = GetComponent<ParticleSystem>();
        leakMain = leak.main;
        //leakEmission is used to set the parameters of 'Bursts' to what they should be, as there's a bug where these get reset
        leak.emission.SetBursts(new ParticleSystem.Burst[]{new ParticleSystem.Burst(0.00f, 1, 1, 0, 1.00f)});
        leakSpriteRenderer = transform.parent.gameObject.GetComponent<SpriteRenderer>();
        dropletSize = leakMain.startSize.constant;
        dynamicBigDropletChanceDelay = bigDropletChanceDelay;
        bigDropletDelayTimer = leakMain.duration / leakMain.simulationSpeed;
        //Adding the collider/s of all objects with the tags "Platform" and "Water" to the 'Colliders' parameter of 'Triggers'
        var collidersList = GameObject.FindGameObjectsWithTag("Player").Concat(GameObject.FindGameObjectsWithTag("Platform")).Concat(GameObject.FindGameObjectsWithTag("Water")).ToArray();
        for (int i = 0; i < collidersList.Length; i++)
        {
            foreach (Collider2D collider in collidersList[i].GetComponents<Collider2D>())
            {
                leak.trigger.SetCollider(i, collider);
            }
        }
        phases = new float[][]{phaseOne, phaseTwo, phaseThree, phaseFour, phaseFive, phaseSix, phaseSeven, phaseEight};
        audioSource = GetComponent<AudioSource>();
        animator = player.GetComponent<Animator>();
        StartCoroutine(PhaseTransitioner());
    }

    void FixedUpdate()
    {
        /*Turning the regular droplet into a big droplet and back if enough time has passed (dynamicBigDropletChanceDelay)
        and if bigDropletChance is greater than/equal to a randomly generated number (RandomNumberScript)*/
        if (dynamicBigDropletChanceDelay <= 0)
        {
            if (RandomNumberScript.GenerateNumber(0, 100) <= bigDropletChance)
            {
                leakMain.startSize = dropletSize * bigDropletSizeIncrease;
                bigDropletDelayTimer -= Time.fixedDeltaTime;
                if (bigDropletDelayTimer <= 0)
                {
                    leakMain.startSize = dropletSize;
                    dynamicBigDropletChanceDelay = bigDropletChanceDelay;
                }
            }
        }
        else
        {
            bigDropletDelayTimer = leakMain.duration / leakMain.simulationSpeed;
            dynamicBigDropletChanceDelay -= Time.fixedDeltaTime;
        }
    }

    /*Runs when a droplet from this particle system collides with another collider.
    Two for loops, one for droplets entering colliders, one for droplets inside colliders.
    Uses OverlapPointAll to identify all colliders at the position of the droplet.
    Checks the tags of these colliders to determine which actions to perform.
    */
    void OnParticleTrigger()
    {
        onDropletCollision();
    }

    void onDropletCollision()
    {
        List<ParticleSystem.Particle> particleList = new List<ParticleSystem.Particle>();

        int triggerParticlesEnter = leak.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, particleList);
        for (int i = 0; i < triggerParticlesEnter; i++)
        {
            ParticleSystem.Particle currentParticle = particleList[i];
            Collider2D[] colliders = Physics2D.OverlapPointAll(particleList[i].position);
            foreach (Collider2D collider in colliders)
            {
                if (collider.tag == "Player")
                {
                    StartCoroutine(PlayerSlip(collider.gameObject, currentParticle.GetCurrentSize(leak) / dropletSize));
                    currentParticle.remainingLifetime = 0;
                    particleList[i] = currentParticle;
                }
            }
        }
        leak.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, particleList);

        int triggerParticlesInside = leak.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, particleList);
        for (int j = 0; j < triggerParticlesInside; j++)
        {
            ParticleSystem.Particle currentParticle = particleList[j];
            Collider2D[] colliders = Physics2D.OverlapPointAll(particleList[j].position);
            foreach (Collider2D collider in colliders)
            {
                if (collider.tag == "Water")
                {
                    var waterPosition = collider.transform.position;
                    if (waterPosition.y < 0)
                    {
                        if (currentParticle.startSize == dropletSize * bigDropletSizeIncrease)
                        {
                            waterPosition.y += waterIncrease * bigDropletSizeIncrease;
                        }
                        else
                        {
                            waterPosition.y += waterIncrease;
                        }
                        collider.transform.position = waterPosition;
                    }
                    currentParticle.remainingLifetime = 0;
                    particleList[j] = currentParticle;
                }
            }
        }
        leak.SetTriggerParticles(ParticleSystemTriggerEventType.Inside, particleList);
    }

    //Used when a droplet collides with the player - makes the player 'slip'
    IEnumerator PlayerSlip(GameObject player, float forceModifier)
    {
        /*  Tl;Dr:
        
            Runs if the characterController isn't currently 'slipping' or in 'slip cooldown.'
            Stops the player from being able to control the character,
            and applies a slipping force (in the character's previous direction) to the player.
            Once the application of the force has ceased, the cooldown commences.
        */

        /*  Details:
        
            Get a reference to the CharacterController script.
            If the CharacterController is not currently on slip cooldown and can move:
                Set CharacterController.canMove to 0 (This ensures no movement can be made by the player's input).
                Get a reference to the character RigidBody.
                Get a copy of the character's current velocity. 
                VelocityX then stores the sign of the character's x velocity (with 0 giving a sign of 0).
                Set the character's RigidBody's physicsMaterial's friction to 0.01 and refresh the RigidBody.
                Add a gravity force.
                Add a slipping force in the direction the character was moving when they slipped.
                Wait for the amount of seconds specified by SlipTime. (Give the character rigidBody time to 'slip').
                Put the characterController on slip cooldown.
                Set the character's RigidBody's physicsMaterial's friction back to its original value of 0.4 and refresh the rigidBody.
                Set CharacterController.canMove back to its original value of 1 (allow player input to move the character again).
                Wait for the specified cooldown timer to expire.
                Take the characterController off slip cooldown.
        */

        

        var playerController = player.GetComponent<CustomPlayerController>();

        if (playerController.slipCooldown == false && playerController.canMove == 1)
        {
            audioSource.Play();

            playerController.canMove = 0;
            PlungerHit[] plungers = player.GetComponentsInChildren<PlungerHit>();
            if(plungers.Count() != 0)
            {
                foreach(PlungerHit script in plungers)
                {
                    script.Rebound();
                }
            }
            animator.SetTrigger("slip");

            var playerRigidbody = player.GetComponent<Rigidbody2D>();

            var playerVelocity = playerRigidbody.velocity;
            float playerVelocityX = playerVelocity.x == 0 ? 0 : Mathf.Sign(playerVelocity.x);

            playerRigidbody.isKinematic = true;
            playerRigidbody.Sleep();
            playerRigidbody.sharedMaterial.friction = 0.01f;
            playerRigidbody.isKinematic = false;
            playerRigidbody.WakeUp();

            playerRigidbody.velocity = new Vector2(playerVelocityX * 15, playerRigidbody.velocity.y);
            


            GameOverUIManager.timesSlipped++;

            yield return new WaitForSecondsRealtime(slipTime);
            
            playerController.slipCooldown = true;

            playerRigidbody.isKinematic = true;
            playerRigidbody.Sleep();
            playerRigidbody.sharedMaterial.friction = 0.4f;
            playerRigidbody.isKinematic = false;
            playerRigidbody.WakeUp();

            animator.SetTrigger("notSlip");
            playerController.canMove = 1;
            
            
            yield return new WaitForSeconds(slipCooldownTime);
            playerController.slipCooldown = false;
        }
    }

    IEnumerator PhaseTransitioner()
    {
        //Applying each phase to the particle system, with the wait time between each one depending on a value stored in the current phase
        /*for (phaseTransitionCounter = 0; phaseTransitionCounter < phases.Length; phaseTransitionCounter++)
        {
            leak.Stop();
            leakMain.duration = phases[phaseTransitionCounter][1];
            leakMain.startSpeed = phases[phaseTransitionCounter][2];
            leakMain.simulationSpeed = phases[phaseTransitionCounter][3];
            for (int j = 0; j < leakSprite.Length; j++)
            {
                if (phaseTransitionCounter == j)
                {
                    leakSpriteRenderer.sprite = leakSprite[j + 1];
                    break;
                }
            }
            leak.Play();

            yield return new WaitForSeconds(phases[phaseTransitionCounter][0]);
        }*/
        do
        {
            leak.Stop();
            leakMain.duration = phases[phaseTransitionCounter][1];
            leakMain.startSpeed = phases[phaseTransitionCounter][2];
            leakMain.simulationSpeed = phases[phaseTransitionCounter][3];
            for (int j = 0; j < leakSprite.Length; j++)
            {
                if (phaseTransitionCounter == j)
                {
                    leakSpriteRenderer.sprite = leakSprite[j + 1];
                    break;
                }
            }
            leak.Play();

            yield return new WaitForSeconds(phases[phaseTransitionCounter][0]);

            phaseTransitionCounter++;
        } while (phaseTransitionCounter < phases.Length);
        
    }
}