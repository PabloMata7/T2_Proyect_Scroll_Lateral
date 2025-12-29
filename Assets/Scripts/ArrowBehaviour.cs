using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour
{
    public Vector2 velocity;

    private Rigidbody2D rigidBody;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        rigidBody.velocity = velocity;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Flecha tocó: {other.gameObject.name} | Tag: {other.tag} | Layer: {other.gameObject.layer}");
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Golpea la flecha");

            GoblinBehaviour enemy = other.GetComponent<GoblinBehaviour>();

            if (enemy != null)
            {
                enemy.TakeDamage(1);
            }

            Destroy(gameObject);
        }
        else if (!other.CompareTag("Player") && !other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
