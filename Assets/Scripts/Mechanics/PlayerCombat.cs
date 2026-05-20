using Platformer.Gameplay;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 20;

    public float attackRate = 2f;
    private float nextAttackTime = 0f;

    void Update()
    {
        // Prevent button mashing based on attack rate
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetButtonDown("Fire1")) // Left Mouse Button or Ctrl
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void Attack()
    {
        // 1. Trigger the animation
        animator.SetTrigger("Attack");

        // 2. Detect enemies in range of the attack point
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // 3. Damage them
        foreach (Collider2D enemy in hitEnemies)
        {
            BaseBoss boss = enemy.GetComponent<BaseBoss>();
            if (boss != null)
            {
                boss.TakeDamage(attackDamage);
            }
        }
    }

    // Visualizes the attack radius inside the Unity Editor scene view
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}