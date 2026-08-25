using UnityEngine;

/// <summary>
/// Puerta desbloqueable. Necesita 2 colliders 2D en el mismo GameObject:
/// - uno IsTrigger=true grande, para detectar "jugador cerca" (rango de interacción)
/// - uno IsTrigger=false que bloquea físicamente el paso, se desactiva al abrir
/// El jugador debe tener el tag "Player" para que la detección funcione.
/// No lee input directamente: escucha PlayerInteraction.OnInteractPressed
/// y solo actúa si el jugador está en su rango.
/// </summary>
public class DoorUnlockable : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Deja Wave Spawner vacío si esta puerta no debe activar ninguna zona de spawn (ej. solo abre un pasillo)")]
    [SerializeField] private ZoneId zoneToUnlock;
    [SerializeField] private int pointsCost = 500;
    [SerializeField] private WaveSpawner waveSpawner;

    [Header("Referencias físicas")]
    [Tooltip("Collider2D (no trigger) que bloquea el paso; se desactiva al abrir")]
    [SerializeField] private Collider2D physicalBlocker;

    [Header("Visual")]
    [Tooltip("El sprite de la puerta cerrada, se oculta al abrir (déjalo vacío si esta puerta usa Open Move Offset en vez de ocultarse)")]
    [SerializeField] private SpriteRenderer doorSprite;
    [Tooltip("Si es distinto de (0,0), la puerta se desplaza este offset al abrirse en vez de (o además de) ocultar el sprite. Ej: (-1.5, 0) la mueve a la izquierda")]
    [SerializeField] private Vector2 openMoveOffset = Vector2.zero;

    [Header("Feedback (opcional)")]
    [SerializeField] private Animator doorAnimator;
    [Tooltip("Ej. un texto 'Mantén E (500 pts)' que se muestra solo si el jugador está cerca")]
    [SerializeField] private GameObject promptUI;

    private bool playerInRange;
    private bool isOpen;

    private void OnEnable()
    {
        PlayerInteraction.OnInteractPressed += HandleInteractPressed;
    }

    private void OnDisable()
    {
        PlayerInteraction.OnInteractPressed -= HandleInteractPressed;
    }

    private void HandleInteractPressed()
    {
        if (isOpen || !playerInRange) return;

        TryOpen();
    }

    private void TryOpen()
    {
        if (!ScoreManager.Instance.TrySpendPoints(pointsCost))
        {
            // Aquí puedes añadir feedback de "no te alcanza": sonido, shake del UI, etc.
            return;
        }

        isOpen = true;
        physicalBlocker.enabled = false;

        // Opcional: algunas puertas (ej. solo un pasillo) no controlan
        // ninguna zona de spawn, así que no tienen WaveSpawner asignado.
        if (waveSpawner != null)
            waveSpawner.UnlockZone(zoneToUnlock);

        if (doorSprite != null) doorSprite.enabled = false;
        if (openMoveOffset != Vector2.zero) transform.position += (Vector3)openMoveOffset;
        if (doorAnimator != null) doorAnimator.SetTrigger("Open");
        if (promptUI != null) promptUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        if (!isOpen && promptUI != null) promptUI.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        if (promptUI != null) promptUI.SetActive(false);
    }
}