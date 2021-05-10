using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy2_controller : enemy_main_controller
{

    public GameObject projectilePrefab;
    Rigidbody2D rigidbody2d;

    public float projectileFrequency;
    public float projectileSpeed;
    float lastProjectile;


    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        rigidbody2d = GetComponent<Rigidbody2D>();
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
                    if (PlayerIsInChaseDistance())
                    {
                        enemyState = enemyStates.CHASE;
                        prevX = transform.position.x;
                    }
                    break;
                }
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
                        lastProjectile = Time.time;
                    }
                    break;
                }
            case enemyStates.ATTACK:
                {
                    animationDuringAttack(true);
                    float dist = Mathf.Sqrt(Mathf.Pow(player.position.x - transform.position.x, 2) + Mathf.Pow(player.position.y - transform.position.y, 2));
                    if (Time.time - lastProjectile > projectileFrequency)
                    {
                        Launch();
                        lastProjectile = Time.time;
                    }


                    if (dist >= path.endReachedDistance)
                    {
                        animationDuringAttack(false);
                        enemyState = enemyStates.CHASE;
                    }
                    break;
                }
        }

    }

    private void FixedUpdate()
    {
        switch (enemyState)
        {
            case enemyStates.CHASE:
                {
                    animationDuringPathSearch();
                    break;
                }
            case enemyStates.ATTACK:
                {
                    
                    break;
                }
        }
    }

    void Launch()
    {
        float divider = Mathf.Max(Mathf.Abs(player.position.x - transform.position.x), Mathf.Abs(player.position.y - transform.position.y));
        Vector2 direction = new Vector2((player.position.x - transform.position.x)/divider, (player.position.y - transform.position.y)/divider);
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + direction , Quaternion.identity);
        flying_eye_projectile projectile = projectileObject.GetComponent<flying_eye_projectile>();
        projectile.Launch(direction, projectileSpeed);
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

    void animationDuringAttack(bool attackSwitch)
    {
        if (player.position.x < transform.position.x) animator.SetFloat("Move", -0.5f);
        else animator.SetFloat("Move", 0.5f);
        animator.SetBool("Attack", attackSwitch);
    }

}
