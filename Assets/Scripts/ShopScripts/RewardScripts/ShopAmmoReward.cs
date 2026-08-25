using UnityEngine;

public class ShopAmmoReward : MonoBehaviour
{
    [Header("Ammo")]
    [SerializeField] private int ammoAmount = 10;

    [Header("Player")]
    [SerializeField] private GameObject player;

    public void AddAmmo()
    {
        if (player == null)
        {
            Debug.LogWarning("Player no está asignado en ShopAmmoReward.");
            return;
        }

        IAmmoContainer[] containers =
            player.GetComponentsInChildren<IAmmoContainer>(true);

        if (containers.Length == 0)
        {
            Debug.LogWarning(
                "No se encontraron armas con IAmmoContainer."
            );

            return;
        }

        foreach (IAmmoContainer container in containers)
        {
            container.AddReserveAmmo(ammoAmount);
        }

        Debug.Log(
            $"Compra de munición: +{ammoAmount}"
        );
    }
}