using UnityEngine;
using UnityEngine.UI; // Required to control UI elements like Sliders

public class BaseBoss : MonoBehaviour
{
    [Header("Shared Boss Stats")]
    public string bossName;
    public int maxHealth = 100;
    public float moveSpeed = 2f;
    protected int currentHealth;

    [Header("UI Connection (For Member 6)")]
    public Slider bossHealthSlider; // Member 6 will drag their slider UI here

    [Header("Shared Components")]
    public Transform player;
    protected Animator animator;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        // Initialize the UI slider values
        if (bossHealthSlider != null)
        {
            bossHealthSlider.maxValue = maxHealth;
            bossHealthSlider.value = currentHealth;
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Update the visual UI health bar smoothly
        if (bossHealthSlider != null)
        {
            bossHealthSlider.value = currentHealth;
        }

        if (animator != null) animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (animator != null) animator.SetBool("IsDead", true);

        // Hide the boss health bar UI upon death
        if (bossHealthSlider != null)
        {
            bossHealthSlider.gameObject.SetActive(false);
        }

        if (GetComponent<Collider2D>() != null) GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        Destroy(gameObject, 1.5f);
    }
}