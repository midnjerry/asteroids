using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BigSaucer : MonoBehaviour
{
    public delegate void BigSaucerDestructionEvent(BigSaucer saucer);
    public static event BigSaucerDestructionEvent OnDestroyed;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidBody;
    public WorldBorder worldBorder;
    public EnemyBullet enemyBulletPreFab;
    private HashSet<Asteroid> asteroids;
    private float lastShotTime;
    private float lastTurnTime;
    public float firingRate = 1f;
    public float turnRate = 3f;

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

    public void setAsteroids(HashSet<Asteroid> asteroids)
    {
        this.asteroids = asteroids;
    }

    void FixedUpdate()
    {
        if (worldBorder.IsOutsideYBounds(this.transform.position))
        {
            this.transform.position = worldBorder.CalculateWrappedPosition(this.transform.position);
        }
        else if (worldBorder.IsOutsideXBounds(this.transform.position))
        {
            Destroy(this.gameObject);
        }

        if (this.gameObject.activeSelf)
        {
            attemptShot();
        }
    }

    public void setSmall()
    {
        this.transform.localScale = new Vector2(.7f, .7f);
    }

    void attemptShot()
    {
        var now = Time.time;
        if (now - lastShotTime > firingRate)
        {
            lastShotTime = now;
            Shoot();
        }
        
        if (now - lastTurnTime > turnRate)
        {
            lastTurnTime = now;
            ChangeDirection();

        }
    }

    private void Shoot()
    {
        EnemyBullet bullet = Instantiate(this.enemyBulletPreFab, this.transform.position, this.transform.rotation);
        bullet.worldBorder = worldBorder;
        int x = this.transform.position.x < 0 ? 1 : -1;
        int y = UnityEngine.Random.Range(-1, 1);
        Vector2 direction = new Vector2(x, y);

        if (asteroids.Count > 0)
        {
            Asteroid target = asteroids.ToList()[Random.Range(0, asteroids.Count)];
            direction = (target.transform.position - this.transform.position).normalized;
        }

        bullet.Project(direction);
        
    }

    private void ChangeDirection()
    {
        int x = rigidBody.velocity.x > 0 ? 1 : -1;
        int y = UnityEngine.Random.Range(-1, 2);
        Debug.Log("x = " + x + " y = "+ y);
        Vector2 direction = new Vector2(x, y);
        rigidBody.velocity = direction.normalized * 3;
        //rigidBody.AddForce(direction * 100);
    }

    private void Start()
    {
        lastTurnTime = Time.time;
        lastShotTime = Time.time;
        int x = this.transform.position.x < 0 ? 1 : -1;
        int y = UnityEngine.Random.Range(-1, 2);

        Vector2 direction = new Vector2(x, y);
        //rigidBody.AddForce(direction * 100);
        rigidBody.velocity = direction.normalized * 3;

    }
}