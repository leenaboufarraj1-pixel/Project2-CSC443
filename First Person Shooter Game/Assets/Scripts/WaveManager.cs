using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemyGroup
    {
        public EnemyHealth enemyPrefab;
        public int count;
    }

    [System.Serializable]
    public class WaveSettings
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups; // Define types and amounts here
        public float spawnRate = 2f;

        // Helper to get total count for this wave
        public int GetTotalEnemyCount()
        {
            int total = 0;
            foreach (var group in enemyGroups) total += group.count;
            return total;
        }
    }

    [Header("Wave Configuration")]
    [SerializeField] private List<WaveSettings> waves;
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private float intermissionTime = 10f;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI timerText;

    private int currentWaveIndex = 0;
    private int activeEnemies;

    private void Start()
    {
        StartCoroutine(PlayGame());
    }

    private IEnumerator PlayGame()
    {
        while (currentWaveIndex < waves.Count)
        {
            yield return StartCoroutine(HandleIntermission());
            yield return StartCoroutine(HandleWave());
            currentWaveIndex++;
        }

        // 1. Update the UI text
        waveText.text = "Victory!";

        // 2. Call the Victory Panel from your MenuManager
        // We use FindObjectOfType to locate the script in your scene
        MenuManager menuManager = FindObjectOfType<MenuManager>();
        if (menuManager != null)
        {
            menuManager.ShowVictory();
        }
        else
        {
            Debug.LogWarning("MenuManager not found in the scene! Cannot show Victory Panel.");
        }
    }

    private IEnumerator HandleIntermission()
    {
        float timer = intermissionTime;
        waveText.text = "Intermission";
        while (timer > 0)
        {
            timerText.text = $"Next Wave in: {Mathf.CeilToInt(timer)}s";
            yield return new WaitForSeconds(1f);
            timer--;
        }
        timerText.text = "";
    }

    private IEnumerator HandleWave()
    {
        WaveSettings currentWave = waves[currentWaveIndex];
        waveText.text = $"Wave: {currentWave.waveName}";

        activeEnemies = 0;
        int totalToSpawn = currentWave.GetTotalEnemyCount();
        int spawnedSoFar = 0;

        // Loop through each group defined in the Inspector
        foreach (var group in currentWave.enemyGroups)
        {
            for (int i = 0; i < group.count; i++)
            {
                spawner.SpawnEnemy(group.enemyPrefab);
                spawnedSoFar++;
                activeEnemies++;

                // Wait based on the spawn rate before sending the next one
                yield return new WaitForSeconds(1f / currentWave.spawnRate);
            }
        }

        // Wait until all enemies from all groups are dead
        while (activeEnemies > 0)
        {
            yield return null;
        }
    }

    public void OnEnemyDefeated()
    {
        activeEnemies--;
    }
}