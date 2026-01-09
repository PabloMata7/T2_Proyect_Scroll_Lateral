using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnOpen : MonoBehaviour
{
    [Header("Configuración")]
    // Start is called before the first frame update
    public GameObject DoorOpen;
    public float ColumnLife = 2.0f;
    public Sprite[] spritesRotura;

    private SpriteRenderer _spriteRenderer;
    private int _indiceRotura = 0;

    private Material material;
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Arrow"))
        {
            ColumnLife--;

            if (ColumnLife <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                ChangeSprite();
            }
        }
    }
    private void OnDestroy()
    {
        if(DoorOpen != null)
        {
            Door script = DoorOpen.GetComponent<Door>();
            if(script != null)
            {
                script.OpenDoor();
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
