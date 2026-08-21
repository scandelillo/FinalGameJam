using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject shopPanel;

    [Header("Player")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private Rigidbody2D playerRigidbody;

    [Header("Shop Behaviour")]
    [SerializeField] private bool pauseGameWhileOpen = true;

    private float previousTimeScale = 1f;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        // La tienda empieza cerrada.
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }

        IsOpen = false;
    }

    public void Open()
    {
        if (IsOpen)
            return;

        IsOpen = true;

        // Mostrar menú.
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
        }

        // Detener inmediatamente al Player.
        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector2.zero;
        }

        // Bloquear movimiento.
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Bloquear ataques.
        if (playerCombat != null)
        {
            playerCombat.SetCombatEnabled(false);
        }

        // Opcionalmente pausar todo el mundo.
        if (pauseGameWhileOpen)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        Debug.Log("Menu Abierto");
    }

    public void Close()
    {
        if (!IsOpen)
            return;

        IsOpen = false;

        // Ocultar menú.
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }

        // Devolver movimiento.
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        // Devolver combate.
        if (playerCombat != null)
        {
            playerCombat.SetCombatEnabled(true);
        }

        // Reanudar el juego.
        if (pauseGameWhileOpen)
        {
            Time.timeScale = previousTimeScale;
        }

        Debug.Log("Menu Cerrado");
    }

    public void CloseButton()
    {
        Close();
    }
}