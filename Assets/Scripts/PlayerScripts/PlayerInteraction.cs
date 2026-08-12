using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Centraliza la tecla de interacción (E) usando el Input System nuevo,
/// igual que PlayerMovement/PlayerCombat. No sabe nada de puertas: solo
/// avisa "se presionó interactuar" y quien esté escuchando (una puerta
/// en rango, más adelante un NPC de tienda, etc.) reacciona.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    public static event Action OnInteractPressed;

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;

        OnInteractPressed?.Invoke();
    }
}
