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
    }

    private void Update()
    {
        if (!isDashing)
        {
            UpdateFacingDirection();
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


    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        // Evita que diagonal sea más rápida
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


    private void Move()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }


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

    // Congela visualmente la dirección del personaje
    // en la dirección en la que comenzó el dash.
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
            animator.SetBool("IsDashing", false);
    }

    private void UpdateDashCooldown()
    {
        if (dashCooldownRemaining > 0f)
        {
            dashCooldownRemaining -= Time.fixedDeltaTime;
        }
    }

    private void UpdateFacingDirection()
    {
        if (moveInput.sqrMagnitude < 0.01f)
            return;

        facingDirection = Get8Direction(moveInput);
    }

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