using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Max Health Upgrade")]
    [SerializeField] private float absoluteMaxHealth = 200f;

    [Header("Damage Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float damageFlashDuration = 0.1f;
    [SerializeField] private Color damageColor = Color.red;

    private Color originalColor;
    private Coroutine damageFlashCoroutine;

    private float currentHealth;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float AbsoluteMaxHealth => absoluteMaxHealth;

    public bool IsDead { get; private set; }

    public bool IsMaxHealthUpgraded =>
        maxHealth >= absoluteMaxHealth;

    public event Action<float, float> OnHealthChanged;
    public event Action OnPlayerDied;
    public event Action<float> OnPlayerDamaged;


    private void Awake()
    {
        currentHealth = maxHealth;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
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

        NotifyHealthChanged();

        OnPlayerDamaged?.Invoke(amount);

        FlashDamage();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }


    private void FlashDamage()
    {
        if (spriteRenderer == null)
            return;

        if (damageFlashCoroutine != null)
        {
            StopCoroutine(damageFlashCoroutine);
        }

        damageFlashCoroutine =
            StartCoroutine(
                DamageFlashCoroutine()
            );
    }

    private IEnumerator DamageFlashCoroutine()
    {
        spriteRenderer.color = damageColor;

        yield return new WaitForSeconds(
            damageFlashDuration
        );

        spriteRenderer.color = originalColor;

        damageFlashCoroutine = null;
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

        float previousMaxHealth =
            maxHealth;

        maxHealth += amount;

        maxHealth = Mathf.Clamp(
            maxHealth,
            0f,
            absoluteMaxHealth
        );

        float realIncrease =
            maxHealth - previousMaxHealth;

        // También ganamos esa misma cantidad
        // como vida actual.
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