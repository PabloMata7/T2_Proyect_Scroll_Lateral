using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Column : MonoBehaviour
{
    public float ColumnLife = 3.0f;
    // Start is called before the first frame update
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
            
            if(ColumnLife<=0)
            {
                Destroy(gameObject);
            }
        }
    }
}
