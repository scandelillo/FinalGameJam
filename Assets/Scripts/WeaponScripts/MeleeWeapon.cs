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

    private float cooldownRemaining;

    // Ahora esta dirección se actualiza todo el tiempo
    private Vector2 currentAimDirection =
        Vector2.right;

    private readonly HashSet<IDamageable>
        hitTargets =
            new HashSet<IDamageable>();

    private void Update()
    {
        if (cooldownRemaining > 0f)
        {
            cooldownRemaining -=
                Time.deltaTime;
        }
    }

    // ==========================
    // AIM
    // ==========================

    public override void SetAimDirection(
        Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.001f)
            return;

        currentAimDirection =
            direction.normalized;
    }

    // ==========================
    // ATTACK
    // ==========================

    public override void Attack(
        Vector2 direction)
    {
        if (cooldownRemaining > 0f)
            return;

        if (direction.sqrMagnitude < 0.001f)
            return;

        direction.Normalize();

        cooldownRemaining =
            attackCooldown;

        PerformMeleeAttack(direction);
    }

    private void PerformMeleeAttack(
        Vector2 direction)
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

        hitTargets.Clear();

        foreach (
            Collider2D targetCollider
            in colliders
        )
        {
            Vector2 targetPosition =
                targetCollider.bounds.center;

            Vector2 directionToTarget =
                targetPosition - origin;

            if (
                directionToTarget.sqrMagnitude
                <= 0.001f
            )
                continue;

            float angle =
                Vector2.Angle(
                    direction,
                    directionToTarget.normalized
                );

            
            if (
                angle >
                attackAngle / 2f
            )
                continue;

            // Buscamos cualquier IDamageable.
            IDamageable damageable =
                targetCollider
                    .GetComponentInParent
                    <IDamageable>();

            if (damageable == null)
                continue;

            // Evitamos daño doble si el mismo
            // objetivo tiene varios colliders.
            if (
                hitTargets.Contains(
                    damageable
                )
            )
                continue;

            hitTargets.Add(
                damageable
            );

            damageable.TakeDamage(
                damage
            );
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

        // usamos currentAimDirection,
        // que se actualiza todos los frames.
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
            origin +
            (Vector3)(left * range)
        );

        Gizmos.DrawLine(
            origin,
            origin +
            (Vector3)(right * range)
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
            vector.x * cos
                - vector.y * sin,

            vector.x * sin
                + vector.y * cos
        );
    }
}