using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTrailEffect : MonoBehaviour
{
    [SerializeField] private float fadeSpeed = 1.0f;

    private SpriteRenderer sr;
    private Color currentColor;

    private float startingAlpha;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        startingAlpha = sr.color.a;
    }

    //private void OnEnable()
    //{
    //    if (sr != null) 
    //    { 
    //        currentColor.a -= fadeSpeed * Time.deltaTime;
    //        currentColor.a = sr.color.a;
    //        sr.color = currentColor;
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        // Obtenemos el color actual
        Color color = sr.color;
        // Restamos alpha
        color.a -= fadeSpeed * Time.deltaTime;
        // Aplicamos
        sr.color = color;

        // Si es invisible, desactivamos el objeto para ahorrar
        if (color.a <= 0f)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetUpGhost(Sprite currentSprite, bool flipX)
    {
        sr.sprite = currentSprite;
        sr.flipX = flipX;

        // --- CORRECCIÓN CRÍTICA ---
        // Forzamos el Alpha original cada vez que se usa el fantasma
        Color c = sr.color;
        c.a = startingAlpha;
        sr.color = c;

        // Aseguramos que el objeto se active
        gameObject.SetActive(true);
    }
}
