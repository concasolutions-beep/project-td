using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EnemySpawner : MonoBehaviour
{
    [Header("Riferimenti")]
    [Tooltip("Prefab fallback usato se EnemyData.prefab non e' assegnato")]
    public GameObject enemyPrefab;
    public Transform waypointsParent;

    [Header("Pooling")]
    [Tooltip("Margine aggiunto al massimo di nemici concorrenti per ogni prefab, quando si crea la pool")]
    [SerializeField] private int poolPadding = 5;

    // Pre-crea le pool con una size sufficiente a coprire l'ondata piu' numerosa per ogni prefab,
    // cosi' durante la run ObjectPool.Get() non deve mai istanziare a runtime.
    public void WarmUp(WaveGroupData waveGroupData)
    {
        if (waveGroupData == null || waveGroupData.waves == null)
        {
            return;
        }

        Dictionary<GameObject, int> maxConcurrentByPrefab = new Dictionary<GameObject, int>();

        foreach (WaveData wave in waveGroupData.waves)
        {
            if (wave == null || wave.enemies == null)
            {
                continue;
            }

            Dictionary<GameObject, int> countThisWave = new Dictionary<GameObject, int>();

            foreach (WaveData.EnemySpawnInfo info in wave.enemies)
            {
                if (info == null || info.enemy == null)
                {
                    continue;
                }

                GameObject prefab = info.enemy.prefab != null ? info.enemy.prefab : enemyPrefab;
                if (prefab == null)
                {
                    continue;
                }

                countThisWave.TryGetValue(prefab, out int current);
                countThisWave[prefab] = current + Mathf.Max(0, info.count);
            }

            foreach (KeyValuePair<GameObject, int> kvp in countThisWave)
            {
                maxConcurrentByPrefab.TryGetValue(kvp.Key, out int existingMax);
                maxConcurrentByPrefab[kvp.Key] = Mathf.Max(existingMax, kvp.Value);
            }
        }

        foreach (KeyValuePair<GameObject, int> kvp in maxConcurrentByPrefab)
        {
            PoolManager.Instance.GetPool(kvp.Key, kvp.Value + poolPadding);
        }
    }

    public IEnumerator SpawnWave(WaveData waveData, Action<GameObject> onEnemySpawned = null)
    {
        if (waveData == null)
        {
            Utils.WarningLog("EnemySpawner: WaveData mancante.");
            yield break;
        }

        if (waypointsParent == null || waypointsParent.childCount == 0)
        {
            Utils.WarningLog("EnemySpawner: waypointsParent non assegnato o vuoto.");
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
                Utils.WarningLog("EnemySpawner: entry wave senza EnemyData, skip.");
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
            Utils.WarningLog("EnemySpawner: prefab nemico mancante.");
            return;
        }

        ObjectPool pool = PoolManager.Instance.GetPool(prefabToSpawn, poolPadding);
        GameObject enemy = pool.Get();
        enemy.transform.SetPositionAndRotation(startPos, Quaternion.identity);

        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.SetData(enemyData);
        }

        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        if (movement != null)
        {
            movement.waypointsParent = waypointsParent;
            movement.ResetForSpawn();
        }

        onEnemySpawned?.Invoke(enemy);
    }
}
