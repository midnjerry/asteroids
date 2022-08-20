using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Hunter : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private float speed = 3f;
    private float rotateSpeed = 100;

    GameObject target;
    public WorldBorder worldBorder;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rigidBody.velocity = transform.up * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(this.gameObject);
    }

    public void setTarget(GameObject target)
    {
        this.target = target;
    }

    void FixedUpdate()
    {
        if (worldBorder != null && worldBorder.IsOutOfBounds(this.transform.position))
        {
            this.transform.position = worldBorder.CalculateWrappedPosition(this.transform.position);
        }

        if (target != null)
        {
            Vector2 direction = (Vector2)target.transform.position - rigidBody.position;
            direction.Normalize();
            float rotateAmount = Vector3.Cross(transform.up, direction).z;
            rigidBody.angularVelocity = rotateAmount * rotateSpeed;
            rigidBody.velocity = transform.up * speed;
        }
    }
}
