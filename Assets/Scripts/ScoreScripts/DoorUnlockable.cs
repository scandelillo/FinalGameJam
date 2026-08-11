using UnityEngine;

/// <summary>
/// Puerta desbloqueable. Necesita 2 colliders 2D en el mismo GameObject:
/// - uno IsTrigger=true grande, para detectar "jugador cerca" (rango de interacción)
/// - uno IsTrigger=false que bloquea físicamente el paso, se desactiva al abrir
/// El jugador debe tener el tag "Player" para que la detección funcione.
/// </summary>
public class DoorUnlockable : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private ZoneId zoneToUnlock;
    [SerializeField] private int pointsCost = 500;
    [SerializeField] private WaveSpawner waveSpawner;

    [Header("Referencias físicas")]
    [Tooltip("Collider2D (no trigger) que bloquea el paso; se desactiva al abrir")]
    [SerializeField] private Collider2D physicalBlocker;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Feedback (opcional)")]
    [SerializeField] private Animator doorAnimator;
    [Tooltip("Ej. un texto 'Mantén E (500 pts)' que se muestra solo si el jugador está cerca")]
    [SerializeField] private GameObject promptUI;

    private bool playerInRange;
    private bool isOpen;

    private void Update()
    {
        if (isOpen || !playerInRange) return;

        if (Input.GetKeyDown(interactKey))
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
        waveSpawner.UnlockZone(zoneToUnlock);

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
