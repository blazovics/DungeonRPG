using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy1_controller : enemy_main_controller
{


    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

        switch (enemyState)
        {
            case enemyStates.WANDER:
                {

                    if (NeedsNewDirection())
                    {
                        horizontal = Random.Range(-1.0f, 1.0f);
                        vertical = Random.Range(-1.0f, 1.0f);
                        directionChangeTime = Time.time;
                    }
                    Move();
                    animationDuringWander();
                    /*
                    if (PlayerIsInChaseDistance())
                    {
                        enemyState = enemyStates.CHASE;
                        prevX = transform.position.x;
                    }
                    */
                    break;
                }
                /*
            case enemyStates.CHASE:
                {
                    path.canSearch = true;  
                    if (!PlayerIsInChaseDistance())
                    {
                        enemyState = enemyStates.WANDER;
                        path.canSearch = false;
                    }
                    if (path.reachedEndOfPath)
                    {
                        enemyState = enemyStates.ATTACK;
                    }
                    break;
                }
                */
            case enemyStates.ATTACK:
                {
                    animator.SetBool("Attack", true);
                    float dist = Mathf.Sqrt(Mathf.Pow(player.position.x - transform.position.x, 2) + Mathf.Pow(player.position.y - transform.position.y, 2));
                    if (dist > path.endReachedDistance)
                    {
                        enemyState = enemyStates.CHASE;
                        animator.SetBool("Attack", false);
                    }
                    break;
                }
        }

    }
    private void FixedUpdate()
    {
        switch(enemyState)
        {
            case enemyStates.CHASE:
                {
                    animationDuringPathSearch();
                    break;
                }
        }
    }
    void animationDuringPathSearch()
    {
        if (prevX > transform.position.x) animator.SetFloat("Move", -0.5f);
        else animator.SetFloat("Move", 0.5f);
        prevX = transform.position.x;
    }

    void animationDuringWander()
    {
        if (horizontal > 0) animator.SetFloat("Move", 0.5f);
        else animator.SetFloat("Move", -0.5f);
    }

}
