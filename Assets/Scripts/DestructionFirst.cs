using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionFirst : MonoBehaviour
{


    // Start is called before the first frame update
    public PillarFall pillarFall;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (pillarFall != null)
            {
                pillarFall.FallPillar();
            }

            Destroy(gameObject);
        }
    }
}