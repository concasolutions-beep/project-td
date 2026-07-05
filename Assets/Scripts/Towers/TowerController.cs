using UnityEngine;
using System.Collections.Generic;

public class TowerController : MonoBehaviour
{
    public TowerData data;
    public Transform firePoint;

    private float fireCountdown = 0f;
    private List<Transform> enemiesInRange = new List<Transform>();



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set the collider radius based on the tower's range
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col != null)
        {
            col.radius = data.range;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enemiesInRange.Count == 0)
            return;

        Transform target = enemiesInRange[0];

        if (fireCountdown <= 0f)
        {
            Shoot(target);
            fireCountdown = 1f / data.fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    void Shoot(Transform target)
    {
        ObjectPool pool = PoolManager.Instance.GetPool(data.projectilePrefab,10);

        GameObject projectileGO = pool.Get();
        projectileGO.transform.position = firePoint.position;

        ProjectileController projectile = projectileGO.GetComponent<ProjectileController>();
        projectile.Init(target,pool);

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
            enemiesInRange.Add(other.transform);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
            enemiesInRange.Remove(other.transform);
    }
}
