using UnityEngine;
using System.Collections.Generic;
using ProjectTD.Core;

namespace ProjectTD.Pooling
{
    public class PoolManager : Singleton<PoolManager>
    {
        // A differenza di GameManager, PoolManager non e' mai piazzato a mano in scena:
        // viene creato al volo alla prima richiesta.
        public new static PoolManager Instance
        {
            get
            {
                PoolManager found = Singleton<PoolManager>.Instance;
                if (found == null)
                {
                    GameObject go = new GameObject(nameof(PoolManager));
                    found = go.AddComponent<PoolManager>();
                }
                return found;
            }
        }

        private readonly Dictionary<GameObject, ObjectPool> pools = new Dictionary<GameObject, ObjectPool>();

        public ObjectPool GetPool(GameObject prefab, int size = 10)
        {
            if (!pools.TryGetValue(prefab, out ObjectPool pool))
            {
                pool = new ObjectPool(prefab, size, transform);
                pools.Add(prefab, pool);
            }

            return pool;
        }
    }
}
