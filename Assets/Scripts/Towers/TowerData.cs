using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Scriptable Objects/TowerData")]
public class TowerData : ScriptableObject
{
    [Header("Combat")]
    public float range = 3f;
    public float fireRate = 1f;
    public ProjectileData projectileData;
    [Header("Meta")]
    public string towerName;
    public int cost;
}
