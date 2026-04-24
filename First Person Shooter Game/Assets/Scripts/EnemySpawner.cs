using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private WaveManager waveManager;

    // We use a Dictionary to manage multiple pools (one for each enemy type)
    private Dictionary<EnemyHealth, ObjectPool<EnemyHealth>> pools = new Dictionary<EnemyHealth, ObjectPool<EnemyHealth>>();

    public void SpawnEnemy(EnemyHealth prefab)
    {
        if (spawnPoints.Length == 0) return;

        // If we haven't created a pool for this enemy type yet, make one
        if (!pools.ContainsKey(prefab))
        {
            pools.Add(prefab, new ObjectPool<EnemyHealth>(prefab, transform, 5));
        }

        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        EnemyHealth enemy = pools[prefab].Get(point.position, point.rotation);

        enemy.OnDied += HandleEnemyDied;
    }

    private void HandleEnemyDied(EnemyHealth enemy)
    {
        enemy.OnDied -= HandleEnemyDied;

        // Find the correct pool to return the enemy to
        foreach (var pool in pools.Values)
        {
            // This is a simplified check; in a bigger game, you'd store the prefab ref on the enemy
            pool.Return(enemy);
        }

        if (waveManager != null) waveManager.OnEnemyDefeated();
    }
}