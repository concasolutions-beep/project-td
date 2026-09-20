public interface IPoolable
{
    void Bind(ObjectPool pool);
    void OnSpawn();
    void OnDespawn();
}
