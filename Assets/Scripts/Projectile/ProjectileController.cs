using UnityEngine;
using ProjectTD.Health;
using ProjectTD.Pooling;

namespace ProjectTD.Projectile
{
    public class ProjectileController : PoolableEntity
    {
        public ProjectileData data;
        private Transform target;
        private float lifetime;
        private float damage;

        public void Init(Transform targetTransform, float damageAmount)
        {
            target = targetTransform;
            damage = damageAmount;
            lifetime = data.maxLifetime;
        }

        protected override void OnSpawned()
        {
        }

        protected override void OnDespawned()
        {
            target = null;
        }

        void Update()
        {
            if (target == null || !target.gameObject.activeInHierarchy)
            {
                ReleaseToPool();
                return;
            }

            lifetime -= Time.deltaTime;
            if (lifetime <= 0f)
            {
                ReleaseToPool();
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
                    health.TakeDamage(damage);
                }

                ReleaseToPool();
            }
        }
    }
}
