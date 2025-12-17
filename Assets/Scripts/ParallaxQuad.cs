using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxQuad : MonoBehaviour
{
    [Header("Referencias")]
    // Opcional: asignar la cámara manualmente. Si es null, usa Camera.main
    [SerializeField] private Camera cam;

    [Header("Configuración")]
    // Velocidad relativa a la cámara. 
    // 0 = Se mueve con la cámara (fondo estático visualmente, como la luna).
    // 1 = Se queda quieto en el mundo (como una pared).
    // Valores bajos (0.1) son ideales para fondos muy lejanos.
    [Range(0f, 1f)][SerializeField] private float parallaxSpeed = 0.1f;

    // Referencias internas para optimización
    private Transform _camTransform;
    private Material _matInstance;
    private Vector3 _initialPositionRelativeToCam;
    private Vector2 _savedTextureScale; // Para ajustar la velocidad según el tiling

    void Start()
    {
        if (cam == null) cam = Camera.main;
        _camTransform = cam.transform;

        // Importante: Al acceder a renderer.material, Unity crea automáticamente
        // una instancia (copia) del material solo para este objeto.
        // Guardamos la referencia para no crear basura en memoria cada frame.
        Renderer rend = GetComponent<Renderer>();
        _matInstance = rend.material;

        // Guardamos el Tiling que configuraste en el editor para normalizar la velocidad
        _savedTextureScale = _matInstance.mainTextureScale;

        // Calculamos la distancia inicial Z e Y respecto a la cámara
        _initialPositionRelativeToCam = transform.position - _camTransform.position;
    }

    // Usamos LateUpdate para ir después del movimiento de la cámara
    void LateUpdate()
    {
        if (!_camTransform) return;

        // --- PARTE 1: Mover el Quad ---
        // El Quad siempre debe seguir a la cámara para estar frente a ella.
        // Mantenemos su Z e Y originales relativas.
        transform.position = new Vector3(
            _camTransform.position.x,
            _camTransform.position.y + _initialPositionRelativeToCam.y,
            _camTransform.position.z + _initialPositionRelativeToCam.z
        );

        // --- PARTE 2: Mover la Textura (UV Offset) ---

        // Calculamos cuánto se ha movido la cámara en X en el mundo.
        float worldPosX = _camTransform.position.x;

        // Calculamos el offset UV.
        // La fórmula básica es: PosiciónCámara * Velocidad.
        // Dividimos por _savedTextureScale.x para que la velocidad percibida sea
        // consistente independientemente de si el tiling es 1 o 10.
        float offsetU = (worldPosX * parallaxSpeed) / _savedTextureScale.x;

        // Aplicamos el offset solo en X, mantenemos el offset Y original del material.
        Vector2 currentOffset = _matInstance.mainTextureOffset;
        _matInstance.mainTextureOffset = new Vector2(offsetU, currentOffset.y);
    }

    // Limpieza de memoria al destruir el objeto
    private void OnDestroy()
    {
        if (_matInstance != null)
        {
            // Es buena práctica liberar la instancia del material creada en Start
            Destroy(_matInstance);
        }
    }
}
