using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class DiamondHunter : MonoBehaviour
{
    public delegate void DiamondHunterDestructionEvent(DiamondHunter hunter);
    public static event DiamondHunterDestructionEvent OnDestroyed;
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
        OnDestroyed?.Invoke(this);
        Destroy(this.gameObject);
    }

    public void setTarget(GameObject target)
    {
        this.target = target;
    }

    void FixedUpdate()
    {
        if (worldBorder.IsOutOfBounds(this.transform.position))
        {
            this.transform.position = worldBorder.CalculateWrappedPosition(this.transform.position);
        }

        Vector2 direction = (Vector2)target.transform.position - rigidBody.position;
        direction.Normalize();
        float rotateAmount = Vector3.Cross(transform.up, direction).z;
        rigidBody.angularVelocity = rotateAmount * rotateSpeed;
        rigidBody.velocity = transform.up * speed;

    }
}
