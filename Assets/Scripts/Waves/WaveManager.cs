using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private WaveGroupData waveGroupData;

    [Header("Flow")]
    [SerializeField] private bool autoStart = true;

    public event Action<int> OnWaveStarted;
    public event Action<int> OnWaveCompleted;
    public event Action OnAllWavesCompleted;
    public event Action<EnemyData> OnEnemyKilled;
    public event Action<EnemyData> OnEnemyReachedBase;

    public int CurrentWaveIndex => currentWaveIndex;
    public int TotalWaves => waveGroupData != null && waveGroupData.waves != null ? waveGroupData.waves.Length : 0;
    public int AliveEnemies => aliveEnemyIds.Count;
    public bool IsSpawningWave => isSpawningWave;

    private int currentWaveIndex = -1;
    private bool isSpawningWave;
    private bool runStarted;

    private readonly HashSet<int> aliveEnemyIds = new HashSet<int>();

    void Start()
    {
        if (autoStart)
        {
            StartWaves();
        }
    }

    public void StartWaves()
    {
        if (runStarted)
        {
            return;
        }

        if (enemySpawner == null)
        {
            Debug.LogWarning("WaveManager: EnemySpawner non assegnato.");
            return;
        }

        if (waveGroupData == null || waveGroupData.waves == null || waveGroupData.waves.Length == 0)
        {
            Debug.LogWarning("WaveManager: WaveGroupData mancante o vuoto.");
            return;
        }

        runStarted = true;
        StartCoroutine(RunWavesRoutine());
    }

    private IEnumerator RunWavesRoutine()
    {
        for (int i = 0; i < waveGroupData.waves.Length; i++)
        {
            WaveData waveData = waveGroupData.waves[i];
            if (waveData == null)
            {
                continue;
            }

            currentWaveIndex = i;
            OnWaveStarted?.Invoke(i);

            isSpawningWave = true;
            yield return StartCoroutine(enemySpawner.SpawnWave(waveData, TrackSpawnedEnemy));
            isSpawningWave = false;

            while (aliveEnemyIds.Count > 0)
            {
                yield return null;
            }

            OnWaveCompleted?.Invoke(i);

            bool isLastWave = i >= waveGroupData.waves.Length - 1;
            if (!isLastWave && waveGroupData.delayBetweenWaves > 0f)
            {
                yield return new WaitForSeconds(waveGroupData.delayBetweenWaves);
            }
        }

        OnAllWavesCompleted?.Invoke();
    }

    private void TrackSpawnedEnemy(GameObject enemyObject)
    {
        if (enemyObject == null)
        {
            return;
        }

        int id = enemyObject.GetInstanceID();
        if (!aliveEnemyIds.Add(id))
        {
            return;
        }

        EnemyController controller = enemyObject.GetComponent<EnemyController>();
        EnemyData enemyData = controller != null ? controller.data : null;

        HealthController health = enemyObject.GetComponent<HealthController>();
        if (health != null)
        {
            health.OnDied += () =>
            {
                ResolveEnemy(id);
                OnEnemyKilled?.Invoke(enemyData);
            };
        }

        EnemyMovement movement = enemyObject.GetComponent<EnemyMovement>();
        if (movement != null)
        {
            movement.OnReachedBase += HandleEnemyReachedBase;
        }
    }

    private void HandleEnemyReachedBase(EnemyMovement movement)
    {
        if (movement == null)
        {
            return;
        }

        movement.OnReachedBase -= HandleEnemyReachedBase;
        ResolveEnemy(movement.gameObject.GetInstanceID());
        OnEnemyReachedBase?.Invoke(movement.GetComponent<EnemyController>()?.data);
    }

    private void ResolveEnemy(int enemyId)
    {
        aliveEnemyIds.Remove(enemyId);
    }
}
