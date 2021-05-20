using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileControll : MonoBehaviour
{
    private Animator f_animator;
    private Rigidbody2D f_body2d;

    private float fireball_timer;
    private Vector2 direction;

    public int dmg;

    // Awake called after the creation of the object
    void Awake()
    {
        f_body2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        fireball_timer -= Time.deltaTime;

        //Fireball vanish after a time
        if (fireball_timer < 0)
        {
            Destroy(gameObject);
        }
    }

    public void Launch(int facingDirection,float force)
    {
        fireball_timer = 3.0f;

        if(facingDirection.Equals(1))
        {
            GetComponent<SpriteRenderer>().flipX = false;
            direction = new Vector2(2, 0);
        }

        else if(facingDirection.Equals(-1))
        {
            GetComponent<SpriteRenderer>().flipX = true;
            direction = new Vector2(-2, 0);
        }

        f_body2d.AddForce(direction * force);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        //we also add a debug log to know what the projectile touch
        try
        {
            other.gameObject.GetComponent<enemy_main_controller>().currentHealth -= dmg;
        }
        catch
        {

        }
        
        Destroy(gameObject);
    }
}
