using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy3_controller : enemy_main_controller
{
    float detonation;
    bool firedAlready;

    public GameObject projectilePrefab;
    public float projectileSpeed;
    Rigidbody2D rigidbody2d;

    // Start is called before the first frame update
    void Awake()
    {
        base.Awake();
        rigidbody2d = GetComponent<Rigidbody2D>();
        firedAlready = false;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        switch (enemyState)
        {
            case enemyStates.WANDER:
                {
                    if (PlayerIsInChaseDistance())
                    {
                        enemyState = enemyStates.CHASE;
                        prevX = 0;
                    }
                    break;
                }
            case enemyStates.CHASE:
                {
                    path.canSearch = true;

                    if (path.reachedEndOfPath)
                    {
                        enemyState = enemyStates.ATTACK;
                        detonation = Time.time;
                    }
                    break;
                }
            case enemyStates.ATTACK:
                {
                    path.canSearch = false;
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
                    animator.SetBool("Running", true);
                    animationDuringPathSearch();
                    break;
                }
            case enemyStates.ATTACK:
                {
                    animator.SetBool("Running", false);
                    if (Time.time - detonation > 0.7) Destroy(gameObject);
                    if (!firedAlready)
                    {
                        if (Time.time - detonation > 0.3)
                        {
                            destruct();
                            firedAlready = true;
                        }
                    }

                    break;
                }
        }
    }
    void animationDuringPathSearch()
    {

        if (prevX > transform.position.x) animator.SetFloat("Move", -0.5f);
        else animator.SetFloat("Move", 0.5f);
        prevX = transform.position.x;
        Debug.Log(prevX);
    }

    void destruct()
    {
        Tuple<float, float>[] directions =
        {
            Tuple.Create(1f,1f),
            Tuple.Create(1f,0f),
            Tuple.Create(1f,-1f),
            Tuple.Create(0f,1f),
            Tuple.Create(0f,-1f),
            Tuple.Create(-1f,1f),
            Tuple.Create(-1f,0f),
            Tuple.Create(-1f,-1f),
        };
        for(int i=0; i<directions.Length; i++)
        {
            Vector2 direction = new Vector2(directions[i].Item1, directions[i].Item2);
            GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + direction, Quaternion.identity);
            projectile_controller projectile = projectileObject.GetComponent<projectile_controller>();
            projectile.damage = 20;
            projectile.Launch(direction, projectileSpeed*10, player);
        }
    }

    protected override void enemyDeath()
    {
        return;
    }
}
