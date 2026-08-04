
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    public Rigidbody2D rb;
    bool isFacingRight = true; // true - player faces right, false - left.
    public Animator animator; // for animator component
    public ParticleSystem TrailParticles; //trail when player jumps

    [Header("Movement")]
    public float moveSpeed = 5f;
    float horizontalMovement;

    [Header("Jumping")]
    public float jumpPower = 8f;
    public int maxJumps = 2;
    private int jumpsLeft;

    [Header("Gravity")]
    public float baseGravity = 2;
    public float maxFallSpeed = 15f;
    public float fallSpeedMultiplier = 1.5f;

    [Header("WallMovement")]
    public float wallSlideSpeed = 2;
    float currentWallDir = 0; // 0-none, 1-right, -1=left
    float lastWallDir = 0;

    [Header("GroundCheck")]
    public Transform groundCheckPos; //position on player where groundcheck starts (edited on player prefab)
    public Vector2 groundCheckSize = new Vector2(0.1f, 0.07f); //the size of debug cube to see it lol
    public LayerMask groundLayer; //to check if touching anything tagged as "ground"
    bool isGrounded;


    [Header("WallCheck")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.1f, 0.5f);
    public LayerMask wallLayer;

    private void OnDrawGizmosSelected() //when selecting script owner, this method draws a debug
    {
        Gizmos.color = Color.red; //cube will be red
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.color = Color.yellow; 
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }


    void Start()
    {
        FlipPlayer(); //when starting player always faces right (if default of "isFacingRight" is true
    }

    void Update()
    {
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
        DoGravity();
        DoWallSlide();
        //Debug.Log($"Current: {currentWallDir}, Last: {lastWallDir}");
    }

    public void Move(InputAction.CallbackContext context)
    {
        FlipPlayer(); //when applying movement (left or right) make player face that direction...
        horizontalMovement = context.ReadValue<Vector2>().x; //movement on horizontal(x) axis
    }

    private void FlipPlayer() //make player face direction of movement
    {
        if (isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f; //make character face the other way (in transform, scale x becomes -1 or 1)
            transform.localScale = ls;
        }

        if(rb.linearVelocity.y == 0)
        {
            JumpTrail(); //play trail particles when changing directions but not when falling
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.performed) return; //lets method run only when pressed first, otherwise would repeat on exit.

        if (CheckGrounded())
        {
            jumpsLeft = maxJumps; //fill up double jump when on ground
        }

        if (jumpsLeft != 0 && CheckWalled() && currentWallDir != 0 && currentWallDir != lastWallDir)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            lastWallDir = currentWallDir;
            JumpTrail();
        }

        else if (jumpsLeft > 0 && !CheckWalled()) //double jump
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            jumpsLeft--;
            lastWallDir = currentWallDir;
            JumpTrail();
        }
    }

    private void JumpTrail()
    {
        //animator.SetTrigger("jump"); for later when  we have jump animation and etc

        TrailParticles.Play();
    }

    private bool CheckGrounded() 
    {
        return Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);

    }

    private bool CheckWalled()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
        
    }

    private void DoGravity() //custom gravity for better jumping experience
    {
        if (rb.linearVelocity.y < 0) //check if Y velocity is negative (means we are falling)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier; //makes player fall faster to prevent floaty feel
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed)); //cap negative.y velocity (fall speed) to prevent smashing the floor
        }
        else
        {
            rb.gravityScale = baseGravity; // if not falling return to baseGravity
        }
    }

    private void DoWallSlide()
    {
        if (!CheckGrounded() && CheckWalled())
        {
            currentWallDir = transform.localScale.x; //while on the wall, get if it's on left or right from player
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }
        else
        {
            currentWallDir = 0; //on ground value should be 0 otherwise first wall jump may not work if value was 1,-1
        }
    }

}
