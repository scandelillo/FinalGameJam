using UnityEngine;
using UnityEngine.UI;

public class ShopAutoFireReward : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] private Firearm firearm;

    [Header("UI")]
    [SerializeField] private Button purchaseButton;

    private void Start()
    {
        UpdateButtonState();
    }

    public void UnlockAutomaticFire()
    {
        if (firearm == null)
        {
            Debug.LogWarning(
                "Firearm no está asignado."
            );

            return;
        }

        bool unlocked =
            firearm.UnlockAutomaticFire();

        if (!unlocked)
        {
            Debug.Log(
                "Automatic Fire ya estaba desbloqueado."
            );
        }

        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        if (purchaseButton == null)
            return;

        if (firearm == null)
            return;

        purchaseButton.interactable =
            !firearm.IsAutomaticFireEnabled;
    }
}