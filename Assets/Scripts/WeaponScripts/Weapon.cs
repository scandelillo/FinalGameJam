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

    // Se actualiza constantemente con la dirección del mouse
    public virtual void SetAimDirection(Vector2 direction)
    {
    }

    public abstract void Attack(Vector2 direction);

    public virtual void Reload()
    {
        // Melee no hace nada
    }
}