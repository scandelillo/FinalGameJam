using System;
using UnityEngine;

/// <summary>
/// Singleton simple que guarda los puntos del jugador. ZombieController le suma
/// puntos al matar zombies; DoorUnlockable le pregunta si puede gastar puntos
/// para abrir una puerta; más adelante la tienda hará lo mismo.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public event Action<int> OnPointsChanged;

    [SerializeField] private int startingPoints = 0;
    public int CurrentPoints { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        CurrentPoints = startingPoints;
    }

    public void AddPoints(int amount)
    {
        CurrentPoints += amount;
        OnPointsChanged?.Invoke(CurrentPoints);

        Debug.Log($"+{amount} puntos (total: {CurrentPoints})");
    }

    public bool TrySpendPoints(int amount)
    {
        if (CurrentPoints < amount) return false;

        CurrentPoints -= amount;
        OnPointsChanged?.Invoke(CurrentPoints);

        Debug.Log($"-{amount} puntos gastados (total: {CurrentPoints})");
        return true;
    }
}