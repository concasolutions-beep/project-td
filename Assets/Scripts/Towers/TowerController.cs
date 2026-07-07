using UnityEngine;

public class TowerController : MonoBehaviour
{
    public TowerData data;
    public Transform firePoint;

    private float fireCountdown = 0f;
    private float effectiveRange;

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
        Transform target = FindClosestEnemy();
        if (target == null)
            return;

        if (fireCountdown <= 0f)
        {
            Shoot(target);
            fireCountdown = 1f / data.fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    Transform FindClosestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, effectiveRange);

        Transform closest = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Enemy"))
                continue;

            float distance = Vector2.Distance(transform.position, hit.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
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
