using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnOpen : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject DoorOpen;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
