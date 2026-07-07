using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Scriptable Objects/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    [Header("Movement")]
    public float speed = 10f;

    [Header("Combat")]
    public float damage = 1f;

    [Header("Lifetime")]
    public float maxLifetime = 5f;
}
