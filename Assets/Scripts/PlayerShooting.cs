using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject ArrowRightPrefab;
    public GameObject ArrowLeftPrefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Player.Instance.facingRight)
            {
                GameObject gameObject = Object.Instantiate(ArrowRightPrefab, base.transform.position, Quaternion.identity);
                Physics2D.IgnoreCollision(gameObject.GetComponent<BoxCollider2D>(), base.transform.parent.GetComponent<BoxCollider2D>());
            }
            else
            {
                GameObject gameObject = Object.Instantiate(ArrowLeftPrefab, base.transform.position, Quaternion.Euler(0, 180, 0));
                Physics2D.IgnoreCollision(gameObject.GetComponent<BoxCollider2D>(), base.transform.parent.GetComponent<BoxCollider2D>());
            }
        }
    }
}
