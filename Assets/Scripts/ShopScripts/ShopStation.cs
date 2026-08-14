using UnityEngine;

public class ShopStation : MonoBehaviour
{
    [Header("Shop")]
    [SerializeField] private ShopUI shopUI;

    private bool playerInRange;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;

        PlayerInteraction.OnInteractPressed += HandleInteract;

        Debug.Log("Player entró al rango de la tienda");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;

        PlayerInteraction.OnInteractPressed -= HandleInteract;

        Debug.Log("Player salió del rango de la tienda");

        if (shopUI != null && shopUI.IsOpen)
        {
            shopUI.Close();
        }
    }

    private void OnDisable()
    {
        PlayerInteraction.OnInteractPressed -= HandleInteract;
    }

    private void HandleInteract()
    {
        if (!playerInRange)
            return;

        if (shopUI == null)
            return;

        if (shopUI.IsOpen)
        {
            shopUI.Close();
        }
        else
        {
            shopUI.Open();
        }
    }
}