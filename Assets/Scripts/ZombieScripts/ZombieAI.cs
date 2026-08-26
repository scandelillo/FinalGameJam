using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Persigue al jugador usando NavMesh (esquiva paredes/obstáculos entre salas)
/// y lo ataca al estar en rango. Usa Speed/Damage que ZombieController ya
/// calculó según la ronda actual. Requiere NavMeshPlus instalado y un
/// NavMesh horneado en la escena.
/// </summary>
[RequireComponent(typeof(ZombieController))]
[RequireComponent(typeof(NavMeshAgent))]
public class ZombieAI : MonoBehaviour
{
    [Header("Ataque")]
    [SerializeField] private float attackRange = 0.6f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Pathfinding")]
    [Tooltip("Cada cuánto recalcula el camino hacia el jugador, en segundos. Recalcularlo cada frame es innecesariamente caro.")]
    [SerializeField] private float repathInterval = 0.3f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;


    private NavMeshAgent agent;
    private ZombieController zombieController;
    private Transform player;
    private float attackCooldownRemaining;
    private float repathTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        zombieController = GetComponent<ZombieController>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Claves para 2D: el agente no debe rotar en 3D ni intentar
        // alinearse con un eje "up" que no existe en un juego top-down 2D.
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void OnEnable()
    {
        // El objeto se reactiva desde el pool cada vez que spawnea,
        // así que reseteamos su estado acá.
        attackCooldownRemaining = 0f;
        repathTimer = 0f;
        agent.isStopped = false;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (player == null) return;

        agent.speed = zombieController.Speed;

        if (attackCooldownRemaining > 0f)
            attackCooldownRemaining -= Time.deltaTime;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            ChasePlayer();
        }
        else
        {
            StopToAttack();
        }

        UpdateSpriteDirection();
    }

    private void ChasePlayer()
    {
        agent.isStopped = false;

        // No recalculamos el path cada frame: es una operación cara
        // multiplicada por decenas de zombies. Con repathInterval alcanza
        // para que se sienta responsivo sin matar el framerate.
        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            repathTimer = repathInterval;
            agent.SetDestination(player.position);
        }

        if (animator != null)
            animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void StopToAttack()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        if (animator != null)
            animator.SetFloat("Speed", 0f);

        TryAttack();
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

    private void UpdateSpriteDirection()
    {
        if (spriteRenderer == null)
            return;

        if (agent.velocity.x > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (agent.velocity.x < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
    }
}