using UnityEngine;
using UnityEngine.UI;

public class ShopMaxHealthReward : MonoBehaviour
{
    [Header("Upgrade")]
    [SerializeField] private float healthIncrease = 25f;

    [Header("Player")]
    [SerializeField] private PlayerHealth playerHealth;

    [Header("UI")]
    [SerializeField] private Button purchaseButton;

    private void Start()
    {
        UpdateButtonState();
    }

    public void UpgradeMaxHealth()
    {
        if (playerHealth == null)
        {
            Debug.LogWarning(
                "PlayerHealth no está asignado."
            );

            return;
        }

        bool upgraded =
            playerHealth.IncreaseMaxHealth(
                healthIncrease
            );

        if (!upgraded)
        {
            Debug.Log(
                "La vida máxima ya está al máximo."
            );
        }

        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        if (purchaseButton == null)
            return;

        if (playerHealth == null)
            return;

        purchaseButton.interactable =
            !playerHealth.IsMaxHealthUpgraded;
    }
}