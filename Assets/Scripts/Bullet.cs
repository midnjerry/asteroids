using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public WorldBorder worldBorder;
    public float speed = 500.0f;
    public float lifetimeAfterTeleport = 2f;
    private Rigidbody2D rigidBody;
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (worldBorder.IsOutOfBounds(this.transform.position))
        {
            this.transform.position = worldBorder.CalculateWrappedPosition(this.transform.position);   
        }
    }

    public void Project(Vector2 direction)
    {
        rigidBody.AddForce(direction * speed);
        Destroy(this.gameObject, this.lifetimeAfterTeleport);
    }

}
