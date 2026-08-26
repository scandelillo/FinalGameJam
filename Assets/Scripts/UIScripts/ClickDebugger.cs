using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// DIAGNÓSTICO TEMPORAL. Agrégalo a cualquier botón/panel para confirmar
/// si el clic llega hasta ahí a nivel de EventSystem, sin pasar por
/// Button ni UIManager. Bórralo cuando termines de debuggear.
/// </summary>
public class ClickDebugger : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerDownHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[CLICK DEBUG] Click detectado en: {gameObject.name}");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"[CLICK DEBUG] PointerDown en: {gameObject.name}");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"[CLICK DEBUG] El mouse ENTRÓ al área de: {gameObject.name}");
    }
}
