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
                enemyIsDead = true; // si ya no existe, está muerto
            }

            string sceneToLoad = enemyIsDead ? easterEggScene : finalBossScene;

            ScreenFader.Instance.LoadSceneWithFade(sceneToLoad);
        }
    }
}
