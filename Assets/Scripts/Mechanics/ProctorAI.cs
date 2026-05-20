using UnityEngine;

public class ProctorAI : MonoBehaviour
{
    public enum EnemyState { Patrolling, Chasing }
    public EnemyState currentState = EnemyState.Patrolling;

    [Header("Movement & Target")]
    public Transform player;
    public Transform[] patrolPoints;
    public float speed = 2f;
    public float chaseSpeed = 4f;
    public float targetDetectionRadius = 5f;

    private int currentPatrolIndex = 0;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Tell the animator if we are moving
        if (animator != null)
        {
            animator.SetBool("IsChasing", currentState == EnemyState.Chasing);
        }

        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
                if (distanceToPlayer <= targetDetectionRadius)
                {
                    currentState = EnemyState.Chasing;
                }
                break;

            case EnemyState.Chasing:
                ChasePlayer();
                if (distanceToPlayer > targetDetectionRadius * 1.5f)
                {
                    currentState = EnemyState.Patrolling;
                }
                break;
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    void ChasePlayer()
    {
        Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, chaseSpeed * Time.deltaTime);
    }
}