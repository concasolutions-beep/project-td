using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public ProjectileData data;

    private Transform target;
    private float lifetime;

    public void Init(Transform targetTransform)
    {
        target = targetTransform;
        lifetime = data.maxLifetime;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        MoveTowardsTarget();
    }

    void MoveTowardsTarget()
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

            Destroy(gameObject);
        }
    }
}
