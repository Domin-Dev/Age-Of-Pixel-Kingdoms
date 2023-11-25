using System;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Transform target;
    public Action hit;
    public Transform archer;

    public static void NewBullet(Vector3 startPosition, Transform target, Sprite bulletImage,Action hit,Transform archer)
    {
        Transform bullet = new GameObject("Bullet").transform;
        bullet.position = startPosition;
        bullet.AddComponent<Bullet>().target = target;
        bullet.GetComponent<Bullet>().hit = hit;
        bullet.GetComponent<Bullet>().archer = archer;
        bullet.AddComponent<SpriteRenderer>().sprite = bulletImage;
        bullet.GetComponent<SpriteRenderer>().sortingOrder = 10;
        if (startPosition.x > target.position.x) bullet.GetComponent<SpriteRenderer>().flipX = true;
    }

    private void Update()
    {
        if (target != null && archer != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, 7 * Time.deltaTime);
            if (transform.position == target.position)
            {
                hit();
                Destroy(gameObject);
            }
        }else
        {
            Destroy(gameObject);
        }
    }
}
