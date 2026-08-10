using System;
using UnityEngine;

/// <summary>
/// Va en el prefab de cada zombie. No sabe nada de pooling directamente:
/// recibe sus stats vía Initialize() y avisa con un evento cuando muere,
/// para que el pool lo recicle y el spawner descuente el contador de oleada.
/// </summary>
public class ZombieController : MonoBehaviour
{
    public event Action<ZombieController> OnZombieDied;

    public float CurrentHealth { get; private set; }
    public float Damage { get; private set; }
    public float Speed { get; private set; }

    private ZombieTypeSO typeData;
    private ZombiePoolManager poolManager;

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
        CurrentHealth -= amount;
        if (CurrentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        // Aquí puedes disparar animación de muerte, drop de items, sumar puntos, etc.
        OnZombieDied?.Invoke(this);
        poolManager.ReturnZombie(typeData, gameObject);
    }
}
