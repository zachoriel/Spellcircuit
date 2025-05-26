using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyWaveSpawner : Service
{
    [System.Serializable]
    public struct Wave
    {
        public int enemyCount;
        public float spawnDelay;

        public Wave(int _enemyCount, int _spawnDelay)
        {
            enemyCount = _enemyCount;
            spawnDelay = _spawnDelay;
        }
    }

    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private List<Wave> waves;
    [SerializeField] private float waveCountdown = 10f;

    private int currentWaveIndex = 0;
    private bool preparingWave = false;
    private bool waveInProgress = false;
    private Coroutine waveSpawner;

    private List<Enemy> currentLivingEnemies = new List<Enemy>();
    public List<Enemy> GetCurrentEnemies() { return currentLivingEnemies; }

    private bool debugPauseWaveSpawning = false;


    private void Awake()
    {
        ServiceLocator.Instance.AddService<EnemyWaveSpawner>(this);
    }

    private void Update()
    {
        if (debugPauseWaveSpawning && waveSpawner != null)
        {
            StopCoroutine(waveSpawner);
            waveSpawner = null;
        }

        if (!waveInProgress && !preparingWave && currentWaveIndex < waves.Count)
        {
            waveSpawner = StartCoroutine(SpawnWave(waves[currentWaveIndex]));
        }

        if (currentLivingEnemies.Count == 0 && !preparingWave)
        {
            waveInProgress = false;
        }
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        preparingWave = true;

        Debug.Log($"Wave {currentWaveIndex + 1} starting in:");
        for (int i = 0; i < waveCountdown; i++)
        {
            Debug.Log($"{waveCountdown - i}...");
            yield return new WaitForSeconds(1);
        }

        Debug.Log($"Spawning Wave {currentWaveIndex + 1}");

        waveInProgress = true;
        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(wave.spawnDelay);
        }

        currentWaveIndex++;
        preparingWave = false;
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No valid enemies or spawn points!");
            return;
        }

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject newEnemy = Instantiate(prefab, point.position, point.rotation);
        Enemy enemy = newEnemy.GetComponent<Enemy>();
        if (enemy != null)
        {
            currentLivingEnemies.Add(enemy);
            ImportantObjectHolder objectHolder = ServiceLocator.Instance.GetService<ImportantObjectHolder>();
            enemy.Init(objectHolder.player.transform);
        }
        else
        {
            Debug.LogError($"ERROR: Enemy script not detected on: {newEnemy.gameObject.name}. Stopping game.");

            debugPauseWaveSpawning = true;
            preparingWave = false;
            waveInProgress = false;

            foreach (Enemy livingEnemy in currentLivingEnemies)
            {
                Destroy(livingEnemy.gameObject);
            }
            currentLivingEnemies.Clear();

            newEnemy.name = "ENEMY WITHOUT SCRIPT";
        }
    }
}