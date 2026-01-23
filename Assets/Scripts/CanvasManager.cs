using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;

    public List<string> activeScenes;

    private Canvas canvas;

    private void Awake()
    {
        // 1. Singleton Persistente
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // El Canvas entero viaja entre escenas
            canvas = GetComponent<Canvas>();
        }
        else
        {
            Destroy(gameObject); // Si volvemos al menú y ya hay uno, borramos el nuevo
            return;
        }
    }

    private void OnEnable()
    {
        // Nos suscribimos al evento global de Unity "Se ha cargado una escena"
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Esta función se ejecuta sola cada vez que cambias de nivel (o al iniciar)
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckVisibility(scene.name);
    }

    private void CheckVisibility(string sceneName)
    {
        // Si la escena actual está en nuestra lista blanca, activamos el Canvas
        if (activeScenes.Contains(sceneName))
        {
            canvas.enabled = true;
            Debug.Log($"HUD: Activado en {sceneName}");
        }
        else
        {
            // Si es Menú o Créditos, lo desactivamos (pero sigue existiendo en memoria)
            canvas.enabled = false;
            Debug.Log($"HUD: Ocultado en {sceneName}");
        }
    }
}
