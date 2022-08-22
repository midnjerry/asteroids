using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Saucer : MonoBehaviour
{
    public static float BIG = 1.5F;
    public static float SMALL = .7f;
    public delegate void SaucerDestructionEvent(Saucer saucer);
    public static event SaucerDestructionEvent OnDestroyed;
    private Rigidbody2D rigidBody;
    public WorldBorder worldBorder;
    public EnemyBullet enemyBulletPreFab;
    private HashSet<GameObject> targets;
    private float size = BIG;
    private float lastShotTime;
    private float lastTurnTime;
    public float firingRate = 1f;
    public float turnRate = 3f;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnDestroyed?.Invoke(this);
        Destroy(this.gameObject);
    }

    public void setTargets(HashSet<GameObject> targets)
    {
        this.targets = targets;
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

    public bool isSmall()
    {
        return size <= SMALL;
    }

    public void setSize(float size)
    {
        this.size = size;
        this.transform.localScale = new Vector2(size, size);
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

        if (targets.Count > 0)
        {
            GameObject target = targets.ToList()[Random.Range(0, targets.Count)];
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