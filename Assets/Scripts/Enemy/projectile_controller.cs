using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile_controller : MonoBehaviour
{

    Rigidbody2D rigidbody2d;
    GameObject player;
    public int damage = 5;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Launch(Vector2 direction, float force, GameObject playerIn)
    {
        player = playerIn;
        rigidbody2d = GetComponent<Rigidbody2D>();
        rigidbody2d.AddForce(direction * force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject == player)
        {
            player.GetComponent<KnightController>().ChangeHealth(-damage);
        }
        Destroy(gameObject);
    }

}
