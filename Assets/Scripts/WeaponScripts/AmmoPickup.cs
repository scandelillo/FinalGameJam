using UnityEngine;

/// <summary>
/// Va en el prefab del pickup de munición. Necesita un Collider2D con
/// Is Trigger = true. Al tocarlo el jugador, reparte munición a TODAS
/// las armas de fuego que tenga equipadas (busca IAmmoContainer en sus hijos).
/// </summary>
public class AmmoPickup : MonoBehaviour
{
    private int amount = 10;

    public void SetAmount(int value) => amount = value;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // includeInactive: por si algún arma queda desactivada visualmente
        // mientras no está equipada (SetEquipped(false) suele desactivar el objeto).
        IAmmoContainer[] containers = other.GetComponentsInChildren<IAmmoContainer>(true);

        if (containers.Length == 0) return;

        foreach (var container in containers)
            container.AddReserveAmmo(amount);

        Destroy(gameObject);
    }
}
