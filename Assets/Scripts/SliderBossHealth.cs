using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
public class SliderBossHealth : MonoBehaviour
{
    public Slider healthSlider;
    public BossBehaviour bossScript;

    public float activationDistance = 8f;

    private bool isActivated = false;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    private void Start()
    {
        if (healthSlider == null) healthSlider = GetComponent<Slider>();

        if (bossScript == null)
        {
            bossScript = FindObjectOfType<BossBehaviour>();
        }

        if (bossScript != null)
        {
            bossScript.OnBossHealthChanged += UpdateHealthBar;

            UpdateHealthBar(bossScript.maxHealth, bossScript.maxHealth);
        }
        else
        {
            Debug.LogWarning("BossHealthUI: ¡No encuentro al Boss en la escena!");
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (bossScript == null)
        {
            canvasGroup.alpha = 0f; // Esconder
            return; // No seguir ejecutando el código para evitar errores
        }

        if (isActivated) return;

        if (Player.Instance != null && bossScript != null)
        {
            float distance = Vector2.Distance(Player.Instance.transform.position, bossScript.transform.position);

            if (distance < activationDistance)
            {
                ActivateBar();
            }
        }
    }

    private void ActivateBar()
    {
        isActivated = true;

        canvasGroup.alpha = 1f;

        Debug.Log("¡Jefe detectado! Barra de vida activada.");
    }

    private void OnDestroy()
    {
        if (bossScript != null)
        {
            bossScript.OnBossHealthChanged -= UpdateHealthBar;
        }
    }

    private void UpdateHealthBar(int current, int max)
    {
        float value = (float)current / max;
        Debug.Log($"UI RECIBE: Vida {current}/{max} - Slider: {value}");
        healthSlider.value = value;
    }

    private void OnDrawGizmosSelected()
    {
        if (bossScript != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(bossScript.transform.position, activationDistance);
        }
    }
}
