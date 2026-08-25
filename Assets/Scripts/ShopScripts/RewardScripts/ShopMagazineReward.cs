using UnityEngine;
using UnityEngine.UI;

public class ShopMagazineReward : MonoBehaviour
{
    [Header("Magazine Upgrade")]
    [SerializeField] private int magazineIncrease = 6;

    [Header("Weapon")]
    [SerializeField] private Firearm firearm;

    [Header("UI")]
    [SerializeField] private Button purchaseButton;

    private void Start()
    {
        UpdateButtonState();
    }

    public void UpgradeMagazine()
    {
        if (firearm == null)
        {
            Debug.LogWarning("Firearm no está asignado.");
            return;
        }

        bool upgraded =
            firearm.IncreaseMagazineSize(
                magazineIncrease
            );

        if (!upgraded)
        {
            Debug.Log(
                "El cargador ya está al máximo."
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
            !firearm.IsMagazineMaxed;
    }
}