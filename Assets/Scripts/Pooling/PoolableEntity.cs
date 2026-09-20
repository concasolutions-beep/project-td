using UnityEngine;

public abstract class PoolableEntity : MonoBehaviour, IPoolable
{
    private ObjectPool ownerPool;
    private bool isReleased;

    protected bool IsReleased => isReleased;

    public void Bind(ObjectPool pool)
    {
        ownerPool = pool;
    }

    public void OnSpawn()
    {
        isReleased = false;
        OnSpawned();
    }

    public void OnDespawn()
    {
        OnDespawned();
    }

    protected void ReleaseToPool()
    {
        if (isReleased)
        {
            return;
        }
        isReleased = true;

        if (ownerPool != null)
        {
            ownerPool.Release(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected abstract void OnSpawned();
    protected abstract void OnDespawned();
}
