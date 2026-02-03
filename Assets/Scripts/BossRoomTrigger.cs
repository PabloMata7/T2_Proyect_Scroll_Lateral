using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomTrigger : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "FinalBoss";

    private bool _hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_hasTriggered)
        {
            _hasTriggered = true;
            Debug.Log("Iniciando viaje a: " + sceneToLoad);

            // Llamamos a la nueva función de carga
            ScreenFader.Instance.LoadSceneWithFade(sceneToLoad);
        }
    }
}
