using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomTrigger : MonoBehaviour
{
    [SerializeField] private string sceneIfAlive = "FinalBoss";   // o "Final Boss" si tu escena se llama así
    [SerializeField] private string sceneIfDead = "EasterEgg";

    private bool _hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_hasTriggered)
        {
            _hasTriggered = true;

            // Buscamos al enemigo por nombre
            GameObject enemy = GameObject.Find("SkeletonIdleEasterEgg");

            bool enemyIsDead = false;

            // Si NO lo encontramos, asumimos que está muerto (lo destruiste en Die())
            if (enemy == null)
            {
                enemyIsDead = true;
            }

            string sceneToLoad = enemyIsDead ? sceneIfDead : sceneIfAlive;

            Debug.Log("Iniciando viaje a: " + sceneToLoad);

            ScreenFader.Instance.LoadSceneWithFade(sceneToLoad);
        }
    }
}