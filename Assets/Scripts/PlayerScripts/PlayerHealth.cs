using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Damage Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float damageFlashDuration = 0.1f;
    [SerializeField] private Color damageColor = Color.red;

    private Color originalColor;
    private Coroutine damageFlashCoroutine;

    private float currentHealth;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    public bool IsDead { get; private set; }

    public event Action<float, float> OnHealthChanged;
    public event Action OnPlayerDied;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        originalColor = spriteRenderer.color;
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

        FlashDamage();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void FlashDamage()
    {
        if (damageFlashCoroutine != null)
            StopCoroutine(damageFlashCoroutine);

        damageFlashCoroutine = StartCoroutine(DamageFlashCoroutine());
    }

    private IEnumerator DamageFlashCoroutine()
    {
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        spriteRenderer.color = originalColor;
        damageFlashCoroutine = null;
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
    }
}