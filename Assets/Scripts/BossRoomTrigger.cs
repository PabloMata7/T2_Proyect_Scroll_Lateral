using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomTrigger : MonoBehaviour
{
    [SerializeField] private string finalBossScene = "FinalBoss";
    [SerializeField] private string easterEggScene = "EasterEgg";

    private bool _hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_hasTriggered)
        {
            _hasTriggered = true;

            // Buscamos al enemigo especial
            GameObject enemy = GameObject.Find("SkeletonIdleEasterEgg");

            bool enemyIsDead = false;

            if (enemy != null)
            {
                EnemyBehaviour eb = enemy.GetComponent<EnemyBehaviour>();
                if (eb != null)
                    enemyIsDead = eb.isDead;
            }
            else
            {
                // Si ya no existe, está muerto
                enemyIsDead = true;
            }

            // Elegimos la escena según si está muerto o no
            string sceneToLoad = enemyIsDead ? easterEggScene : finalBossScene;

            Debug.Log("Iniciando viaje a: " + sceneToLoad);

            ScreenFader.Instance.LoadSceneWithFade(sceneToLoad);
        }
    }
}
