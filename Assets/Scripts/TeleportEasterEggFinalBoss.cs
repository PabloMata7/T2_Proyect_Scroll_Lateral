using UnityEngine;

public class TeleportOnKeyUI : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "FinalBoss";
    [SerializeField] private KeyCode keyToPress = KeyCode.T;
    [SerializeField] private GameObject uiMessage;

    private bool playerInside = false;

    private void Start()
    {
        if (uiMessage != null)
            uiMessage.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            if (uiMessage != null)
                uiMessage.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            if (uiMessage != null)
                uiMessage.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(keyToPress))
        {
            ScreenFader.Instance.LoadSceneWithFade(sceneToLoad);
        }
    }
}