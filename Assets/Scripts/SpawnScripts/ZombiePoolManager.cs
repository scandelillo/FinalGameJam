using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mantiene un ObjectPool separado por cada ZombieTypeSO.
/// Es el único punto de entrada para spawnear/devolver zombies.
/// </summary>
public class ZombiePoolManager : MonoBehaviour
{
    public static ZombiePoolManager Instance { get; private set; }

    [SerializeField] private int initialPoolSizePerType = 10;

    private readonly Dictionary<ZombieTypeSO, ObjectPool> pools = new Dictionary<ZombieTypeSO, ObjectPool>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterZombieType(ZombieTypeSO type)
    {
        if (pools.ContainsKey(type)) return;
        pools[type] = new ObjectPool(type.prefab, initialPoolSizePerType, transform);
    }

    public GameObject SpawnZombie(ZombieTypeSO type, Vector3 position, int wave)
    {
        if (!pools.ContainsKey(type))
            RegisterZombieType(type);

        GameObject zombieObj = pools[type].Get(position, Quaternion.identity);

        if (zombieObj.TryGetComponent(out ZombieController controller))
            controller.Initialize(type, wave, this);

        return zombieObj;
    }

    public void ReturnZombie(ZombieTypeSO type, GameObject zombieObj)
    {
        if (pools.ContainsKey(type))
            pools[type].Release(zombieObj);
        else
            Destroy(zombieObj); // fallback de seguridad, no debería pasar en flujo normal
    }
}
