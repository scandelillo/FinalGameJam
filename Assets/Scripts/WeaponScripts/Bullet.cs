using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;

    private ObjectPool pool;

    private float damage;

    private float lifeTimeRemaining;

    private LayerMask hitLayers;

    private bool isActive;

    private void Awake()
    {
        rb =
            GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!isActive)
            return;

        lifeTimeRemaining -=
            Time.deltaTime;

        if (lifeTimeRemaining <= 0f)
        {
            ReturnToPool();
        }
    }

    public void Launch(
        Vector2 direction,
        float speed,
        float bulletDamage,
        float lifeTime,
        LayerMask bulletHitLayers,
        ObjectPool sourcePool)
    {
        pool =
            sourcePool;

        damage =
            bulletDamage;

        hitLayers =
            bulletHitLayers;

        lifeTimeRemaining =
            lifeTime;

        isActive =
            true;

        direction.Normalize();

        rb.linearVelocity =
            direction * speed;
    }

    private void OnTriggerEnter2D(
        Collider2D other)
    {
        if (!isActive)
            return;

        int otherLayer =
            1 << other.gameObject.layer;

        if (
            (hitLayers.value & otherLayer)
            == 0
        )
            return;

        ZombieController zombie =
            other.GetComponentInParent
            <ZombieController>();

        if (zombie != null)
        {
            zombie.TakeDamage(
                damage
            );
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (!isActive)
            return;

        isActive = false;

        rb.linearVelocity =
            Vector2.zero;

        rb.angularVelocity =
            0f;

        if (pool != null)
        {
            pool.Release(
                gameObject
            );
        }
        else
        {
            gameObject.SetActive(
                false
            );
        }
    }
}