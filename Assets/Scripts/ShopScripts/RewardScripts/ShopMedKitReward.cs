using UnityEngine;
using UnityEngine.UI;

public class ShopMedkitReward : MonoBehaviour
{
    [Header("Medkit")]
    [SerializeField] private float healAmount = 50f;

    [Header("Player")]
    [SerializeField] private PlayerHealth playerHealth;

    [Header("UI")]
    [SerializeField] private Button purchaseButton;

    private void Start()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += HandleHealthChanged;
        }

        UpdateButtonState();
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= HandleHealthChanged;
        }
    }

    public void HealPlayer()
    {
        if (playerHealth == null)
        {
            Debug.LogWarning(
                "PlayerHealth no está asignado."
            );

            return;
        }

        playerHealth.Heal(healAmount);

        Debug.Log(
            $"Botiquín usado: +{healAmount} vida"
        );

        UpdateButtonState();
    }

    private void HandleHealthChanged(
        float currentHealth,
        float maxHealth)
    {
        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        if (purchaseButton == null)
            return;

        if (playerHealth == null)
            return;

        purchaseButton.interactable =
            !playerHealth.IsDead &&
            playerHealth.CurrentHealth <
            playerHealth.MaxHealth;
    }
}