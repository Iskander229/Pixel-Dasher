using UnityEngine;

public class FloatEnemy : EnemyController
{
    public float floatSpeed = 2f;
    public float floatHeight = 0.5f;
    private Vector3 startPosition;
    private Rigidbody2D rb;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic; // no gravity/forces, but proper physics sync
    }

    void Start()
    {
        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        rb.MovePosition(new Vector2(transform.position.x, newY));
    }
}