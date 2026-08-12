using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    public bool IsDead { get; private set; }

    public event Action<float, float> OnHealthChanged;
    public event Action OnPlayerDied;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead)
            return;

        if (amount <= 0f)
            return;

        currentHealth -= amount;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        Debug.Log(
            $"Player recibió {amount} de daño. " +
            $"Vida: {currentHealth}/{maxHealth}"
        );

        OnHealthChanged?.Invoke(
            currentHealth,
            maxHealth
        );

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead)
            return;

        if (amount <= 0f)
            return;

        currentHealth += amount;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        OnHealthChanged?.Invoke(
            currentHealth,
            maxHealth
        );
    }

    private void Die()
    {
    if (IsDead)
        return;

    IsDead = true;

    Debug.Log("MUERTO");

    OnPlayerDied?.Invoke();

    Time.timeScale = 0f;
  }
}