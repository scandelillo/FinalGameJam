using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3.5f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 9f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.5f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;

    private Vector2 moveInput;
    private Vector2 facingDirection = Vector2.down;

    // Dash
    private bool isDashing;
    private Vector2 dashDirection;
    private float dashTimeRemaining;
    private float dashCooldownRemaining;

    public Vector2 FacingDirection => facingDirection;
    public bool IsDashing => isDashing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isDashing)
        {
            UpdateFacingDirection();
        }

        // No actualizamos Speed durante el dash.
        if (animator != null && !isDashing)
        {
            animator.SetFloat("Speed", moveInput.magnitude);
        }
    }

    private void FixedUpdate()
    {
        UpdateDashCooldown();

        if (isDashing)
        {
            DashMovement();
        }
        else
        {
            Move();
        }
    }

    // ==========================
    // INPUT
    // ==========================

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        // Evita que diagonal sea más rápida.
        moveInput = Vector2.ClampMagnitude(moveInput, 1f);
    }

    public void OnDash(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (isDashing)
            return;

        if (dashCooldownRemaining > 0f)
            return;

        StartDash();
    }

    // ==========================
    // MOVEMENT
    // ==========================

    private void Move()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    // ==========================
    // DASH
    // ==========================

    private void StartDash()
    {
        isDashing = true;

        dashTimeRemaining = dashDuration;
        dashCooldownRemaining = dashCooldown;

        if (moveInput.sqrMagnitude > 0.01f)
        {
            dashDirection = moveInput.normalized;
        }
        else
        {
            dashDirection = facingDirection.normalized;
        }

        // Conservamos la dirección del movimiento para el dash.
        facingDirection = Get8Direction(dashDirection);

        if (animator != null)
        {
            animator.SetBool("IsDashing", true);
        }
    }

    private void DashMovement()
    {
        rb.linearVelocity = dashDirection * dashSpeed;

        dashTimeRemaining -= Time.fixedDeltaTime;

        if (dashTimeRemaining <= 0f)
        {
            EndDash();
        }
    }

    private void EndDash()
    {
        isDashing = false;

        rb.linearVelocity = Vector2.zero;

        if (animator != null)
        {
            animator.SetBool("IsDashing", false);
        }
    }

    private void UpdateDashCooldown()
    {
        if (dashCooldownRemaining > 0f)
        {
            dashCooldownRemaining -= Time.fixedDeltaTime;
        }
    }

    // ==========================
    // FACING DIRECTION
    // ==========================

    private void UpdateFacingDirection()
    {
        if (moveInput.sqrMagnitude < 0.01f)
            return;

        // Esta dirección se conserva para movimiento/dash.
        // Ya no controla visualmente el flip del sprite.
        facingDirection = Get8Direction(moveInput);
    }

    // ==========================
    // MOUSE AIM FLIP
    // ==========================

    public void SetAimFacing(Vector2 aimDirection)
    {
        if (spriteRenderer == null)
            return;

        // Los sprites miran hacia la derecha por defecto.
        if (aimDirection.x > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (aimDirection.x < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
    }

    // ==========================
    // 8 DIRECTIONS
    // ==========================

    private Vector2 Get8Direction(Vector2 direction)
    {
        float angle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg;

        angle = Mathf.Round(angle / 45f) * 45f;

        float radians = angle * Mathf.Deg2Rad;

        return new Vector2(
            Mathf.Cos(radians),
            Mathf.Sin(radians)
        ).normalized;
    }
}