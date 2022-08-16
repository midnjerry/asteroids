using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public const float LARGE = 2.0f;
    public const float MEDIUM = 1.0f;
    public const float SMALL = .5f;
    public delegate void AsteroidDestructionEvent(Asteroid asteroid);
    public static event AsteroidDestructionEvent OnDestroyed;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidBody;
    public WorldBorder worldBorder;
    private float size;


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

    public void ScaleSize(float size)
    {
        this.size = size;
        this.transform.localScale = new Vector2(size, size);
    }

    public float getSize()
    {
        return this.size;
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
        this.transform.eulerAngles = new Vector3(0, 0, Random.value * 360.0f);
        rigidBody.angularVelocity = Random.Range(0, 360) - 180;
        float randomSpeed = Random.Range(100, 200);
        rigidBody.AddForce(this.transform.up * rigidBody.angularVelocity);
    }
}

