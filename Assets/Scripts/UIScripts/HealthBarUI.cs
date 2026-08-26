using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Slider slider;

    private void OnEnable()
    {
        playerHealth.OnHealthChanged += ActualizarBarra;
    }

    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= ActualizarBarra;
    }

    private void Start()
    {
        
        slider.maxValue = playerHealth.MaxHealth;
        slider.value = playerHealth.CurrentHealth;
    }

    private void ActualizarBarra(float vidaActual, float vidaMaxima)
    {
        slider.maxValue = vidaMaxima;
        slider.value = vidaActual;
    }
}