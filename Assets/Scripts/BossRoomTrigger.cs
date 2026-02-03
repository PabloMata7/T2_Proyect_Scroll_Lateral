using UnityEngine;
using UnityEngine.SceneManagement;

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

            // buscar al enemigo específico
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
                // si ya no existe, está muerto
                enemyIsDead = true;
            }

            string sceneToLoad = enemyIsDead ? easterEggScene : finalBossScene;

            // si ScreenFader existe, se usa
            if (ScreenFader.Instance != null)
            {
                ScreenFader.Instance.LoadSceneWithFade(sceneToLoad);
            }
            else
            {
                // si no existe, carga la escena directamente
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}