using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] private MeleeWeapon meleeWeapon;

    public void PerformMeleeAttack()
    {
        if (meleeWeapon != null)
        {
            meleeWeapon.PerformMeleeAttack();
        }
    }
}