using UnityEngine;
using UnityEngine.AI;
using StarterAssets;

public class AnimalAI : MonoBehaviour
{
    NavMeshAgent agent;
    FirstPersonController player;
    Animator animator; // To control the wolf's legs

    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackRange = 1.8f; // Animals might need a slightly larger range
    [SerializeField] private float attackRate = 1.5f;
    private float nextAttackTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        player = FindAnyObjectByType<FirstPersonController>();
    }

    private void OnEnable()
    {
        // Safety check to snap the wolf to the blue NavMesh floor
        if (agent != null)
        {
            agent.enabled = false;
            // Force the wolf to the height of the terrain
            agent.enabled = true;

            // This tries to find the nearest blue surface within 2 meters
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }
    }

    private void Update()
    {
        if (player == null) return;

        if (agent.enabled && agent.isOnNavMesh)
        {
            // 1. Move the wolf toward the player
            agent.SetDestination(player.transform.position);

            // 2. Drive the Animations
            if (animator != null)
            {
                // Calculate how fast the NavMeshAgent is currently moving
                float speed = agent.velocity.magnitude;

                if (speed > 0.1f)
                {
                    // These values "unlock" the Run animation in your Blend Tree
                    animator.SetFloat("Vert", 1f);
                    animator.SetFloat("State", 1f);
                }
                else
                {
                    // Return to Idle
                    animator.SetFloat("Vert", 0f);
                    animator.SetFloat("State", 0f);
                }
            }

            // 3. Attack Logic
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= attackRange && Time.time >= nextAttackTime)
            {
                AttackPlayer();
            }
        }
    }

    void AttackPlayer()
    {
        nextAttackTime = Time.time + attackRate;

        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(attackDamage);

            Debug.Log("The animal attacked the player!");
        }
    }
}