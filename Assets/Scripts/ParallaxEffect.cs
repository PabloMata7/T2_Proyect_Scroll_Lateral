using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Camera cam;

   
    [Tooltip("0 = Estático (Suelo), 1 = Pegado a la cámara (Cielo infinito)")]
    [Range(0f, 1f)] public float parallaxFactor;

    [Tooltip("Activar si la textura se debe repetir infinitamente")]
    public bool infiniteLoop = true;

    // Estado interno
    private float _startPositionX;
    private float _textureUnitSizeX;

    // Cacheamos el ancho de la cámara para no calcularlo cada frame si es ortográfica
    private float _cameraHalfWidth;

    private void Start()
    {
        // Si no asignas cámara manual, busca la principal (costoso en Start, pero aceptable)
        if (cam == null) cam = Camera.main;

        _startPositionX = transform.position.x;

        // Calculamos el ancho real del sprite en unidades de Unity
        // Esto es necesario para saber cuándo "teletransportar" el fondo
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            _textureUnitSizeX = sprite.bounds.size.x;
        }

        // Calculamos el ancho visible de la cámara (Ortográfica)
        // OrthographicSize es la mitad de la altura. Multiplicamos por el Aspect Ratio para el ancho.
        _cameraHalfWidth = cam.orthographicSize * cam.aspect;
    }

    // Usamos LateUpdate para ir sincronizados con el script de cámara (OneWayCamera)
    // Si usáramos Update, habría un frame de desincronización causando jitter.
    private void LateUpdate()
    {
        // 1. CÁLCULO DE DESPLAZAMIENTO (LA MAGIA DEL PARALLAX)

        // temp: Cuánto se ha movido la cámara en el mundo REAL. 
        // Usamos (1 - factor) porque si el factor es 1 (cielo), temp será 0, y el objeto nunca se "alejará" del inicio relativo.
        //float temp = (cam.transform.position.x * (1 - parallaxFactor));

        // dist: Cuánto debemos mover este objeto para acompañar a la cámara.
        float dist = (cam.transform.position.x * parallaxFactor);

        // Posición visual actual del objeto
        Vector3 newPos = new Vector3(_startPositionX + dist, transform.position.y, transform.position.z);
        transform.position = newPos;

        // 2. RE-CENTERING BASADO EN LÍMITES DE CÁMARA
        if (infiniteLoop)
        {
            // Calculamos los bordes mundiales actuales
            float cameraRightEdge = cam.transform.position.x + _cameraHalfWidth;
            float cameraLeftEdge = cam.transform.position.x - _cameraHalfWidth;

            // Asumimos Pivot en el CENTRO (0.5, 0.5)
            float spriteRightEdge = transform.position.x + (_textureUnitSizeX / 2);
            float spriteLeftEdge = transform.position.x - (_textureUnitSizeX / 2);

            // CASO A: Vemos hueco a la DERECHA
            // Si el borde derecho de la cámara ha superado al borde derecho del sprite...
            // (Restamos un pequeño offset "0.1f" para evitar flickering por un pixel)
            if (cameraRightEdge > spriteRightEdge)
            {
                // Movemos el anclaje inicial una unidad a la derecha
                _startPositionX += _textureUnitSizeX;
            }
            // CASO B: Vemos hueco a la IZQUIERDA
            else if (cameraLeftEdge < spriteLeftEdge)
            {
                _startPositionX -= _textureUnitSizeX;
            }
        }
    }


}
