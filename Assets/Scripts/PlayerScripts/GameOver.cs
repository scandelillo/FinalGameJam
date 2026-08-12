using UnityEngine;

public class GameOver : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;

    private void Awake()
    {
        // Por seguridad, cada vez que inicia la escena
        // el juego empieza sin pausa
        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDied += HandlePlayerDeath;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDied -= HandlePlayerDeath;
        }
    }

    private void HandlePlayerDeath()
    {
        Debug.Log("GAME OVER");

        Time.timeScale = 0f;
    }
}