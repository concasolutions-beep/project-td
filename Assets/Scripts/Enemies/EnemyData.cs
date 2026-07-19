using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Prefab")]
    public GameObject prefab;

    [Header("Stats")]
    public float maxHealth = 10f;
    public float speed = 2f;
    public int damage = 1;

    [Header("Rewards")]
    public int gold = 1;

    [Header("Meta")]
    public string enemyName;
}
