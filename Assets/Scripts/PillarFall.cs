using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarFall : MonoBehaviour
{
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb= GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void FallPillar()
    {
        rb.bodyType =RigidbodyType2D.Dynamic;

    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player")|| other.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
