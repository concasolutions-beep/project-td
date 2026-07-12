using UnityEngine;

//Controller per centralizzare script nemici
[RequireComponent(typeof(HealthController))]
[RequireComponent(typeof(EnemyMovement))]
public class EnemyController : MonoBehaviour
{

    public EnemyData data;
    private HealthController health;
    private EnemyMovement movement;

   void Awake()
    {
        health = GetComponent<HealthController>();
        movement = GetComponent<EnemyMovement>();
    }

    void Start()
    {
        Init();
    }

    public void Init()
    {
        if (data == null)
        {
            return;
        }


        health.SetMaxHealth(data.maxHealth);
        movement.SetSpeed(data.speed);
    }

    public void SetData(EnemyData enemyData)
    {
        data = enemyData;
        Init();
    }
}
