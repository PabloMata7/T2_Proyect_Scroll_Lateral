using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiControl : MonoBehaviour
{
    public void Playbutton()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void CreditsButton()
    {
        SceneManager.LoadScene("Credits");
    }
    public void ExitButton()
    {         
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
    public void MenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
