using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorialmove : MonoBehaviour
{
    // Start is called before the first frame update
    public string message = "";
    public float displayTime = 2.5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (CanvasManager.Instance != null)
            {
                CanvasManager.Instance.ShowTutorial(message, displayTime);
            }
            Destroy(gameObject);
        }
    }
}
