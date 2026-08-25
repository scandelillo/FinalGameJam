using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private Camera combatCamera;
    [SerializeField] private Animator animator;

    [Header("Weapons")]
    [SerializeField] private Weapon[] weapons = new Weapon[3];

    private PlayerInput playerInput;

    private int currentSlot = 0;
    private int previousSlot = -1;

    private Vector2 mouseScreenPosition;
    private Vector2 aimDirection = Vector2.down;

    // Permite bloquear todos los inputs
    // mientras está abierta la tienda.
    private bool combatEnabled = true;

    public Vector2 AimDirection =>
        aimDirection;

    public int CurrentSlot =>
        currentSlot;

    public Weapon CurrentWeapon
    {
        get
        {
            if (weapons == null)
                return null;

            if (
                currentSlot < 0 ||
                currentSlot >= weapons.Length
            )
                return null;

            return weapons[currentSlot];
        }
    }


    private void Awake()
    {
        if (movement == null)
        {
            movement =
                GetComponent<PlayerMovement>();
        }

        if (combatCamera == null)
        {
            combatCamera =
                Camera.main;
        }

        if (animator == null)
        {
            animator =
                GetComponent<Animator>();
        }

        playerInput =
            GetComponent<PlayerInput>();
    }

    private void Start()
    {
        InitializeWeapons();
    }

    private void Update()
    {
        UpdateAimDirection();

        if (movement != null)
        {
            movement.SetAimFacing(
                aimDirection
            );
        }

        UpdateWeaponAim();

        HandleAutomaticFire();
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
        mouseScreenPosition =
            value.Get<Vector2>();
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
            aimDirection =
                direction.normalized;
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
        if (!combatEnabled)
            return;

        if (!value.isPressed)
            return;

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
    // AUTOMATIC FIRE
    // ==========================

    private void HandleAutomaticFire()
    {
        if (!combatEnabled)
            return;

        if (
            movement != null &&
            movement.IsDashing
        )
            return;

        if (playerInput == null)
            return;

        if (CurrentWeapon == null)
            return;

        Firearm firearm =
            CurrentWeapon as Firearm;

        if (firearm == null)
            return;

        if (!firearm.IsAutomaticFireEnabled)
            return;

        InputAction attackAction =
            playerInput.actions["Attack"];

        if (attackAction == null)
            return;

        if (!attackAction.IsPressed())
            return;

        firearm.Attack(
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
        {
            SelectSlot(0);
        }
    }

    public void OnSlot2(InputValue value)
    {
        if (!combatEnabled)
            return;

        if (value.isPressed)
        {
            SelectSlot(1);
        }
    }

    public void OnSlot3(InputValue value)
    {
        if (!combatEnabled)
            return;

        if (value.isPressed)
        {
            SelectSlot(2);
        }
    }

    public void OnPreviousWeapon(
        InputValue value)
    {
        if (!combatEnabled)
            return;

        if (!value.isPressed)
            return;

        SwitchToPreviousWeapon();
    }

    // ==========================
    // ANIMATION
    // ==========================

    private void UpdateWeaponAnimation()
    {
        if (animator == null)
            return;

        bool hasMelee =
            CurrentWeapon != null &&
            CurrentWeapon.UsesMeleeWalkAnimation;

        animator.SetBool(
            "HasMeleeWeapon",
            hasMelee
        );
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

        for (
            int i = 0;
            i < weapons.Length;
            i++
        )
        {
            if (weapons[i] != null)
            {
                weapons[i].SetEquipped(
                    false
                );
            }
        }

        if (weapons[0] != null)
        {
            currentSlot = 0;

            weapons[0].SetEquipped(
                true
            );
        }

        UpdateWeaponAnimation();
    }

    private void SelectSlot(
        int newSlot)
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

        CurrentWeapon?.SetEquipped(
            false
        );

        previousSlot = currentSlot;

        currentSlot = newSlot;

        CurrentWeapon.SetEquipped(
            true
        );

        CurrentWeapon.SetAimDirection(
            aimDirection
        );

        UpdateWeaponAnimation();
    }

    private void SwitchToPreviousWeapon()
    {
        if (previousSlot < 0)
            return;

        if (
            previousSlot >= weapons.Length
        )
            return;

        if (
            weapons[previousSlot] == null
        )
            return;

        CurrentWeapon?.SetEquipped(
            false
        );

        int temp = currentSlot;

        currentSlot =
            previousSlot;

        previousSlot =
            temp;

        CurrentWeapon.SetEquipped(
            true
        );

        CurrentWeapon.SetAimDirection(
            aimDirection
        );

        UpdateWeaponAnimation();
    }
}