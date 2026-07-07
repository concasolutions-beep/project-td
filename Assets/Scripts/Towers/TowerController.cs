using UnityEngine;

public class TowerController : MonoBehaviour
{
    public TowerData data;
    public Transform firePoint;

    private float fireCountdown = 0f;
    private float effectiveRange;
    private Transform currentTarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, effectiveRange);

        Transform closest = null;
        float closestRemainingDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Enemy"))
                continue;

            EnemyMovement movement = hit.GetComponent<EnemyMovement>();
            float remainingDistance = movement != null
                ? movement.GetRemainingDistance()
                : Vector2.Distance(transform.position, hit.transform.position);

            if (remainingDistance < closestRemainingDistance)
            {
                closestRemainingDistance = remainingDistance;
                closest = hit.transform;
            }
        }

        return closest;
    }

    void Shoot(Transform target)
    {
        Debug.Log($"[TowerController] {name} shooting at {target.name}");

        ObjectPool pool = PoolManager.Instance.GetPool(data.projectilePrefab, 10);

        GameObject projectileGO = pool.Get();
        projectileGO.transform.position = firePoint.position;

        ProjectileController projectile = projectileGO.GetComponent<ProjectileController>();
        projectile.Init(target, pool);

    }
}
