using UnityEngine;
using System.Collections;
using System;

public class EnemySpawner : MonoBehaviour
{
    [Header("Riferimenti")]
    [Tooltip("Prefab fallback usato se EnemyData.prefab non e' assegnato")]
    public GameObject enemyPrefab;
    public Transform waypointsParent;

    public IEnumerator SpawnWave(WaveData waveData, Action<GameObject> onEnemySpawned = null)
    {
        if (waveData == null)
        {
            Debug.LogWarning("EnemySpawner: WaveData mancante.");
            yield break;
        }

        if (waypointsParent == null || waypointsParent.childCount == 0)
        {
            Debug.LogWarning("EnemySpawner: waypointsParent non assegnato o vuoto.");
            yield break;
        }

        if (waveData.enemies == null || waveData.enemies.Length == 0)
        {
            yield break;
        }

        for (int i = 0; i < waveData.enemies.Length; i++)
        {
            WaveData.EnemySpawnInfo spawnInfo = waveData.enemies[i];
            if (spawnInfo == null || spawnInfo.enemy == null)
            {
                Debug.LogWarning("EnemySpawner: entry wave senza EnemyData, skip.");
                continue;
            }

            int count = Mathf.Max(0, spawnInfo.count);
            float delay = Mathf.Max(0f, spawnInfo.spawnDelay);

            for (int c = 0; c < count; c++)
            {
                SpawnEnemy(spawnInfo.enemy, onEnemySpawned);
                if (delay > 0f)
                {
                    yield return new WaitForSeconds(delay);
                }
            }
        }
    }

    private void SpawnEnemy(EnemyData enemyData, Action<GameObject> onEnemySpawned)
    {
        Vector3 startPos = waypointsParent.GetChild(0).position;
        GameObject prefabToSpawn = enemyData.prefab != null ? enemyData.prefab : enemyPrefab;

        if (prefabToSpawn == null)
        {
            Debug.LogWarning("EnemySpawner: prefab nemico mancante.");
            return;
        }

        GameObject enemy = Instantiate(prefabToSpawn, startPos, Quaternion.identity);

        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.SetData(enemyData);
        }

        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        if (movement != null)
        {
            movement.waypointsParent = waypointsParent;
        }

        onEnemySpawned?.Invoke(enemy);
    }
}