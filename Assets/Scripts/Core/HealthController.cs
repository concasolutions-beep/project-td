using UnityEngine;
using System;

public class HealthController : MonoBehaviour
{

    public float maxHealth = 5f;
    private float currentHealth;
    private bool isDead;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float NormalizedHealth => maxHealth > 0f ? currentHealth / maxHealth : 0f;

    public event Action<float, float> OnHealthChanged;
    public event Action<float, float, float> OnDamaged;
    public event Action OnDied;

    void Awake()
    {
        maxHealth = Mathf.Max(0f, maxHealth);
        currentHealth = maxHealth;
        isDead = false;
    }

    public void TakeDamage(float amount)
    {
        if (isDead || amount <= 0f)
        {
            return;
        }

        float previousHealth = currentHealth;
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (Mathf.Approximately(previousHealth, currentHealth))
        {
            return;
        }

        OnDamaged?.Invoke(amount, currentHealth, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        OnDied?.Invoke();
        Debug.Log(gameObject.name + " morto cuppato");
        Destroy(gameObject);
    }
}
