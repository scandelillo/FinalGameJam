using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [Header("Melee Stats")]
    [SerializeField]
    private float damage = 30f;

    [SerializeField]
    private float range = 2f;

    [Tooltip("Ángulo TOTAL del abanico")]
    [SerializeField]
    private float attackAngle = 90f;

    [SerializeField]
    private float attackCooldown = 0.45f;

    [Header("Collision")]
    [SerializeField]
    private LayerMask zombieLayers;

    [Header("Origin")]
    [SerializeField]
    private Transform attackOrigin;

    [Header("Animation")]
    [SerializeField]
    private Animator animator;

    public override bool UsesMeleeWalkAnimation => true;

    private float cooldownRemaining;

    // Dirección que se utilizará cuando llegue el momento
    // del impacto en la animación.
    private Vector2 attackDirection = Vector2.right;

    // Dirección utilizada para mostrar los Gizmos.
    private Vector2 currentAimDirection = Vector2.right;

    // Evita que un mismo zombie reciba daño dos veces
    // si tiene varios colliders.
    private readonly HashSet<IDamageable> hitTargets =
        new HashSet<IDamageable>();

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInParent<Animator>();
    }

    private void Update()
    {
        if (cooldownRemaining > 0f)
        {
            cooldownRemaining -= Time.deltaTime;
        }
    }

    // ==========================
    // AIM
    // ==========================

    public override void SetAimDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.001f)
            return;

        currentAimDirection = direction.normalized;
    }

    // ==========================
    // ATTACK
    // ==========================

    public override void Attack(Vector2 direction)
    {
        if (cooldownRemaining > 0f)
            return;

        if (direction.sqrMagnitude < 0.001f)
            return;

        // Guardamos la dirección del ataque.
        // El daño se aplicará después mediante
        // el Animation Event.
        attackDirection = direction.normalized;

        currentAimDirection = attackDirection;

        cooldownRemaining = attackCooldown;

        // Solo iniciamos la animación.
        // El daño NO se aplica aquí.
        if (animator != null)
            animator.SetTrigger("MeleeAttack");
    }

    // ==========================
    // ANIMATION EVENT
    // ==========================

    // Este método será llamado por el Animation Event
    // exactamente en el frame donde ocurre el impacto.
    public void PerformMeleeAttack()
    {
        PerformMeleeAttack(attackDirection);
    }

    // ==========================
    // MELEE DAMAGE
    // ==========================

    private void PerformMeleeAttack(Vector2 direction)
    {
        Vector2 origin =
            attackOrigin != null
                ? attackOrigin.position
                : transform.position;

        Collider2D[] colliders =
            Physics2D.OverlapCircleAll(
                origin,
                range,
                zombieLayers
            );

        // Limpiamos los objetivos golpeados
        // para este nuevo ataque.
        hitTargets.Clear();

        foreach (Collider2D targetCollider in colliders)
        {
            Vector2 targetPosition =
                targetCollider.bounds.center;

            Vector2 directionToTarget =
                targetPosition - origin;

            if (directionToTarget.sqrMagnitude <= 0.001f)
                continue;

            // Calculamos el ángulo entre la dirección
            // del ataque y el objetivo.
            float angle =
                Vector2.Angle(
                    direction,
                    directionToTarget.normalized
                );

            // Si está fuera del abanico del ataque,
            // no recibe daño.
            if (angle > attackAngle / 2f)
                continue;

            // Buscamos el IDamageable en el objeto
            // o en alguno de sus padres.
            IDamageable damageable =
                targetCollider.GetComponentInParent<IDamageable>();

            if (damageable == null)
                continue;

            // Evitamos daño doble si el zombie
            // tiene varios colliders.
            if (hitTargets.Contains(damageable))
                continue;

            hitTargets.Add(damageable);

            // AQUÍ ocurre finalmente el daño.
            damageable.TakeDamage(damage);
        }
    }

    // ==========================
    // GIZMOS
    // ==========================

    private void OnDrawGizmosSelected()
    {
        Vector3 origin =
            attackOrigin != null
                ? attackOrigin.position
                : transform.position;

        Gizmos.DrawWireSphere(
            origin,
            range
        );

        Vector2 left =
            RotateVector(
                currentAimDirection,
                -attackAngle / 2f
            );

        Vector2 right =
            RotateVector(
                currentAimDirection,
                attackAngle / 2f
            );

        Gizmos.DrawLine(
            origin,
            origin + (Vector3)(left * range)
        );

        Gizmos.DrawLine(
            origin,
            origin + (Vector3)(right * range)
        );
    }

    private Vector2 RotateVector(
        Vector2 vector,
        float degrees)
    {
        float radians =
            degrees * Mathf.Deg2Rad;

        float cos =
            Mathf.Cos(radians);

        float sin =
            Mathf.Sin(radians);

        return new Vector2(
            vector.x * cos - vector.y * sin,
            vector.x * sin + vector.y * cos
        );
    }
}