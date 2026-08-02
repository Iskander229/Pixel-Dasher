using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    public Rigidbody2D rb;
    [Header("Movement")]
    public float moveSpeed = 5f;
    float horizontalMovement;

    [Header("Jumping")]
    public float jumpPower = 8f;
    public int maxJumps = 2;
    private int jumpsRemaining;

    [Header("GroundCheck")]
    public Transform groundCheckPos; //position on player where groundcheck starts (edited on player prefab)
    public Vector2 groundcheckSize = new Vector2(0.5f, 0.05f); //the size of debug cube to see it lol
    public LayerMask groundLayer; //to check if touching anything tagged as "ground"

    //to see our groundcheck location
    private void OnDrawGizmosSelected() //when selecting script owner, this method draws a debug
    {
        Gizmos.color = Color.red; //cube will be red
        Gizmos.DrawWireCube(groundCheckPos.position, groundcheckSize);
    }

    //to check if grounded
    private void CheckGrounded()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundcheckSize, 0, groundLayer))
        {
            jumpsRemaining = maxJumps; //when grounded update remaining jumps to max.
        }
    }


    void Start()
    {

    }

    void Update()
    {
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x; //movement on horizontal(x) axis
    }

    public void Jump(InputAction.CallbackContext context)
    {
        CheckGrounded (); //so before jumping check if we grounded (if yes remaining jumps are maxed)
        if (jumpsRemaining > 0)
        {
            if (context.performed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpsRemaining--; //count our jumps 
            }
            //else if(context.canceled) {rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower/2);} - In case we want to jump half power when pressed once and fully if holding the button.

        }

    }
}
