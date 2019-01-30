using System.Collections;
using UnityEngine;

public class PlayerCollisionScript : MonoBehaviour
{
    //Initializing variables that will store values related to the player 'slipping' on collision with droplets
    [Space(5)]
    [SerializeField] [Tooltip("How long the player 'slips' for on collision with a droplet")] public float slipTime;
    [SerializeField] [Tooltip("How long after 'slipping' the player is immune from 'slipping' for")] public float slipCooldownTime;

    void OnParticleCollision(GameObject other)
    {
        //Need this other.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
        //StartCoroutine(Slip(((ParticleSystem.Particle)other).GetCurrentSize(leak) / dropletSize));
        //ParticleSystem.Particle attribute = GetComponent(typeof(ParticleSystem.Particle)) as ParticleSystem.Particle;
        /*Component[] particles = other.gameObject.GetComponents(typeof(ParticleSystem.Particle));
        foreach (Component particle in particles)
        {
            print(particle);
        }*/
        //ParticleSystem.Particle particle = (ParticleSystem.Particle)other;
        //print(particle.startSize * 100);
        StartCoroutine(Slip(other.GetComponent<ParticleSystem.Particle>().GetCurrentSize(other.GetComponent<ParticleSystem>()) / other.GetComponent<ParticleSystem.Particle>().startSize));
        //var particle = other.GetComponent<ParticleSystem>().GetComponent<ParticleSystem.Particle>();
        //particle.remainingLifetime = 0;
    }

    //Used when a droplet collides with the player - makes the player 'slip'
    IEnumerator Slip(float forceModifier)
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

        var playerController = GetComponent<CustomPlayerController>();

        if (playerController.slipCooldown == false && playerController.canMove == 1)
        {
            playerController.canMove = 0;

            var playerRigidbody = GetComponent<Rigidbody2D>();

            var playerVelocity = playerRigidbody.velocity;
            float playerVelocityX = playerVelocity.x == 0 ? 0 : Mathf.Sign(playerVelocity.x);

            playerRigidbody.isKinematic = true;
            playerRigidbody.Sleep();
            playerRigidbody.sharedMaterial.friction = 0.01f;
            playerRigidbody.isKinematic = false;
            playerRigidbody.WakeUp();

            Vector2 gravityDirection = new Vector2((playerController.groundCheck.position.x - playerRigidbody.transform.position.x), playerController.groundCheck.position.y - playerRigidbody.transform.position.y);
            playerRigidbody.AddForce(gravityDirection * 100);
            if (playerController.grounded)
            {
                playerRigidbody.AddForce(new Vector2((playerRigidbody.transform.right.x * playerController.moveForce * playerVelocityX * 100) * forceModifier, 0));
            }
            else
            {
                playerRigidbody.AddForce(new Vector2((playerRigidbody.transform.right.x * playerController.moveForce * playerVelocityX * 10) * forceModifier, 0));
            }
            playerRigidbody.velocity = Vector2.ClampMagnitude(playerRigidbody.velocity, playerController.maxSpeed);

            //yield return new WaitForSecondsRealtime(2);
            yield return new WaitForSeconds(2);

            playerController.slipCooldown = true;

            playerRigidbody.isKinematic = true;
            playerRigidbody.Sleep();
            playerRigidbody.sharedMaterial.friction = 0.4f;
            playerRigidbody.isKinematic = false;
            playerRigidbody.WakeUp();

            playerController.canMove = 1;
            yield return new WaitForSeconds(slipCooldownTime);
            playerController.slipCooldown = false;
        }
    }
}