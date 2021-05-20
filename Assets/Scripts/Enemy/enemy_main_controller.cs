using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class enemy_main_controller : MonoBehaviour
{
    public GameObject player;
    public float speed;
    public float directionChangeFrequency;
    public float wanderDistance;
    public float chaseDistance;
    public int currentHealth = 3;

    protected enemyStates enemyState = enemyStates.WANDER;
    protected Vector2 startingPosition;

    protected float horizontal = 0;
    protected float vertical = 0;
    protected float directionChangeTime;
    protected AIPath path;
    protected Animator animator;
    protected float deathTimer;
    protected float prevX;

    private bool deathTimerSetOnlyOnce = true;

    // Start is called before the first frame update
    protected void Start()
    {
        startingPosition = transform.position;
        gameObject.GetComponent<AIDestinationSetter>().target = player.transform;
        path = GetComponent<AIPath>();
        animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        {
            enemyDeath();
            if (deathTimerSetOnlyOnce)
            {
                deathTimer = Time.time;
                deathTimerSetOnlyOnce = false;
            }
        }
    }

    protected abstract void enemyDeath();
 

    protected void Move()
    {
        Vector2 position = transform.position;
        position.x = position.x + horizontal * speed * Time.deltaTime;
        position.y = position.y + vertical * speed * Time.deltaTime;
        transform.position = position;
    }

    protected bool PlayerIsInChaseDistance()
    {
        float distanceFromPlayer = Mathf.Sqrt(Mathf.Abs(player.transform.position.x - transform.position.x) + Mathf.Abs(player.transform.position.y - transform.position.y));
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

    public void changeHealth(int value)
    {
        currentHealth = currentHealth + value;
    }



}

public enum enemyStates
{
    WANDER,
    CHASE,
    ATTACK
}