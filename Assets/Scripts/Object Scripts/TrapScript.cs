using UnityEngine;

public class TrapScript : MonoBehaviour
{
    public float bounceForce = 7.5f;
    public int damage = 1;

    private Vector2 bounceDirection;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            HandlePlayerBounce(collision.gameObject);

        }
    }

    private void Start()
    {
        bounceDirection = transform.up; //init trap's facing local up direction
    }

    private void HandlePlayerBounce(GameObject player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb)
        {
            // Remove the player's existing velocity
            // in the direction the trap will bounce them.
            float velocityInBounceDirection = Vector2.Dot(rb.linearVelocity, bounceDirection);

            rb.linearVelocity -= bounceDirection * velocityInBounceDirection;

            // Apply bounce force in the trap's facing direction
            rb.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);
            Debug.Log("Bounce direction: " + bounceDirection);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(
            transform.position,
            transform.up * 1f
        );
    }
}
