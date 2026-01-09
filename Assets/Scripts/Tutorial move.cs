using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorialmove : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject text;
    public float time = 2.5f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (text != null)
            {
                text.SetActive(true);
                Invoke("OcultarTexto",time);
            }
        }
    }
    void OcultarTexto()
    {
        text.SetActive(false);
    }
}
