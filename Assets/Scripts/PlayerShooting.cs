using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject ArrowRightPrefab;
    public GameObject ArrowLeftPrefab;

    [SerializeField] private float spawnDelay = 0.7f;
    private float lifeTime = 5.0f;

    [SerializeField] private float shootingAnimationDuration = 1.23f;
    private bool isShooting = false;

    private void Awake()
    {
        if (spawnDelay >= shootingAnimationDuration)
        {
            Debug.LogWarning("PlayerShooting: El spawnDelay es mayor que la duración total. Se ha ajustado para evitar errores.");
            shootingAnimationDuration = spawnDelay + 0.1f;
        }
    }

    private void Start()
    {
        //Destroy(gameObject, lifeTime);    
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isShooting)
        {
            StartCoroutine(ShootSequence());
        }
    }

    private IEnumerator ShootSequence()
    {
        isShooting = true;

        if (Player.Instance != null)
        {
            Player.Instance.isShooting = true;
        }

        // 1. ACTIVAR ANIMACIÓN (INMEDIATO)
        // Queremos que el personaje empiece a moverse ya.
        Player.Instance.TriggerShootAnim();

        // 2. ESPERAR (DELAY)
        // Devolvemos el control al motor y esperamos X segundos
        yield return new WaitForSeconds(spawnDelay);

        // 3. LÓGICA DE INSTANCIACIÓN (TRAS EL DELAY)
        SpawnArrow();

        yield return new WaitForSeconds(shootingAnimationDuration - spawnDelay);

        if (Player.Instance != null)
        {
            Player.Instance.isShooting = false;
        }

        isShooting = false;
    }

    private void SpawnArrow()
    {
        if (Player.Instance.facingRight)
        {
            GameObject gameObject = Object.Instantiate(ArrowRightPrefab, base.transform.position, Quaternion.identity);
            Physics2D.IgnoreCollision(gameObject.GetComponent<BoxCollider2D>(), base.transform.parent.GetComponent<BoxCollider2D>());
            Destroy(gameObject, lifeTime);
        }
        else
        {
            GameObject gameObject = Object.Instantiate(ArrowLeftPrefab, base.transform.position, Quaternion.Euler(0, 180, 0));
            Physics2D.IgnoreCollision(gameObject.GetComponent<BoxCollider2D>(), base.transform.parent.GetComponent<BoxCollider2D>());
            Destroy(gameObject, lifeTime);
        }
    }
}
