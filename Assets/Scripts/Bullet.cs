using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void Awake()
    {
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            StartCoroutine(collision.GetComponent<PlayerController>().Hit());
            collision.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            collision.GetComponent<Rigidbody2D>().AddForce(GetComponent<Rigidbody2D>().linearVelocity * 50);
            Destroy(gameObject);
        }
    }
}
