using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionEmpty : MonoBehaviour
{
    
    // Start is called before the first frame update
    public PillarFall[] pillarFall;
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
            foreach (PillarFall pillar in pillarFall)
            {
                if (pillarFall != null)
                {
                    pillar.FallPillar();
                }

            }
                
            Destroy(gameObject);
            
        }
    }
}
