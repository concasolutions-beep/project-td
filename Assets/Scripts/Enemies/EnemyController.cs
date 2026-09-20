using System;
using UnityEngine;

//Controller per centralizzare script nemici
[RequireComponent(typeof(HealthController))]
[RequireComponent(typeof(EnemyMovement))]
public class EnemyController : MonoBehaviour, IPoolable
{

    public EnemyData data;
    private HealthController health;
    private EnemyMovement movement;
    private GameObject sourcePrefab;
    private bool isDespawning;
    public event Action<EnemyController, EnemyExitReason> OnEnemyDespawned;

    public void SetSourcePrefab(GameObject prefab)
    {
        sourcePrefab = prefab;
    }

    void Awake()
    {
        health = GetComponent<HealthController>();
        movement = GetComponent<EnemyMovement>();
        movement.OnReachedBase += HandleReachedBase;
    }

    void Start()
    {
        Init();
    }

    public void Init()
    {
        if (data == null)
        {
            return;
        }


        health.SetMaxHealth(data.maxHealth);
        movement.SetSpeed(data.speed);
    }

    public void SetData(EnemyData enemyData)
    {
        data = enemyData;
        Init();
    }

    public void OnSpawn()
    {
        isDespawning = false;
        health.ResetHealth();
        health.OnDied += HandleDied;
    }

    public void OnDespawn()
    {
        health.ClearSubscribers();
        OnEnemyDespawned = null;
    }

    private void HandleDied()
    {
        Despawn(EnemyExitReason.Killed);
    }

    private void HandleReachedBase(EnemyMovement _)
    {
        Despawn(EnemyExitReason.Escaped);
    }

    private void Despawn(EnemyExitReason reason)
    {
        if (isDespawning)
        {
            return;
        }

        isDespawning = true;
        OnEnemyDespawned?.Invoke(this, reason);

        if (sourcePrefab != null)
        {
            PoolManager.Instance.GetPool(sourcePrefab).Release(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

public enum EnemyExitReason
{
    Killed,
    Escaped
}
