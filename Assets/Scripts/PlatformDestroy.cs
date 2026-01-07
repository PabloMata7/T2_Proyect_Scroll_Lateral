using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDestroy : MonoBehaviour
{
    public float delay =2.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DestroyPlatform());
        }
    }
    IEnumerator DestroyPlatform()
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
