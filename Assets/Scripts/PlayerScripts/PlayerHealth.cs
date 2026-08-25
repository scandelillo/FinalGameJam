using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Max Health Upgrade")]
    [SerializeField] private float absoluteMaxHealth = 200f;

    private float currentHealth;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float AbsoluteMaxHealth => absoluteMaxHealth;

    public bool IsDead { get; private set; }

    public bool IsMaxHealthUpgraded =>
        maxHealth >= absoluteMaxHealth;

    public event Action<float, float> OnHealthChanged;
    public event Action OnPlayerDied;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    // ==========================
    // DAMAGE
    // ==========================

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

        NotifyHealthChanged();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    // ==========================
    // HEAL
    // ==========================

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

        NotifyHealthChanged();
    }

    // ==========================
    // MAX HEALTH UPGRADE
    // ==========================

    public bool IncreaseMaxHealth(float amount)
    {
        if (IsDead)
            return false;

        if (amount <= 0f)
            return false;

        if (maxHealth >= absoluteMaxHealth)
            return false;

        float previousMaxHealth = maxHealth;

        maxHealth += amount;

        maxHealth = Mathf.Clamp(
            maxHealth,
            0f,
            absoluteMaxHealth
        );

        float realIncrease =
            maxHealth - previousMaxHealth;

        
        currentHealth += realIncrease;

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        Debug.Log(
            $"Vida máxima aumentada: " +
            $"{previousMaxHealth} → {maxHealth}"
        );

        NotifyHealthChanged();

        return true;
    }

    // ==========================
    // EVENTS
    // ==========================

    private void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(
            currentHealth,
            maxHealth
        );
    }

    // ==========================
    // DEATH
    // ==========================

    private void Die()
    {
        if (IsDead)
            return;

        IsDead = true;

        Debug.Log("MUERTO");

        OnPlayerDied?.Invoke();
    }
}