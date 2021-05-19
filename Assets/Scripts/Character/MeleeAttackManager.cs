using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackManager : MonoBehaviour
{
    private BoxCollider2D k_boxCol2D_attack;

    private float attackbox_timer;

    // Update is called once per frame
    void Update()
    {
        attackbox_timer -= Time.deltaTime;

        if (attackbox_timer < 0)
        {
            Destroy(gameObject);
        }
    }

    public void AttackBoxColl(int facingDirection, float k_body_x, float k_body_y)
    {
        attackbox_timer = 0.07f;
        if (facingDirection.Equals(1))
        {
            k_boxCol2D_attack = gameObject.AddComponent<BoxCollider2D>() as BoxCollider2D;
            k_boxCol2D_attack.size = new Vector2(1.0f, 1.7f);
            k_boxCol2D_attack.transform.position = new Vector2(k_body_x + 1.0f, k_body_y + 0.9f);
        }
        else
        {
            k_boxCol2D_attack = gameObject.AddComponent<BoxCollider2D>() as BoxCollider2D;
            k_boxCol2D_attack.size = new Vector2(1.0f, 1.7f);
            k_boxCol2D_attack.transform.position = new Vector2(k_body_x - 1.1f, k_body_y + 0.9f);
        }

        
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        //we also add a debug log to know what the projectile touch
        Debug.Log("Attacked " + other.gameObject);
    }
}
