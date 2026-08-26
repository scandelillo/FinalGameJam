using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private Firearm weapon; 
    [SerializeField] private TextMeshProUGUI textoBalas;

    void Update()
    {
        if (weapon == null || textoBalas == null)
            return;

        textoBalas.text = $"{weapon.CurrentAmmo} / {weapon.ReserveAmmo}";
    }
}
