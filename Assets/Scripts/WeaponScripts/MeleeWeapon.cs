using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [Header("Melee Stats")]
    [SerializeField]
    private float damage = 30f;

    [SerializeField]
    private float range = 1.2f;

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

    private Vector2 lastAttackDirection =
        Vector2.right;

    private readonly HashSet<ZombieController>
        hitZombies =
            new HashSet<ZombieController>();

    private void Update()
    {
        if (cooldownRemaining > 0f)
        {
            cooldownRemaining -=
                Time.deltaTime;
        }
    }

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

        lastAttackDirection =
            direction;

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

        hitZombies.Clear();

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

            // Está fuera de rango
            if (
                angle >
                attackAngle / 2f
            )
                continue;

            ZombieController zombie =
                targetCollider
                    .GetComponentInParent
                    <ZombieController>();

            if (zombie == null)
                continue;

            
            if (hitZombies.Contains(zombie))
                continue;

            hitZombies.Add(zombie);

            zombie.TakeDamage(damage);
        }
    }
  
     // Gizzmos  

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
                lastAttackDirection,
                -attackAngle / 2f
            );

        Vector2 right =
            RotateVector(
                lastAttackDirection,
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
            vector.x * cos
                - vector.y * sin,

            vector.x * sin
                + vector.y * cos
        );
    }
}