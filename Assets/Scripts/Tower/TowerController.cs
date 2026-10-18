using System.Collections.Generic;
using UnityEngine;
using ProjectTD.Enemy;
using ProjectTD.Pooling;
using ProjectTD.Projectile;

namespace ProjectTD.Tower
{
    public class TowerController : PoolableEntity
    {
        [SerializeField] private TowerData data;
        public Transform firePoint;
        public LayerMask enemyLayerMask;

        private float fireCountdown;
        private float rangeScale;
        private Transform currentTarget;
        private ContactFilter2D contactFilter;
        private List<Collider2D> targetsBuffer = new List<Collider2D>();

        /// <summary>Asset con i valori base: sola lettura. Per i valori correnti usare <see cref="RuntimeStats"/>.</summary>
        public TowerData Data => data;

        /// <summary>Statistiche di questa torretta; è l'unico posto dove i poteri possono applicare modificatori.</summary>
        public TowerRuntimeStats RuntimeStats { get; private set; }

        private float EffectiveRange => RuntimeStats.Range * rangeScale;

        void Awake()
        {
            contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(enemyLayerMask);

            CircleCollider2D col = GetComponent<CircleCollider2D>();
            if (col != null)
            {
                col.radius = data.Range;
            }

            rangeScale = Mathf.Abs(transform.lossyScale.x);

            RuntimeStats = new TowerRuntimeStats(data);
        }

        protected override void OnSpawned()
        {
            fireCountdown = 0f;
            currentTarget = null;
            RuntimeStats.ResetModifiers();
        }

        protected override void OnDespawned()
        {
            currentTarget = null;
        }

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
                fireCountdown = 1f / RuntimeStats.FireRate;
            }

            fireCountdown -= Time.deltaTime;
        }

        bool IsTargetValid(Transform target)
        {
            if (target == null || !target.gameObject.activeInHierarchy)
                return false;

            float distance = Vector2.Distance(transform.position, target.position);
            return distance <= EffectiveRange;
        }

        Transform FindClosestEnemy()
        {
            targetsBuffer.Clear();
            Physics2D.OverlapCircle(transform.position, EffectiveRange, contactFilter, targetsBuffer);

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
            ObjectPool pool = PoolManager.Instance.GetPool(data.ProjectileData.projectilePrefab, 10);

            GameObject projectileGO = pool.Get();
            projectileGO.transform.position = firePoint.position;

            ProjectileController projectile = projectileGO.GetComponent<ProjectileController>();
            projectile.Init(target, RuntimeStats.Damage);
        }
    }
}
