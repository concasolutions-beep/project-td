using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public TowerData data;

    private Transform target;
    private float lifetime;
    private ObjectPool pool;

    public void Init(Transform targetTransform, ObjectPool poolRef)
    {
        target = targetTransform;
        pool = poolRef;
        lifetime = data.maxLifetime;
    }

    void Update()
    {
        if (target == null)
        {
            Release();
            return;
        }

        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            Release();
            return;
        }

        Move();
    }

    void Move()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        transform.position += (Vector3)(direction * data.speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            HealthController health = other.GetComponent<HealthController>();
            if (health != null)
            {
                health.TakeDamage(data.damage);
            }

            Release();
        }
    }

    void Release()
    {
        pool.Release(gameObject);
    }
}
