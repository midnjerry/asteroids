using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 500.0f;
    public float maxLifetime = 2.0f;
    private Rigidbody2D rigidBody;
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    public void Project(Vector2 direction)
    {
        rigidBody.AddForce(direction * speed);
        Destroy(this.gameObject, this.maxLifetime);
    }

}
