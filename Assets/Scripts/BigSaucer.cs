using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigSaucer : MonoBehaviour
{
    public delegate void BigSaucerDestructionEvent(BigSaucer saucer);
    public static event BigSaucerDestructionEvent OnDestroyed;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidBody;
    public WorldBorder worldBorder;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnDestroyed?.Invoke(this);
        Destroy(this.gameObject);
    }

    void FixedUpdate()
    {
        if (worldBorder.IsOutOfBounds(this.transform.position))
        {
            this.transform.position = worldBorder.CalculateWrappedPosition(this.transform.position);
        }
    }

    private void Start()
    {
        rigidBody.AddForce(this.transform.up * 10);
    }
}