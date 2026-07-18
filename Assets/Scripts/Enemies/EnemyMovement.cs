using UnityEngine;
using System;

public class EnemyMovement : MonoBehaviour
{
    [Header("Percorso")]
    public Transform waypointsParent;
    private float speed = 3f;            // velocità di movimento

    private Transform[] waypoints;
    private int currentIndex = 0;
    private bool hasReachedBase;

    public event Action<EnemyMovement> OnReachedBase;

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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasReachedBase)
        {
            return;
        }

        if (other.CompareTag("Base"))
        {
            hasReachedBase = true;
            OnReachedBase?.Invoke(this);
            HealthController health = other.GetComponent<HealthController>();
            if (health != null)
            {
                health.TakeDamage(1);
            }
            Destroy(gameObject);
        }
    }

    // Distanza rimanente lungo il percorso fino all'ultimo waypoint (usata per il targeting delle torrette)
    public float GetRemainingDistance()
    {
        if (waypoints == null || currentIndex >= waypoints.Length)
            return 0f;

        float distance = Vector2.Distance(transform.position, waypoints[currentIndex].position);
        for (int i = currentIndex; i < waypoints.Length - 1; i++)
        {
            distance += Vector2.Distance(waypoints[i].position, waypoints[i + 1].position);
        }

        return distance;
    }

    public void SetSpeed(float value)
    {
        speed = value;
    }
}