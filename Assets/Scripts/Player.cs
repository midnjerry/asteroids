using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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
    }

    // Update is called once per frame
    void Update()
    {
        this.isThrusting = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow));
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
    }

    private void Shoot()
    {
        Bullet bullet = Instantiate(this.bulletPreFab, this.transform.position, this.transform.rotation);
        bullet.Project(this.transform.up);
        this.rigidBody.AddForce(this.transform.up * this.recoil);
    }
}
