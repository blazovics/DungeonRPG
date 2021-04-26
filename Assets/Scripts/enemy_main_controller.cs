using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_main_controller : MonoBehaviour
{
    public Transform player;
    public float speed;
    public float directionChangeFrequency;
    public float wanderDistance;
    public float chaseDistance;

    protected enemyStates enemyState = enemyStates.WANDER;
    protected Vector2 startingPosition;

    protected float horizontal = 0;
    protected float vertical = 0;
    protected float directionChangeTime;
    protected AIPath path;
    protected Animator animator;

    protected float prevX;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
        path = GetComponent<AIPath>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
       
        
    }


    protected void Move()
    {
        if (horizontal > 0) animator.SetFloat("Move", 0.5f);
        else animator.SetFloat("Move", -0.5f);
        Vector2 position = transform.position;
        position.x = position.x + horizontal * speed * Time.deltaTime;
        position.y = position.y + vertical * speed * Time.deltaTime;
        transform.position = position;
    }

    protected bool PlayerIsInChaseDistance()
    {
        float distanceFromPlayer = Mathf.Sqrt(Mathf.Abs(player.position.x - transform.position.x) + Mathf.Abs(player.position.y - transform.position.y));
        return distanceFromPlayer < chaseDistance;
    }

    
    protected void GetToSomewhere(Vector2 where)
    {
        path.canSearch = true;
        
    }
    protected bool NeedsNewDirection()
    {
        if(horizontal==0 && vertical == 0)
        {
            return true;
        }

        if (Time.time - directionChangeTime > directionChangeFrequency)
        {
                return true;
        }
        return false;
    }

    protected void animationDuringPathSearch()
    {
        if (prevX > transform.position.x) animator.SetFloat("Move", -0.5f);
        else animator.SetFloat("Move", 0.5f);
        prevX = transform.position.x;
    }

}

public enum enemyStates
{
    WANDER,
    CHASE,
    ATTACK
}