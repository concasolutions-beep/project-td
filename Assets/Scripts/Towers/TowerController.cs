using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    public TowerData data;
    public Transform firePoint;
    public LayerMask enemyLayerMask;

    private float fireCountdown = 0f;
    private float effectiveRange;
    private Transform currentTarget;
    private ContactFilter2D contactFilter;
    private List<Collider2D> targetsBuffer = new List<Collider2D>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(enemyLayerMask);
        // Set the collider radius based on the tower's range (used for range visualization/gizmos)
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col != null)
        {
            col.radius = data.range;
        }

        effectiveRange = data.range * Mathf.Abs(transform.lossyScale.x);

        Debug.Log($"[TowerController] {name} started. Range: {data.range}, FireRate: {data.fireRate}");
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsTargetValid(currentTarget))
        {
            currentTarget = FindClosestEnemy();
        }

        if (currentTarget == null)
            return;

        if (fireCountdown <= 0f)
        {
            Shoot(currentTarget);
            fireCountdown = 1f / data.fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    bool IsTargetValid(Transform target)
    {
        if (target == null)
            return false;

        float distance = Vector2.Distance(transform.position, target.position);
        return distance <= effectiveRange;
    }

    Transform FindClosestEnemy()
    {
        targetsBuffer.Clear();
        Physics2D.OverlapCircle(transform.position, effectiveRange, contactFilter, targetsBuffer);

        Transform closest = null;
        float closestRemainingDistance = float.MaxValue;

        foreach (Collider2D target in targetsBuffer)
        {
            EnemyMovement movement = target.GetComponent<EnemyMovement>();
            float remainingDistance = movement != null
                ? movement.GetRemainingDistance()
                : Vector2.Distance(transform.position, target.transform.position);

            if (remainingDistance < closestRemainingDistance)
            {
                closestRemainingDistance = remainingDistance;
                closest = target.transform;
            }
        }

        return closest;
    }

    void Shoot(Transform target)
    {
        Debug.Log($"[TowerController] {name} shooting at {target.name}");

        ObjectPool pool = PoolManager.Instance.GetPool(data.projectileData.projectilePrefab, 10);

        GameObject projectileGO = pool.Get();
        projectileGO.transform.position = firePoint.position;

        ProjectileController projectile = projectileGO.GetComponent<ProjectileController>();
        projectile.Init(target, pool);

    }
}
