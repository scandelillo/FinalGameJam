using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Va en el prefab de cada zombie. No sabe nada de pooling directamente:
/// recibe sus stats vía Initialize() y avisa con un evento cuando muere,
/// para que el pool lo recicle y el spawner descuente el contador de oleada.
/// </summary>
public class ZombieController : MonoBehaviour, IDamageable
{
    public event Action<ZombieController> OnZombieDied;

    public float CurrentHealth { get; private set; }
    public float Damage { get; private set; }
    public float Speed { get; private set; }

    private ZombieTypeSO typeData;
    private ZombiePoolManager poolManager;

    [Header("Damage Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float damageFlashDuration = 0.1f;
    [SerializeField] private Color damageColor = Color.red;

    private Color originalColor;
    private Coroutine damageFlashCoroutine;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        originalColor = spriteRenderer.color;
    }


    public void Initialize(ZombieTypeSO data, int wave, ZombiePoolManager manager)
    {
        typeData = data;
        poolManager = manager;

        CurrentHealth = data.GetHealthForWave(wave);
        Damage = data.GetDamageForWave(wave);
        Speed = data.GetSpeedForWave(wave);
    }

    public void TakeDamage(float amount)
    {
        Debug.Log($"[DEBUG] {name} recibió {amount} de daño. Vida restante: {CurrentHealth - amount}");

        CurrentHealth -= amount;
        
        FlashDamage();

        if (CurrentHealth <= 0f)
            Die();
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



    private void Die()
    {
        Debug.Log($"[DEBUG] {name} está muriendo, ejecutando Die()");

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddPoints(typeData.pointsValue);

        TryDropAmmo();

        Debug.Log($"[DEBUG] {name} terminó Die(), devolviendo al pool");

        // Aquí puedes disparar animación de muerte, etc.
        OnZombieDied?.Invoke(this);
        poolManager.ReturnZombie(typeData, gameObject);
    }

    private void TryDropAmmo()
    {
        if (typeData.ammoPickupPrefab == null) return;
        if (UnityEngine.Random.value > typeData.ammoDropChance) return;

        int amount = UnityEngine.Random.Range(typeData.minAmmoDrop, typeData.maxAmmoDrop + 1);

        GameObject pickupObj = Instantiate(typeData.ammoPickupPrefab, transform.position, Quaternion.identity);

        if (pickupObj.TryGetComponent(out AmmoPickup pickup))
            pickup.SetAmount(amount);
    }

    private void OnDisable()
    {
        if (damageFlashCoroutine != null)
        {
            StopCoroutine(damageFlashCoroutine);
            damageFlashCoroutine = null;
        }

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }
}