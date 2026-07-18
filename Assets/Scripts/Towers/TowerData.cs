using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Scriptable Objects/TowerData")]
public class TowerData : ScriptableObject
{
    [Header("Combat")]
    public float range = 3f;
    public float fireRate = 1f;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public float speed = 10f;
    public float damage = 1f;
    public float maxLifetime = 5f;

    [Header("Meta")]
    public string towerName;
    public int cost;
}
