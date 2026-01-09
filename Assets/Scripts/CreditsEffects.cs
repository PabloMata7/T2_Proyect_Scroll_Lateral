using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsEffects : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    public void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }
    private void Start()
    {
        Aparecer();
    }
    public void Aparecer()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float duracion = 7f;
        float tiempo = 0;
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            canvasGroup.alpha = tiempo / duracion;
            yield return null;
        }
        canvasGroup.alpha = 1;
    }
}