/// <summary>
/// Cualquier arma que use munición la implementa (ej. Firearm). AmmoPickup
/// busca este componente para rellenar reserva sin saber qué arma es.
/// </summary>
public interface IAmmoContainer
{
    void AddReserveAmmo(int amount);
}
