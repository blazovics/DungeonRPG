using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flying_eye_projectile : MonoBehaviour
{

    Rigidbody2D rigidbody2d;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Launch(Vector2 direction, float force)
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        rigidbody2d.AddForce(direction * force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

}
