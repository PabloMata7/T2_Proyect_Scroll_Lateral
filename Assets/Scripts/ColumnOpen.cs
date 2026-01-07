using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnOpen : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject DoorOpen;
    public float ColumnLife = 2.0f;
    void Start()
    {
        
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
