using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialBullet : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed;

    private void Awake()
    {
        Destroy(gameObject, 3f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Hero"))
        {
            SkillSpreadBullet();
            Destroy(gameObject);
        }
    }

    private void SkillSpreadBullet()
    {
        int oneShoting = 8;
        float angle = 360 / oneShoting;
        for (int j = 0; j < oneShoting; j++)
        {
            GameObject bullet;
            bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.transform.Rotate(new Vector3(0f, 0f, angle * j));
            bullet.GetComponent<Rigidbody2D>().linearVelocity = bullet.transform.right * bulletSpeed;
        }
    }
}
