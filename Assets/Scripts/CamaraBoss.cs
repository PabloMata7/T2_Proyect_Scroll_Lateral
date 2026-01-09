using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraBoss : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target; // Arrastra aquí a tu Player

    [Header("Settings")]
    [SerializeField] private float smoothTime = 0.15f; // Tiempo para alcanzar al objetivo (0 = instantáneo)
    [SerializeField] private Vector3 offset; // Desfase (puedes ajustarlo en el editor o dejar que se calcule solo)

    // Estado interno
    private float _minX; // La posición X mínima permitida (el "trinquete")
    private Vector3 _currentVelocity; // Referencia para el algoritmo SmoothDamp

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("¡Falta asignar el Target en la OneWayCamera!");
            enabled = false;
            return;
        }

        // Si el offset es (0,0,0), lo calculamos automáticamente basado en la posición inicial
        if (offset == Vector3.zero)
        {
            offset = transform.position - target.position;
        }

        // Inicializamos el límite con la posición actual de la cámara
        _minX = transform.position.x;
    }

    // Usamos LateUpdate, NO Update ni FixedUpdate
    private void LateUpdate()
    {
        if (!target) return;

        // 1. Calcular la posición ideal donde QUERRÍA estar la cámara
        Vector3 targetPosition = target.position + offset;

        // 2. Lógica de "No Retroceder" (The Ratchet)
        // Actualizamos _minX solo si la nueva posición deseada es mayor (más a la derecha)
        // que el límite anterior.
        


        // 3. Definir la posición final del frame
        // En X: Usamos _minX (que nunca disminuye).
        // En Y: Seguimos al target (o puedes bloquearlo si no quieres scroll vertical).
        // En Z: Mantenemos la Z original de la cámara (crítico para renderizado 2D).
        Vector3 finalPosition = new Vector3(targetPosition.x, transform.position.y, transform.position.z);

        // 4. Aplicar movimiento suavizado
        // Usamos SmoothDamp para un movimiento orgánico en lugar de un Lerp lineal.
        transform.position = Vector3.SmoothDamp(
            transform.position,
            finalPosition,
            ref _currentVelocity,
            smoothTime
        );
    }
}
