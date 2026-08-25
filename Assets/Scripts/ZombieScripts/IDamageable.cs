/// <summary>
/// Cualquier objeto que pueda recibir daño la implementa (jugador, barricadas, etc.).
/// ZombieAI busca este componente en el objetivo para atacarlo, sin necesitar
/// saber si es un PlayerHealth, una barricada, u otra cosa.
/// </summary>
public interface IDamageable
{
    void TakeDamage(float amount);
}
