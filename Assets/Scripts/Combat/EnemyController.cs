using System;
using UnityEngine;

[RequireComponent(typeof(HealthController))]
[RequireComponent(typeof(EnemyMovement))]
public class EnemyController : PoolableEntity
{
    public EnemyData data;
    private HealthController health;
    private EnemyMovement movement;
    public event Action<EnemyController, EnemyExitReason> OnEnemyDespawned;

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

    protected override void OnSpawned()
    {
        health.ResetHealth();
        health.OnDied += HandleDied;
    }

    protected override void OnDespawned()
    {
        health.OnDied -= HandleDied;
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
        if (IsReleased)
        {
            return;
        }

        OnEnemyDespawned?.Invoke(this, reason);
        ReleaseToPool();
    }
}

public enum EnemyExitReason
{
    Killed,
    Escaped
}
