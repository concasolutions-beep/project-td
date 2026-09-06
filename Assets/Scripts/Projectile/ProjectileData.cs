using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Scriptable Objects/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public float speed = 10f;
    public float damage = 1f;
    public float maxLifetime = 5f;
}
