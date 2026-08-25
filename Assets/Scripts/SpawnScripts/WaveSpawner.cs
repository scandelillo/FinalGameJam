using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Orquesta las rondas de forma GLOBAL (estilo COD Zombies): la ronda sube
/// aunque solo tengas una zona desbloqueada. Solo se spawnea en los puntos
/// de las zonas ya desbloqueadas. Otro sistema (puertas/puntos) llama a
/// UnlockZone() cuando el jugador paga el desbloqueo.
/// </summary>
public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ZombieWaveEntry
    {
        public ZombieTypeSO zombieType;
        [Tooltip("Cantidad de este tipo en la ronda 1")]
        public int baseCount = 3;
        [Tooltip("Cuántos más de este tipo aparecen por cada ronda adicional")]
        public int countGrowthPerWave = 1;
    }

    [System.Serializable]
    public class ZoneSpawnGroup
    {
        public ZoneId zoneId;
        public Transform[] spawnPoints;
        [Tooltip("La habitación inicial debe empezar desbloqueada")]
        public bool startsUnlocked = false;

        [HideInInspector] public bool isUnlocked;
    }

    [Header("Tipos de zombie (3 entradas = 3 tipos distintos)")]
    [SerializeField] private List<ZombieWaveEntry> zombieEntries;

    [Header("Zonas y sus puntos de spawn")]
    [SerializeField] private List<ZoneSpawnGroup> zones;

    [Header("Ritmo")]
    [SerializeField] private float delayBetweenSpawns = 0.4f;
    [SerializeField] private float delayBetweenWaves = 5f;

    private int currentWave = 0;
    private int zombiesAliveThisWave = 0;

    private void Start()
    {
        foreach (var entry in zombieEntries)
            ZombiePoolManager.Instance.RegisterZombieType(entry.zombieType);

        foreach (var zone in zones)
            zone.isUnlocked = zone.startsUnlocked;

        StartCoroutine(RunWaves());
    }

    /// <summary>
    /// Llamado por el sistema de puertas/puntos cuando el jugador desbloquea una zona.
    /// </summary>
    public void UnlockZone(ZoneId zoneId)
    {
        ZoneSpawnGroup zone = zones.Find(z => z.zoneId == zoneId);
        if (zone == null)
        {
            Debug.LogWarning($"WaveSpawner: no hay ninguna ZoneSpawnGroup configurada con ZoneId.{zoneId}");
            return;
        }
        zone.isUnlocked = true;
    }

    private IEnumerator RunWaves()
    {
        while (true)
        {
            currentWave++;
            yield return StartCoroutine(SpawnWave(currentWave));

            yield return new WaitUntil(() => zombiesAliveThisWave <= 0);
            yield return new WaitForSeconds(delayBetweenWaves);
        }
    }

    private IEnumerator SpawnWave(int wave)
    {
        foreach (var entry in zombieEntries)
        {
            int countThisWave = entry.baseCount + entry.countGrowthPerWave * (wave - 1);

            for (int i = 0; i < countThisWave; i++)
            {
                Transform point = GetRandomUnlockedSpawnPoint();
                if (point == null)
                {
                    // No hay ninguna zona desbloqueada todavía; espera un poco y reintenta
                    yield return new WaitForSeconds(delayBetweenSpawns);
                    continue;
                }

                SpawnOne(entry.zombieType, wave, point);
                zombiesAliveThisWave++;
                yield return new WaitForSeconds(delayBetweenSpawns);
            }
        }
    }

    private Transform GetRandomUnlockedSpawnPoint()
    {
        List<Transform> availablePoints = new List<Transform>();

        foreach (var zone in zones)
        {
            if (!zone.isUnlocked) continue;
            availablePoints.AddRange(zone.spawnPoints);
        }

        if (availablePoints.Count == 0) return null;
        return availablePoints[Random.Range(0, availablePoints.Count)];
    }

    private void SpawnOne(ZombieTypeSO type, int wave, Transform point)
    {
        GameObject zombieObj = ZombiePoolManager.Instance.SpawnZombie(type, point.position, wave);

        if (zombieObj.TryGetComponent(out ZombieController controller))
            controller.OnZombieDied += HandleZombieDied;
    }

    private void HandleZombieDied(ZombieController controller)
    {
        controller.OnZombieDied -= HandleZombieDied;
        zombiesAliveThisWave--;
    }
}
