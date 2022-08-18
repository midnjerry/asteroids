using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public WorldBorder worldBorder;
    private float speed = 500.0f;
    private float maxLifetime = .25f;
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
            Destroy(this.gameObject, this.maxLifetime);
        }
    }

    public void Project(Vector2 direction)
    {
        rigidBody.AddForce(direction * speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(this.gameObject);
    }
}
