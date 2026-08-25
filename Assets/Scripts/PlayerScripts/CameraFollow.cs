using UnityEngine;

/// <summary>
/// Sigue al jugador con suavizado.
/// También aplica un pequeño shake cuando recibe daño.
/// Va en la cámara principal.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private Vector3 offset =
        new Vector3(0f, 0f, -10f);

    [Header("Damage Shake")]
    [SerializeField] private float shakeDuration = 0.12f;
    [SerializeField] private float shakeIntensity = 0.08f;

    private Vector3 velocity;

    private float shakeTimeRemaining;

    private PlayerHealth playerHealth;

    private void Awake()
    {
        if (target == null)
        {
            GameObject playerObj =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }
    }

    private void Start()
    {
        if (target == null)
            return;

        playerHealth =
            target.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.OnPlayerDamaged +=
                HandlePlayerDamaged;
        }
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition =
            target.position + offset;

        Vector3 smoothPosition =
            Vector3.SmoothDamp(
                transform.position,
                desiredPosition,
                ref velocity,
                smoothTime
            );


        if (shakeTimeRemaining > 0f)
        {
            Vector2 randomOffset =
                Random.insideUnitCircle *
                shakeIntensity;

            smoothPosition +=
                new Vector3(
                    randomOffset.x,
                    randomOffset.y,
                    0f
                );

            shakeTimeRemaining -=
                Time.deltaTime;
        }

        transform.position =
            smoothPosition;
    }

    private void HandlePlayerDamaged(
        float damageAmount)
    {
        Shake();
    }

    public void Shake()
    {
        shakeTimeRemaining =
            shakeDuration;
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDamaged -=
                HandlePlayerDamaged;
        }
    }
}