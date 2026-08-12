using System.Collections;
using UnityEngine;

public class Firearm : Weapon
{
    
    [Header("Aim")]
    [SerializeField] private Transform aimPivot;

    
    // BULLET / POOL

    [Header("Bullet")]
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform bulletPoolParent;
    [SerializeField] private int initialBulletPoolSize = 20;

    

    [Header("Bullet Stats")]
    [SerializeField] private float bulletDamage = 20f;
    [SerializeField] private float bulletSpeed = 12f;
    [SerializeField] private float bulletLifeTime = 3f;

    

    [Header("Fire")]

    // Punto exacto desde donde sale la bala.
    [SerializeField] private Transform firePoint;

    [SerializeField] private float fireCooldown = 0.2f;

    [SerializeField] private float bulletSpawnDistance = 0.6f;

    // ==========================
    // AMMO
    // ==========================

    [Header("Ammo")]
    [SerializeField] private int magazineSize = 6;
    [SerializeField] private int reserveAmmo = 24;
    [SerializeField] private float reloadTime = 1.2f;


    [Header("Collision")]
    [SerializeField] private LayerMask bulletHitLayers;


    private ObjectPool bulletPool;

    private int currentAmmo;

    private float fireCooldownRemaining;

    private bool isReloading;

    // ==========================
    // PUBLIC DATA
    // ==========================

    public int CurrentAmmo => currentAmmo;
    public int ReserveAmmo => reserveAmmo;
    public int MagazineSize => magazineSize;
    public bool IsReloading => isReloading;


    private void Awake()
    {
        currentAmmo = magazineSize;

        if (bulletPrefab == null)
        {
            Debug.LogError(
                $"{name}: Bullet Prefab no asignado."
            );

            return;
        }

        bulletPool = new ObjectPool(
            bulletPrefab.gameObject,
            initialBulletPoolSize,
            bulletPoolParent
        );
    }

    private void Update()
    {
        if (fireCooldownRemaining > 0f)
        {
            fireCooldownRemaining -= Time.deltaTime;
        }
    }

    // ==========================
    // AIM
    // ==========================

    public override void SetAimDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.001f)
            return;

        if (aimPivot == null)
            return;

        float angle =
            Mathf.Atan2(
                direction.y,
                direction.x
            ) * Mathf.Rad2Deg;

        aimPivot.rotation =
            Quaternion.Euler(
                0f,
                0f,
                angle
            );
    }

    // ==========================
    // ATTACK
    // ==========================

    public override void Attack(Vector2 direction)
    {
        if (isReloading)
            return;

        if (fireCooldownRemaining > 0f)
            return;

        if (direction.sqrMagnitude < 0.001f)
            return;

        if (currentAmmo <= 0)
        {
            StartReload();
            return;
        }

        if (bulletPool == null)
            return;

        direction.Normalize();

        Fire(direction);
    }

    // ==========================
    // FIRE
    // ==========================

    private void Fire(Vector2 direction)
    {
        fireCooldownRemaining =
            fireCooldown;

        currentAmmo--;

        Vector2 spawnPosition =
            firePoint != null
                ? (Vector2)firePoint.position
                : (Vector2)transform.position
                    + direction * bulletSpawnDistance;

        GameObject bulletObject =
            bulletPool.Get(
                spawnPosition,
                Quaternion.identity
            );

        if (
            bulletObject.TryGetComponent(
                out Bullet bullet
            )
        )
        {
            bullet.Launch(
                direction,
                bulletSpeed,
                bulletDamage,
                bulletLifeTime,
                bulletHitLayers,
                bulletPool
            );
        }
        else
        {
            Debug.LogError(
                "El prefab usado en Firearm " +
                "no tiene Bullet.cs."
            );

            bulletPool.Release(
                bulletObject
            );
        }

        Debug.Log(
            $"Ammo: {currentAmmo}/{reserveAmmo}"
        );
    }

    // ==========================
    // RELOAD
    // ==========================

    public override void Reload()
    {
        StartReload();
    }

    private void StartReload()
    {
        if (isReloading)
            return;

        if (currentAmmo >= magazineSize)
            return;

        if (reserveAmmo <= 0)
            return;

        StartCoroutine(
            ReloadCoroutine()
        );
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;

        Debug.Log("Reloading...");

        yield return new WaitForSeconds(
            reloadTime
        );

        int missingAmmo =
            magazineSize - currentAmmo;

        int amountToReload =
            Mathf.Min(
                missingAmmo,
                reserveAmmo
            );

        currentAmmo += amountToReload;

        reserveAmmo -= amountToReload;

        isReloading = false;

        Debug.Log(
            $"Reloaded: " +
            $"{currentAmmo}/{reserveAmmo}"
        );
    }
}