using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnightController : MonoBehaviour
{
    [SerializeField] float k_speed = 4.0f;
    [SerializeField] float k_rollForce = 5.0f;
    [SerializeField] int knight_maxHealth = 100;
    [SerializeField] int k_meleeDamage = 10;
    [SerializeField] int k_projectileDamage = 10;

    private float horizontal;
    private float vertical;

    private Animator k_animator;
    private Rigidbody2D k_body2d;

    public GameObject projectilePrefab;

    private int k_facingDirection = 1;
    // Health parameters
    private int knight_health { get { return knight_currentHealth; } }
    private int knight_currentHealth;
    // Roll Parameters
    private bool k_rolling = false;
    // Attack Parameters
    private bool k_attacking = false;
    private int k_currentAttack = 0;
    private float k_timeSinceAttack = 0.0f;
    private float k_delayToIdle = 0.0f;
    // Invincible Parameters
    private bool k_isInvincible = false;
    private float k_timeInvincible = 2.0f;
    private float k_InvincibleTimer;

    public Generation mapgen;
    public SpawnPlayer playerSpawn;
    public LootSpawner lootSpawner;
    public EnemySpawner enemySpawner;
    public Image blackScreen;

    //Generation

    // Start is called before the first frame update
    void Start()
    {
        k_animator = GetComponent<Animator>();
        k_body2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Input x and y
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        // Increase timer that controls attack combo
        k_timeSinceAttack += Time.deltaTime;

        // Swap direction of sprite depending on walk direction
        SwapDirection();

        // Timing of the Invinciblity
        if (k_isInvincible)
        {
            k_InvincibleTimer -= Time.deltaTime;
            if (k_InvincibleTimer < 0)
            {
                k_isInvincible = false;
            }
        }

        if (k_timeSinceAttack > 0.24f)
        {
            AE_ResetAttack();
        }

        //Projectile
        if (Input.GetMouseButtonDown(1) && !k_rolling)
        {
            Launch();
        }

        //Attack
        if (Input.GetMouseButtonDown(0) && k_timeSinceAttack > 0.25f && !k_rolling)
        {
            Attack();
        }

        // Roll
        else if (Input.GetKeyDown("left shift") && !k_rolling)
        {
            k_isInvincible = true;
            k_InvincibleTimer = k_timeInvincible;
            Roll();
        }

        //Run
        else if (Mathf.Abs(horizontal) > Mathf.Epsilon)
        {
            Run();
        }

        //Idle
        else
        {
            Idle();
        }
    }

    void FixedUpdate()
    {
        //Move
        if (!k_rolling && !k_attacking)
        {
            Move();
        }
    }

    // Changing health value
    public void ChangeHealth(int value)
    {
        if (value > 0)
        {
            if (k_isInvincible)
            {
                return;
            }
            k_isInvincible = true;
            k_InvincibleTimer = k_timeInvincible;
        }

        knight_currentHealth = Mathf.Clamp(knight_currentHealth + value, 0, knight_maxHealth);
        Debug.Log(knight_currentHealth + "/" + knight_maxHealth);
    }

    // Animation Events
    // Called in end of roll animation.
    void AE_ResetRoll()
    {
        k_rolling = false;
    }

    void AE_ResetAttack()
    {
        k_attacking = false;
    }

    public void SwapDirection()
    {
        if (horizontal > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            k_facingDirection = 1;
        }

        else if (horizontal < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            k_facingDirection = -1;
        }
    }

    public void Move()
    {
        k_body2d.velocity = new Vector2(horizontal * k_speed, vertical * k_speed);
    }

    public void Attack()
    {
        // For staying in one place
        k_body2d.velocity = new Vector2(0, 0);
        k_attacking = true;

        k_currentAttack++;

        // Loop back to one after third attack
        if (k_currentAttack > 3)
            k_currentAttack = 1;

        // Reset Attack combo if time since last attack is too large
        if (k_timeSinceAttack > 1.0f)
            k_currentAttack = 1;

        // Call one of three attack animations "Attack1", "Attack2", "Attack3"
        k_animator.SetTrigger("Attack" + k_currentAttack);

        // Reset timer
        k_timeSinceAttack = 0.0f;
    }

    public void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, k_body2d.position + Vector2.up * 0.8f, Quaternion.identity);

        ProjectileControll projectile = projectileObject.GetComponent<ProjectileControll>();
        projectile.Launch(k_facingDirection, 300);
    }

    public void Roll()
    {
        k_rolling = true;
        k_animator.SetTrigger("Roll");
        k_body2d.velocity = new Vector2(k_facingDirection * k_rollForce, vertical * k_speed);
        AE_ResetRoll();
    }

    public void Run()
    {
        // Reset timer
        k_delayToIdle = 0.05f;
        k_animator.SetInteger("AnimState", 1);
    }

    public void Idle()
    {
        // Prevents flickering transitions to idle
        k_delayToIdle -= Time.deltaTime;
        if (k_delayToIdle < 0)
            k_animator.SetInteger("AnimState", 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {       
        print("Exit");
        if (other.tag == "ExitTile")
        {

            blackScreen.CrossFadeAlpha(1.0f, 0.0f, true);
            Destroy(GameObject.FindGameObjectWithTag("ExitTile"));
            lootSpawner.resetLoot();
            enemySpawner.ResetEnemy();
            mapgen.map.SetTilemapToNull();
            mapgen.Start();
            enemySpawner.Start();
            lootSpawner.Start();
            playerSpawn.Start();
        }
    }
}
