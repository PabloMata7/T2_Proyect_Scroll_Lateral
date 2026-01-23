using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Image[] helmets;

    // Start is called before the first frame update
    void Start()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.OnLivesChanged += UpdateHelmets;
        }
    }

    private void OnDestroy() // Usamos OnDestroy para desuscribirnos si el objeto UI se borra
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLivesChanged -= UpdateHelmets;
        }
    }

    private void UpdateHelmets(int currentLives)
    {
        for (int i = 0; i < helmets.Length; i++)
        {
            // Obtenemos el color actual de la imagen
            Color heartColor = helmets[i].color;

            if (i < currentLives)
            {
                // Vida llena: Alpha a 1 (Totalmente visible)
                heartColor.a = 1f;
            }
            else
            {
                // Vida perdida: Alpha a 0 (Totalmente transparente)
                // El objeto sigue ahí ocupando sitio, pero no se ve.
                heartColor.a = 0f;
            }

            // Aplicamos el color modificado
            helmets[i].color = heartColor;
        }
    }
}
