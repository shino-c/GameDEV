using UnityEngine;
using System.Collections;

public class BugKingBoss : BaseBoss
{
    [Header("Glitch & Dash Settings")]
    public float ActionCooldown = 4f;       // Time between glitch actions
    public float GlitchTeleportDistance = 4f; // How close he teleports to the player
    public float DashSpeed = 15f;           // Speed of the physical dash attack
    public float ChargeDuration = 0.5f;     // "Glitch freeze" warning time before dashing

    private float _actionTimer;
    private bool _isAttacking = false;

    protected override void Start()
    {
        // Inherits baseline setup (finding player, setting health) from BaseBoss
        base.Start();
        _actionTimer = ActionCooldown;
    }

    void Update()
    {
        // If the boss is currently executing a glitch-dash sequence, suspend normal tracking
        if (_isAttacking) return;

        // Keep a countdown timer between glitch attacks
        _actionTimer -= Time.deltaTime;

        if (_actionTimer <= 0f)
        {
            StartCoroutine(GlitchDashSequence());
            _actionTimer = ActionCooldown; // Reset the cycle timer
        }
        else
        {
            // Standard state: Hover or float menacingly towards the player's general area
            HoverTowardsPlayer();
        }
    }

    void HoverTowardsPlayer()
    {
        if (player == null) return;

        // Slow movement inherited from BaseBoss.moveSpeed
        Vector2 targetPos = new Vector2(player.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    IEnumerator GlitchDashSequence()
    {
        _isAttacking = true;

        if (player == null) yield break;

        // --- STEP 1: THE GLITCH TELEPORT ---
        // Trigger a glitch sprite effect if your Animator supports it
        if (animator != null) animator.SetTrigger("GlitchOut");

        // Calculate a position slightly left or right of the player
        float directionSign = player.position.x > transform.position.x ? 1f : -1f;
        Vector2 teleportTarget = new Vector2(player.position.x - (directionSign * GlitchTeleportDistance), transform.position.y);

        // Instantly shift position (Teleport)
        transform.position = teleportTarget;

        // --- STEP 2: THE GLITCH CHARGE (Warning Phase) ---
        if (animator != null) animator.SetTrigger("GlitchIn");

        // Lock onto the direction of the target right before launching
        Vector2 dashDirection = (player.position - transform.position).normalized;
        // Flatten the Y axis so he dashes horizontally across platforms
        dashDirection.y = 0;

        // Freeze in place momentarily to simulate an error/charge-up frame
        yield return new WaitForSeconds(ChargeDuration);

        // --- STEP 3: THE HIGH-SPEED DASH ---
        if (animator != null) animator.SetTrigger("DashStrike");

        float dashTime = 0.3f; // Duration of active physical slide movement
        while (dashTime > 0)
        {
            // Move via a direct transform translation across delta space
            transform.Translate(dashDirection * DashSpeed * Time.deltaTime);
            dashTime -= Time.deltaTime;
            yield return null;
        }

        // Sequence complete, yield control back to standard state evaluation loops
        _isAttacking = false;
    }

    // Handles dealing damage back to the player if they crash together during the sprint
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isAttacking && collision.CompareTag("Player"))
        {
            // Replace with your actual Player health execution line if available
            Debug.Log("Player was struck by BugKing's Glitch Dash!");
        }
    }
}