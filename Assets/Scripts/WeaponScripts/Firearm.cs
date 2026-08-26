using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Firearm : Weapon, IAmmoContainer
{

    [Header("Aim")]
    [SerializeField] private Transform aimPivot;

    private SpriteRenderer weaponSpriteRenderer;

    

    [Header("Bullet")]
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform bulletPoolParent;
    [SerializeField] private int initialBulletPoolSize = 20;

    

    [Header("Bullet Stats")]
    [SerializeField] private float bulletDamage = 20f;
    [SerializeField] private float bulletSpeed = 12f;
    [SerializeField] private float bulletLifeTime = 3f;

    

    [Header("Fire")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireCooldown = 0.2f;

    // Respaldo si FirePoint no está asignado.
    [SerializeField] private float bulletSpawnDistance = 0.6f;

    

    [Header("Muzzle Flash")]
    [SerializeField] private Light2D muzzleFlashLight;
    [SerializeField] private float muzzleFlashDuration = 0.04f;

    private float muzzleFlashTimer;

    

    [Header("Full Auto Recoil")]
    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField]
    private float fullAutoRecoilStrength = 0.8f;

    

    [Header("Fire Mode")]
    [SerializeField] private bool automaticFireUnlocked = false;

    

    [Header("Ammo")]
    [SerializeField] private int magazineSize = 6;
    [SerializeField] private int reserveAmmo = 24;
    [SerializeField] private float reloadTime = 1.2f;

    

    [Header("Magazine Upgrade")]
    [SerializeField] private int maxMagazineSize = 30;


    [Header("Collision")]
    [SerializeField] private LayerMask bulletHitLayers;

    

    private ObjectPool bulletPool;

    private int currentAmmo;

    private float fireCooldownRemaining;

    private bool isReloading;

    

    public int CurrentAmmo =>
        currentAmmo;

    public int ReserveAmmo =>
        reserveAmmo;

    public int MagazineSize =>
        magazineSize;

    public int MaxMagazineSize =>
        maxMagazineSize;

    public bool IsReloading =>
        isReloading;

    public bool IsAutomaticFireEnabled =>
        automaticFireUnlocked;

    public bool IsMagazineMaxed =>
        magazineSize >= maxMagazineSize;

    private void Awake()
    {
        currentAmmo =
            magazineSize;

        if (weaponVisual != null)
        {
            weaponSpriteRenderer =
                weaponVisual
                    .GetComponentInChildren<SpriteRenderer>();
        }

        // Busca automáticamente PlayerMovement.
        if (playerMovement == null)
        {
            playerMovement =
                GetComponentInParent<PlayerMovement>();
        }

        if (
            muzzleFlashLight == null &&
            firePoint != null
        )
        {
            muzzleFlashLight =
                firePoint
                    .GetComponentInChildren<Light2D>(
                        true
                    );
        }

        // Al comenzar la luz debe estar apagada.
        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.enabled =
                false;
        }

        if (bulletPrefab == null)
        {
            Debug.LogError(
                $"{name}: Bullet Prefab no asignado."
            );

            return;
        }

        bulletPool =
            new ObjectPool(
                bulletPrefab.gameObject,
                initialBulletPoolSize,
                bulletPoolParent
            );
    }

    private void Update()
    {

        if (fireCooldownRemaining > 0f)
        {
            fireCooldownRemaining -=
                Time.deltaTime;
        }

        if (muzzleFlashTimer > 0f)
        {
            muzzleFlashTimer -=
                Time.deltaTime;

            if (muzzleFlashTimer <= 0f)
            {
                if (muzzleFlashLight != null)
                {
                    muzzleFlashLight.enabled =
                        false;
                }
            }
        }
    }

    public override void SetAimDirection(
        Vector2 direction)
    {
        if (
            direction.sqrMagnitude <
            0.001f
        )
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

        // Evita que el arma quede boca abajo
        // cuando apunta hacia la izquierda.
        if (weaponSpriteRenderer != null)
        {
            weaponSpriteRenderer.flipY =
                direction.x < 0f;
        }
    }

    public override void Attack(
        Vector2 direction)
    {
        if (isReloading)
            return;

        if (fireCooldownRemaining > 0f)
            return;

        if (
            direction.sqrMagnitude <
            0.001f
        )
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

    private void Fire(
        Vector2 direction)
    {
        fireCooldownRemaining =
            fireCooldown;

        currentAmmo--;

        TriggerMuzzleFlash();

        if (automaticFireUnlocked)
            AudioManager.Instance.sfxManager.PlayIndividualShot();
        else
            AudioManager.Instance.sfxManager.PlayIndividualShot();

        // Recoil solamente si Full Auto
        // está desbloqueado.
        if (
            automaticFireUnlocked &&
            playerMovement != null
        )
        {
            playerMovement.ApplyRecoil(
                direction,
                fullAutoRecoilStrength
            );
        }

        Vector2 spawnPosition =
            firePoint != null
                ? (Vector2)firePoint.position
                : (Vector2)transform.position
                    + direction *
                    bulletSpawnDistance;

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
    // MUZZLE FLASH
    // ==========================

    private void TriggerMuzzleFlash()
    {
        if (muzzleFlashLight == null)
            return;

        muzzleFlashLight.enabled =
            true;

        muzzleFlashTimer =
            muzzleFlashDuration;
    }

    // ==========================
    // AUTOMATIC FIRE UPGRADE
    // ==========================

    public bool UnlockAutomaticFire()
    {
        if (automaticFireUnlocked)
        {
            return false;
        }

        automaticFireUnlocked =
            true;

        Debug.Log(
            $"{name}: AUTOMATIC FIRE desbloqueado."
        );

        return true;
    }

    // ==========================
    // MAGAZINE UPGRADE
    // ==========================

    public bool IncreaseMagazineSize(
        int amount)
    {
        if (amount <= 0)
            return false;

        if (
            magazineSize >=
            maxMagazineSize
        )
            return false;

        int previousMagazineSize =
            magazineSize;

        magazineSize +=
            amount;

        magazineSize =
            Mathf.Clamp(
                magazineSize,
                1,
                maxMagazineSize
            );

        int realIncrease =
            magazineSize -
            previousMagazineSize;

        currentAmmo +=
            realIncrease;

        currentAmmo =
            Mathf.Clamp(
                currentAmmo,
                0,
                magazineSize
            );

        Debug.Log(
            $"Cargador aumentado: " +
            $"{previousMagazineSize} → " +
            $"{magazineSize}"
        );

        return true;
    }

    public override void Reload()
    {
        StartReload();
    }

    private void StartReload()
    {
        if (isReloading)
            return;

        if (
            currentAmmo >=
            magazineSize
        )
            return;

        if (reserveAmmo <= 0)
            return;

        AudioManager.Instance.sfxManager.PlayReload();
        StartCoroutine(
            ReloadCoroutine()
        );
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading =
            true;

        Debug.Log(
            "Reloading..."
        );

        yield return
            new WaitForSeconds(
                reloadTime
            );

        int missingAmmo =
            magazineSize -
            currentAmmo;

        int amountToReload =
            Mathf.Min(
                missingAmmo,
                reserveAmmo
            );

        currentAmmo +=
            amountToReload;

        reserveAmmo -=
            amountToReload;

        isReloading =
            false;

        Debug.Log(
            $"Reloaded: " +
            $"{currentAmmo}/{reserveAmmo}"
        );
    }

    public void AddReserveAmmo(
        int amount)
    {
        if (amount <= 0)
            return;

        reserveAmmo +=
            amount;

        Debug.Log(
            $"+{amount} munición de reserva " +
            $"({reserveAmmo} total)"
        );
    }
}