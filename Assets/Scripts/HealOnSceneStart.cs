using UnityEngine;

public class HealOnSceneStart : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentLives = GameManager.Instance.maxLives;
            GameManager.Instance.UpdateLive();
        }
    }
}