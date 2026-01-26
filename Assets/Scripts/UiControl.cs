using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class UiControl : MonoBehaviour
{
    public void Playbutton()
    {
        SceneManager.LoadScene("SampleScene");
        GameManager.Instance.lifeReset();
    }
    public void CreditsButton()
    {
        SceneManager.LoadScene("Credits");
    }
    public void ExitButton()
    {         
        Debug.Log("Saliendo del juego...");
        EditorApplication.isPlaying = false; // Detiene el modo de juego en el editor
        Application.Quit();
    }
    public void MenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
