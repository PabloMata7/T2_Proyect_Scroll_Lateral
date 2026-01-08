using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Column : MonoBehaviour
{
    [Header("Configuración")]
    // Aumentamos la vida a 3:
    // 1 golpe -> se rompe un poco
    // 2 golpes -> se rompe mucho
    // 3 golpes -> se destruye
    public float ColumnLife = 2.0f;

    // Aquí arrastrarás las 2 imágenes que te generé (o más)
    public Sprite[] spritesRotura;

    // Variable privada para controlar el componente visual
    private SpriteRenderer _spriteRenderer;

    // Un contador interno para saber qué imagen del array toca poner
    private int _indiceRotura = 0;

    void Start()
    {
        // Obtenemos el componente que dibuja el sprite al iniciar
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // El update se queda vacío si no necesitamos hacer nada cada frame
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Arrow"))
        {
            ColumnLife--; // Restamos vida

            // Si la vida llega a 0, destruimos el objeto
            if (ColumnLife <= 0)
            {
                Destroy(gameObject);
            }
            // Si todavía tiene vida, cambiamos el sprite
            else
            {
                ChangeSprite();
            }
        }
    }
    void ChangeSprite()
    {
        // Verificamos que no nos salgamos del límite del array para evitar errores
        if (_indiceRotura < spritesRotura.Length)
        {
            // Asignamos la nueva imagen al componente SpriteRenderer
            _spriteRenderer.sprite = spritesRotura[_indiceRotura];

            // Aumentamos el índice para que la próxima vez coja la siguiente imagen
            _indiceRotura++;
        }
    }
}
