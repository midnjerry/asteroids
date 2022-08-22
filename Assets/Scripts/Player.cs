using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public delegate void PlayerDestructionEvent(Player player);
    public static event PlayerDestructionEvent OnDestroyed;
    public WorldBorder worldBorder;
    public Bullet bulletPreFab;
    public float thrustScaler = 4.0f;
    public float recoil = -1.5f;
    public float rotationScaler = 150f;
    private Rigidbody2D rigidBody;
    private float turnDirection;
    private bool isThrusting;

    void Awake()
    {
        this.rigidBody = GetComponent<Rigidbody2D>();
        GetComponent<AudioSource>().loop = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnDestroyed?.Invoke(this);
        this.gameObject.SetActive(false);
        this.gameObject.transform.position = Vector2.zero;
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            if (!isThrusting)
            {
                GetComponent<AudioSource>().Play();
            }
            this.isThrusting = true;
        } else
        {
            this.isThrusting = false;
            GetComponent<AudioSource>().Stop();
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            this.turnDirection = 1.0f;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            this.turnDirection = -1.0f;
        }
        else
        {
            this.turnDirection = 0.0f;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void FixedUpdate()
    {

        if (this.isThrusting)
        {
            this.rigidBody.AddForce(this.transform.up * this.thrustScaler);            
        }

        if (turnDirection != 0.0f)
        {
            this.rigidBody.angularVelocity = 0;
            this.rigidBody.AddTorque(this.turnDirection * this.rotationScaler);
        } else
        {
            this.rigidBody.angularVelocity = 0;
        }
        if (worldBorder.IsOutOfBounds(this.transform.position))
        {
            this.transform.position = worldBorder.CalculateWrappedPosition(this.transform.position);
        }
    }

    private void Shoot()
    {
        Bullet bullet = Instantiate(this.bulletPreFab, this.transform.position, this.transform.rotation);
        bullet.worldBorder = worldBorder;
        bullet.Project(this.transform.up);
        this.rigidBody.AddForce(this.transform.up * this.recoil);
    }
}
