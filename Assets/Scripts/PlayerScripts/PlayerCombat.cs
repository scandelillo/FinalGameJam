using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private Camera combatCamera;

    [Header("Weapons")]
    [SerializeField] private Weapon[] weapons = new Weapon[3];

    private int currentSlot = 0;
    private int previousSlot = -1;

    private Vector2 mouseScreenPosition;
    private Vector2 aimDirection = Vector2.down;

    // Permite bloquear todos los inputs de combate.
    private bool combatEnabled = true;

    public Vector2 AimDirection => aimDirection;
    public int CurrentSlot => currentSlot;

    public Weapon CurrentWeapon
    {
        get
        {
            if (weapons == null)
                return null;

            if (currentSlot < 0 || currentSlot >= weapons.Length)
                return null;

            return weapons[currentSlot];
        }
    }

    private void Awake()
    {
        if (movement == null)
        {
            movement = GetComponent<PlayerMovement>();
        }

        if (combatCamera == null)
        {
            combatCamera = Camera.main;
        }
    }

    private void Start()
    {
        InitializeWeapons();
    }

    private void Update()
    {
        // Actualizamos la dirección hacia el mouse.
        UpdateAimDirection();

        // Volteamos visualmente el jugador hacia el mouse.
        if (movement != null)
        {
            movement.SetAimFacing(aimDirection);
        }

        // El arma actual sigue apuntando al mouse.
        UpdateWeaponAim();
    }

    public void SetCombatEnabled(bool enabled)
    {
        combatEnabled = enabled;
    }

    // ==========================
    // AIM
    // ==========================

    public void OnAim(InputValue value)
    {
        mouseScreenPosition = value.Get<Vector2>();
    }

    private void UpdateAimDirection()
    {
        if (combatCamera == null)
            return;

        float cameraDistance =
            Mathf.Abs(
                combatCamera.transform.position.z
                - transform.position.z
            );

        Vector3 screenPosition =
            new Vector3(
                mouseScreenPosition.x,
                mouseScreenPosition.y,
                cameraDistance
            );

        Vector3 mouseWorldPosition =
            combatCamera.ScreenToWorldPoint(
                screenPosition
            );

        Vector2 direction =
            (Vector2)mouseWorldPosition
            - (Vector2)transform.position;

        if (direction.sqrMagnitude > 0.001f)
        {
            aimDirection = direction.normalized;
        }
    }

    private void UpdateWeaponAim()
    {
        if (CurrentWeapon == null)
            return;

        CurrentWeapon.SetAimDirection(
            aimDirection
        );
    }

    // ==========================
    // ATTACK
    // ==========================

    public void OnAttack(InputValue value)
    {
        // Si el combate está bloqueado, no hacemos nada.
        if (!combatEnabled)
            return;

        if (!value.isPressed)
            return;

        // No atacar mientras hace dash.
        if (
            movement != null &&
            movement.IsDashing
        )
            return;

        if (CurrentWeapon == null)
            return;

        CurrentWeapon.Attack(
            aimDirection
        );
    }

    // ==========================
    // RELOAD
    // ==========================

    public void OnReload(InputValue value)
    {
        if (!combatEnabled)
            return;

        if (!value.isPressed)
            return;

        if (CurrentWeapon == null)
            return;

        CurrentWeapon.Reload();
    }

    // ==========================
    // SLOTS
    // ==========================

    public void OnSlot1(InputValue value)
    {
        if (!combatEnabled)
            return;

        if (value.isPressed)
            SelectSlot(0);
    }

    public void OnSlot2(InputValue value)
    {
        if (!combatEnabled)
            return;

        if (value.isPressed)
            SelectSlot(1);
    }

    public void OnSlot3(InputValue value)
    {
        if (!combatEnabled)
            return;

        if (value.isPressed)
            SelectSlot(2);
    }

    public void OnPreviousWeapon(InputValue value)
    {
        if (!combatEnabled)
            return;

        if (!value.isPressed)
            return;

        SwitchToPreviousWeapon();
    }

    // ==========================
    // WEAPONS
    // ==========================

    private void InitializeWeapons()
    {
        if (
            weapons == null ||
            weapons.Length == 0
        )
            return;

        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                weapons[i].SetEquipped(false);
            }
        }

        if (weapons[0] != null)
        {
            currentSlot = 0;
            weapons[0].SetEquipped(true);
        }
    }

    private void SelectSlot(int newSlot)
    {
        if (weapons == null)
            return;

        if (
            newSlot < 0 ||
            newSlot >= weapons.Length
        )
            return;

        if (weapons[newSlot] == null)
            return;

        if (newSlot == currentSlot)
            return;

        CurrentWeapon?.SetEquipped(false);

        previousSlot = currentSlot;
        currentSlot = newSlot;

        CurrentWeapon.SetEquipped(true);

        CurrentWeapon.SetAimDirection(
            aimDirection
        );
    }

    private void SwitchToPreviousWeapon()
    {
        if (previousSlot < 0)
            return;

        if (previousSlot >= weapons.Length)
            return;

        if (weapons[previousSlot] == null)
            return;

        CurrentWeapon?.SetEquipped(false);

        int temp = currentSlot;

        currentSlot = previousSlot;
        previousSlot = temp;

        CurrentWeapon.SetEquipped(true);

        CurrentWeapon.SetAimDirection(
            aimDirection
        );
    }
}