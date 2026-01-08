using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnOpen : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject DoorOpen;
    public float ColumnLife = 2.0f;

    public Color colorBrillo = Color.yellow;
    public float velocidad = 2f;
    public float intensidadMax = 3f;

    private Material material;
    void Start()
    {
        material = GetComponent<Renderer>().material;
        material.EnableKeyword("_EMISSION");
    }

    // Update is called once per frame
    void Update()
    {
        float intensidad = Mathf.PingPong(Time.time * velocidad, intensidadMax);
        material.SetColor("_EmissionColor", colorBrillo * intensidad);
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
}
