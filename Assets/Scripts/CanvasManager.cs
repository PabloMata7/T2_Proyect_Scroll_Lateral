using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;

    public List<string> gameplayScenes;
    public List<string> creditsScenes;

    public GameObject gameplayContainer;
    public GameObject mainMenuContainer;
    public GameObject creditsContainer;

    private void Awake()
    {
        // 1. Singleton Persistente
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // El Canvas entero viaja entre escenas
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
        if (gameplayContainer == null || mainMenuContainer == null)
        {
            Debug.LogWarning("CanvasManager: ¡No has asignado el GameplayContainer!");
            return;
        }

        // Si la escena actual está en nuestra lista blanca, activamos el Canvas
        if (gameplayScenes.Contains(sceneName))
        {
            SetContainers(true, false, false);
            Debug.Log($"HUD: Visible en {sceneName}");
        }
        else if(creditsScenes.Contains(sceneName))
        {
            SetContainers(false, false, true);
            Debug.Log($"Créditos: Visible en {sceneName}");
        }
        else
        {
            SetContainers(false, true, false);
            Debug.Log($"HUD: Oculto en {sceneName}");
        }
    }

    private void SetContainers(bool game, bool menu, bool credits)
    {
        if (gameplayContainer != null) gameplayContainer.SetActive(game);
        if (mainMenuContainer != null) mainMenuContainer.SetActive(menu);
        if (creditsContainer != null) creditsContainer.SetActive(credits);
    }
}
