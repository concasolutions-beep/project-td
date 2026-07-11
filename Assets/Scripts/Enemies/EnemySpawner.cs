using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Riferimenti")]
    public GameObject enemyPrefab;
    public Transform waypointsParent;

    [Header("Impostazioni ondata")]
    public int enemyCount = 5;
    public float spawnInterval = 1f;

    void Start()
    {
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        Vector3 startPos = waypointsParent.GetChild(0).position;

        GameObject enemy = Instantiate(enemyPrefab, startPos, Quaternion.identity);

        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        movement.waypointsParent = waypointsParent;
    }
}