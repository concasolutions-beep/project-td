using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Percorso")]
    public Transform waypointsParent;
    public float speed = 3f;            // velocità di movimento

    private Transform[] waypoints;
    private int currentIndex = 0;

    void Start()
    {
        int count = waypointsParent.childCount;
        waypoints = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            waypoints[i] = waypointsParent.GetChild(i);
        }

        if (waypoints.Length > 0)
            transform.position = waypoints[0].position;
    }

    void Update()
    {
        if (currentIndex >= waypoints.Length)
            return;

        Transform target = waypoints[currentIndex];

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target.position) < 0.05f)
        {
            currentIndex++;
        }
    }
}