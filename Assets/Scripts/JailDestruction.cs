using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JailDestruction : MonoBehaviour
{
    public JailFall pillarFall;
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
