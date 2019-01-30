using UnityEngine;
using UnityEngine.UI;

//Allows the player to control their character (refactored from BeardBoy)
public class CustomPlayerController : MonoBehaviour
{
    [Tooltip("The maximum velocity of the player")] public float maxSpeed;
    [Tooltip("The player's acceleration speed")] public float moveForce;
    [Tooltip("The player's animation speed")] public float animationSpeed;
    [Tooltip("The strength of the player's jump")] public float jumpForce;
    [SerializeField] [Tooltip("An object placed at the center of the player's feet-circle collider")] public Transform groundCheck;     //Public so it can be accessed from within leakScript.
    [SerializeField] [Tooltip("All the tags that are to be used when checking if the player is grounded")] private LayerMask whatIsGround;

    public static Animator animator;

    private float groundRadius = 1f;                     //Used for the radius of an overlapCircle call in Update that checks if the player is grounded
    [HideInInspector] public float moving;                  //The value of the user's horizontal-movement input - Public so it can be accessed by the leak script.

    private Rigidbody2D rigidBody;                          //Used for altering the player character
    private SpriteRenderer spriteRenderer;                  //Used for rendering the player character
    private PolygonCollider2D polygonCollider;              //Used for altering the player collider

    private Vector2 gravityDirection;                       //Placeholder variable used in calculation
    [HideInInspector] public bool grounded = false;         //Status variable for checking if the player is touching the ground or not - Public so that it can be accessed by the leak script.

    [HideInInspector] public int canMove;                   //Used for disabling and enabling player movement on collision with a droplet
    [HideInInspector] public bool slipCooldown;             //Used to manage a cooldown before the movement of the player can be re-disabled on collision with a droplet

    private string deviceCategory;                          //Stores the type of device the user is using to play the game

    void OnValidate()
    {
        if (maxSpeed < 0)
        {
            maxSpeed *= -1;
        }
        if (moveForce < 0)
        {
            moveForce *= -1;
        }
        if (animationSpeed < 0)
        {
            animationSpeed *= -1;
        }
        if (jumpForce < 0)
        {
            jumpForce *= -1;
        }
    }

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.isKinematic = true;
        rigidBody.Sleep();
        rigidBody.sharedMaterial.friction = 0.4f;
        rigidBody.isKinematic = false;
        rigidBody.WakeUp();

        animator = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        moving = 0.0f;

        polygonCollider = GetComponent<PolygonCollider2D>();

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Water"), LayerMask.NameToLayer("Player"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Pump"), LayerMask.NameToLayer("Player"), true);

        canMove = 1;
        slipCooldown = false;

        deviceCategory = SystemInfo.deviceType.ToString();
    }

    public void Move(GameObject slider)
    {
        moving = slider.GetComponent<Slider>().value;
    }

    public void MoveReset(GameObject slider)
    {
        slider.GetComponent<Slider>().value = 0;
        moving = 0;
    }

    public void Jump()
    {
        if (grounded)
        {
            rigidBody.AddForce(new Vector2(0, jumpForce * transform.up.y * canMove));
        }
    }

    void Update()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.transform.position, groundRadius, whatIsGround);       //isTouchingLayers is only called against the last physics system update which could cause frame delay errors with our future plans to add objects colliding with the player
        animator.SetBool("grounded", grounded);

        float vspeed = Vector2.Dot(rigidBody.velocity, (groundCheck.position - transform.position));
        animator.SetFloat("vSpeed", vspeed);

        if (deviceCategory == "Handheld")
        {
            if (moving != 0)
            {
                animator.SetBool("moving", true);
            }
            else if (moving == 0)
            {
                animator.SetBool("moving", false);
            }
            if (moving < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (moving > 0)
            {
                spriteRenderer.flipX = false;
            }
        }
        else
        {
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
            }

            //animator.speed = animationSpeed;

            if (Input.GetAxis("Horizontal") < 0)
            {
                if (canMove == 1)
                {
                    spriteRenderer.flipX = true;
                }
                /*for (int i = 0; i < polygonCollider.points.Length; i++)
                {
                    polygonCollider.points[i] = new Vector2(polygonCollider.points[i].x * -1, polygonCollider.points[i].y * -1);
                }*/
                Vector2[] points;
                for (int i = 0; i < polygonCollider.pathCount; i++)
                {
                    /*foreach (Vector2 point in polygonCollider.GetPath(i))
                    {
                        polygonCollider.SetPath(i, polygonCollider.GetPath(i).ForEach(point = point * -1));
                    }
                    polygonCollider.SetPath(i);*/
                    //polygonCollider.SetPath(i, polygonCollider.GetPath(i).ForEach(Vector2 *= -1));

                    points = polygonCollider.GetPath(i);
                    /*foreach (Vector2 point in points)
                    {
                        point = new Vector2(point.x * -1, point.y * -1);
                    }*/
                    for (int j = 0; j < points.Length; j++)
                    {
                        points[j] = new Vector2(points[j].x * -1, points[j].y * -1);
                    }
                    polygonCollider.SetPath(i, points);
                }
                animator.SetBool("moving", true);
            }
            else if (Input.GetAxis("Horizontal") > 0)
            {
                if (canMove == 1)
                {
                    spriteRenderer.flipX = false;
                }
                Vector2[] points;
                for (int i = 0; i < polygonCollider.pathCount; i++)
                {
                    points = polygonCollider.GetPath(i);
                    /*foreach (Vector2 point in points)
                    {
                        point = new Vector2(point.x * -1, point.y * -1);
                    }*/
                    for (int j = 0; j < points.Length; j++)
                    {
                        points[j] = new Vector2(points[j].x * -1, points[j].y * -1);
                    }
                    polygonCollider.SetPath(i, points);
                }
                animator.SetBool("moving", true);
            }
            else
            {
                animator.SetBool("moving", false);
            }
        }
    }

    void FixedUpdate()
    {
        /*
        Get a user input for horizontal movement.
        Obtain and add force in the direction of the player's feet (gravity).
        Add horizontal movement in the player's current horizontal direction only if grounded.
        Limit the player's velocity only if they are holding down the key and grounded. (This ensures falling velocity is not affected).
        */

        if (deviceCategory != "Handheld")
        {
            moving = Input.GetAxis("Horizontal");
        }

        if (canMove == 1)
        {
            rigidBody.velocity = new Vector2(transform.right.x * moveForce * moving * canMove, rigidBody.velocity.y);
        }
    }
}