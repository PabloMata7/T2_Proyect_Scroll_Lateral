using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private float waitTime = 0.5f;

    private CanvasGroup canvasGroup;
    private bool isFading = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Si ya existe uno (porque volvimos al menú y entramos de nuevo),
            // destruimos el nuevo para quedarnos con el original.
            Destroy(gameObject);
            return;
        }

        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(TransitionSequence(sceneName));
    }

    private IEnumerator TransitionSequence(string sceneName)
    {
        // Accedemos al Singleton del Player para bloquearlo
        if (Player.Instance != null)
        {
            Player.Instance.enabled = false; // Desactiva inputs
            Player.Instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero; // Freno de mano
        }

        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += fadeSpeed * Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Esperamos un momento en negro
        yield return new WaitForSeconds(waitTime);

        // Usamos Async para que el juego no se congele si la escena es pesada
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Esperamos hasta que la carga termine totalmente
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (Player.Instance != null)
        {
            Player.Instance.enabled = true; // Nos aseguramos de que pueda moverse
        }

        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
