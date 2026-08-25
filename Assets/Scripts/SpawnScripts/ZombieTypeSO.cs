using UnityEngine;

/// <summary>
/// Define los datos de un tipo de zombie: stats base y cómo escalan con las oleadas.
/// Crear un asset por cada tipo desde: Assets > Create > Zombies > Zombie Type.
/// </summary>
[CreateAssetMenu(fileName = "NewZombieType", menuName = "Zombies/Zombie Type")]
public class ZombieTypeSO : ScriptableObject
{
    [Header("Identidad")]
    public string zombieName = "Zombie";
    public GameObject prefab;

    [Header("Stats base (oleada 1)")]
    public float baseHealth = 30f;
    public float baseDamage = 5f;
    public float baseSpeed = 2f;
    [Tooltip("Puntos que otorga al jugador al morir")]
    public int pointsValue = 10;

    [Header("Escalado por oleada")]
    [Tooltip("Fracción de crecimiento por oleada, ej. 0.15 = +15% por oleada")]
    public float healthGrowthPerWave = 0.15f;
    public float damageGrowthPerWave = 0.10f;
    public float speedGrowthPerWave = 0.02f;
    [Tooltip("Límite superior de velocidad para que no se vuelva injugable en oleadas altas")]
    public float speedCap = 4f;

    [Header("Drop de munición")]
    [Range(0f, 1f)]
    [Tooltip("Probabilidad de soltar munición al morir, 0 = nunca, 1 = siempre")]
    public float ammoDropChance = 0.15f;
    public GameObject ammoPickupPrefab;
    public int minAmmoDrop = 6;
    public int maxAmmoDrop = 12;

    public float GetHealthForWave(int wave) =>
        baseHealth * (1f + healthGrowthPerWave * (wave - 1));

    public float GetDamageForWave(int wave) =>
        baseDamage * (1f + damageGrowthPerWave * (wave - 1));

    public float GetSpeedForWave(int wave) =>
        Mathf.Min(baseSpeed * (1f + speedGrowthPerWave * (wave - 1)), speedCap);
}