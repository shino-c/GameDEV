using UnityEngine;

public class ProctorBoss : BaseBoss
{
    [Header("Chasing Settings")]
    public float chaseRadius = 5f;

    private Rigidbody2D rb;
    private bool isFacingRight = false;

    protected override void Start()
    {
        // Call the base class Start method to initialize player and health variables
        base.Start();

        // Get the Rigidbody2D component attached to the boss for physics movement
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= chaseRadius)
        {
            ChasePlayerHorizontal();
        }
        else
        {
            StopMoving();
        }
    }

    void ChasePlayerHorizontal()
    {
        // Calculate the direction only on the X axis (-1 for Left, 1 for Right)
        float directionX = Mathf.Sign(player.position.x - transform.position.x);

        // Move the boss using physics velocity so it respects platforms and gravity
        rb.linearVelocity = new Vector2(directionX * moveSpeed, rb.linearVelocity.y);

        // Update the animation speed parameter if your Animator uses one
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        }

        // Flip the boss sprite to face the player
        FlipSprite(directionX);
    }

    void StopMoving()
    {
        // Bring the horizontal movement to a stop while keeping gravity active
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    void FlipSprite(float directionX)
    {
        // Flip the sprite scale if moving in the opposite direction of current orientation
        if ((directionX > 0 && !isFacingRight) || (directionX < 0 && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    // Optional: Draws the chase radius circle in the Unity Editor Scene view for easier tuning
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }
}