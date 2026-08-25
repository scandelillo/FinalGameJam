using UnityEngine;
using UnityEngine.InputSystem;

public class DebugPoints : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private int pointsToAdd = 10000;

    public void OnDebugPoints(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (ScoreManager.Instance == null)
        {
            Debug.LogWarning("No existe ScoreManager.");
            return;
        }

        ScoreManager.Instance.AddPoints(pointsToAdd);

        Debug.Log(
            $"DEBUG: +{pointsToAdd} puntos"
        );
    }
}