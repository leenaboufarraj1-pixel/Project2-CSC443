using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IPoolable
{
    [SerializeField] private int startingHealth = 3;
    private int currentHealth;

    [SerializeField] private int pointValue = 100; 
    [SerializeField] private GameObject xpPrefab;

    public event Action<EnemyHealth> OnDied;

    private void Awake()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " took damage! Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDied?.Invoke(this);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.enemyDeathSound);
        }

        if (xpPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 1.2f;
            GameObject droppedXP = Instantiate(xpPrefab, spawnPos, Quaternion.identity);

            // Get the XPPoint script from the new object and set its value
            XPPoint xpScript = droppedXP.GetComponent<XPPoint>();
            if (xpScript != null)
            {
                xpScript.Init(pointValue); // Pass the value
            }
        }

        gameObject.SetActive(false);
    }

    public void OnGetFromPool()
    {
        currentHealth = startingHealth;
    }

    public void OnReturnFromPool()
    {
        OnDied = null;

        // Stop the AI so it doesn't keep trying to "pathfind" while invisible
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false;
        }
    }
}
