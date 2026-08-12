using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField]
    protected GameObject weaponVisual;

    public bool IsEquipped { get; private set; }

    public virtual void SetEquipped(bool equipped)
    {
        IsEquipped = equipped;

        if (weaponVisual != null)
        {
            weaponVisual.SetActive(equipped);
        }
    }

    public abstract void Attack(Vector2 direction);

    public virtual void Reload()
    {
        // Las armas melee no recargan.
    }
}