using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3.5f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 9f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.5f;

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
    }

    private void Update()
    {
        UpdateFacingDirection();
    }

    private void FixedUpdate()
    {
        // Cooldown del dash
        if (dashCooldownRemaining > 0f)
        {
            dashCooldownRemaining -= Time.fixedDeltaTime;
        }

        if (isDashing)
        {
            DashMovement();
        }
        else
        {
            Move();
        }
    }

    // Movimiento
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        // Evita que la diagonal sea más rápida
        moveInput = Vector2.ClampMagnitude(moveInput, 1f);
    }

    // Espacio / Dash
    public void OnDash(InputValue value)
    {
        // Solo cuando se presiona el botón
        if (!value.isPressed)
            return;

        if (isDashing)
            return;

        if (dashCooldownRemaining > 0f)
            return;

        StartDash();
    }

    private void Move()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    private void StartDash()
    {
        isDashing = true;

        dashTimeRemaining = dashDuration;
        dashCooldownRemaining = dashCooldown;

        // Si el jugador está moviéndose,
        // hacemos dash hacia esa dirección.
        if (moveInput.sqrMagnitude > 0.01f)
        {
            dashDirection = moveInput.normalized;
        }
        else
        {
            // Si está quieto, usa la última dirección
            // hacia donde estaba mirando.
            dashDirection = facingDirection.normalized;
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

        // Lo detenemos al terminar.
        // En el siguiente FixedUpdate volverá a
        // tomar la velocidad normal del movimiento.
        rb.linearVelocity = Vector2.zero;
    }

    private void UpdateFacingDirection()
    {
        if (moveInput.sqrMagnitude < 0.01f)
            return;

        facingDirection = Get8Direction(moveInput);
    }

    private Vector2 Get8Direction(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x)
                      * Mathf.Rad2Deg;

        angle = Mathf.Round(angle / 45f) * 45f;

        float radians = angle * Mathf.Deg2Rad;

        return new Vector2(
            Mathf.Cos(radians),
            Mathf.Sin(radians)
        ).normalized;
    }
}