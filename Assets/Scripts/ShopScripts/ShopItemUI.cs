using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] private string itemName = "Producto";
    [SerializeField] private int price = 100;

    [Header("UI")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;

    [Header("Purchase")]
    [SerializeField] private UnityEvent onPurchased;

    private void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (nameText != null)
        {
            nameText.text = itemName;
        }

        if (priceText != null)
        {
            priceText.text = price.ToString();
        }
    }

    public void Buy()
    {
        if (ScoreManager.Instance == null)
        {
            Debug.LogWarning("No existe ScoreManager.");
            return;
        }

        bool purchased =
            ScoreManager.Instance.TrySpendPoints(price);

        if (!purchased)
        {
            Debug.Log("No tienes suficientes puntos.");
            return;
        }

        Debug.Log($"Compraste: {itemName}");

        onPurchased?.Invoke();
    }
}