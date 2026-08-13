using UnityEngine;

/// <summary>
/// Persigue al jugador y lo ataca al estar en rango. Usa Speed/Damage que
/// ZombieController ya calculó según la ronda actual (Initialize los setea).
/// Va en el mismo prefab que ZombieController.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ZombieController))]
public class ZombieAI : MonoBehaviour
{
    [Header("Ataque")]
    [SerializeField] private float attackRange = 0.6f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private ZombieController zombieController;
    private Transform player;
    private float attackCooldownRemaining;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        zombieController = GetComponent<ZombieController>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // El objeto se reactiva desde el pool cada vez que spawnea,
        // así que reseteamos su estado de ataque acá.
        attackCooldownRemaining = 0f;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        if (attackCooldownRemaining > 0f)
            attackCooldownRemaining -= Time.fixedDeltaTime;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            ChasePlayer();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;

            if (animator != null)
                animator.SetFloat("Speed", 0f);

            TryAttack();
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * zombieController.Speed;

        if (animator != null)
            animator.SetFloat("Speed", zombieController.Speed);
    }

    private void TryAttack()
    {
        if (attackCooldownRemaining > 0f) return;

        attackCooldownRemaining = attackCooldown;

        if (animator != null)
            animator.SetTrigger("Attack");

        if (player.TryGetComponent(out IDamageable damageable))
            damageable.TakeDamage(zombieController.Damage);
    }
}