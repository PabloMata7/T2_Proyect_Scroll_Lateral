using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody rb;
    void Start()
    {
        rb= GetComponent<Rigidbody>();
        rb.isKinematic=true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenDoor()
    {
        rb.isKinematic=false;
        rb.AddForce(Vector3.forward * 5f, ForceMode.Impulse);
    }
}
