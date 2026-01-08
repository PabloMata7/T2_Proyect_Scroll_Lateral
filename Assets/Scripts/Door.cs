using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
   
    // Start is called before the first frame update
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2D;
    void Start()
    {
        rb= GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        rb.isKinematic=true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenDoor()
    {
        rb.isKinematic=false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        boxCollider2D.isTrigger = true;
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Destruction"))
        {
            Destroy(gameObject);
        }
    }
}
