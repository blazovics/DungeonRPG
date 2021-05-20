using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KnightController : MonoBehaviour, IDamageManager
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
    public GameObject attackCollider;

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
    public float k_timeInvincible = 0.75f;
    private float k_InvincibleTimer;
    //Inventory Parameters
    public int Urns { get; set; } = 0;
    public int Spiritboxes { get; set; } = 0;
    public GameObject pathfinder;

    public float spiritboxTimeBuffed = 10.0f;
    private bool isSpiritboxBuffed = false;
    private float spiritboxTimer;
    private float percentage;
    private float k_preSpiritBoxSpeed;
    private int k_preSpiritBoxMeleeDamage;
    private int k_preSpiritBoxProjectileDamage;
    //Inventory Display Parameters
    //Urn Dialog
    public float displayTime = 1.0f;
    public GameObject urnDialogBox;
    float timerUrnDisplay;
    //Spiritbox Dialog
    public GameObject spiritboxDialogBox;
    public GameObject spiritboxUI;
    float timerSpiritboxDisplay;
    float timerSpiritboxUIDisplay;
    //Audio
    AudioSource audioSource;
    public AudioClip spiritboxUsedClip;
    public AudioClip urnUsedClip;
    private float random;
    public AudioClip hit1Clip;
    public AudioClip hit2Clip;
    public AudioClip hit3Clip;
    public AudioClip attack1Clip;
    public AudioClip attack2Clip;
    public AudioClip attack3Clip;
    public AudioClip rollClip;
    public AudioClip fireball1Clip;
    public AudioClip fireball2Clip;
    public AudioClip fireball3Clip;
    public AudioClip footstep1Clip;
    public AudioClip footstep2Clip;
    public AudioClip footstep3Clip;
    public AudioClip footstep4Clip;
    private bool isRunning = false;
    private float timeRunning = 0.4f;
    private float timerRunning;
    //Mapgen
    public Generation mapgen;
    public SpawnPlayer playerSpawn;
    public LootSpawner lootSpawner;
    public EnemySpawner enemySpawner;
    public Image blackScreen;
    //Death
    private float deathTimer = 1.0f;
    private bool isDying = false;

    // Start is called before the first frame update
    void Start()
    {
        k_animator = GetComponent<Animator>();
        k_body2d = GetComponent<Rigidbody2D>();

        knight_currentHealth = knight_maxHealth;

        //Consumable Display
        urnDialogBox.SetActive(false);
        timerUrnDisplay = -1.0f;
        spiritboxDialogBox.SetActive(false);
        timerSpiritboxDisplay = -1.0f;
        spiritboxUI.SetActive(false);
        timerSpiritboxUIDisplay = -1.0f;

        //Audio
        audioSource = GetComponent<AudioSource>();
        k_preSpiritBoxSpeed = k_speed;
        SetFootstepFrequency();
    }

    // Update is called once per frame
    void Update()
    {

        if(knight_currentHealth <= 0)
        {
            if (!isDying)
            {
                k_animator.SetTrigger("Death");
                isDying = true;
            }
            deathTimer -= Time.deltaTime;
            if(deathTimer <= 0)
            {
                Death();
            }
        }

        // Input x and y
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        // Increase timer that controls attack combo
        k_timeSinceAttack += Time.deltaTime;

        // Swap direction of sprite depending on walk direction
        SwapDirection();

        // Timing of the running audio
        if (timerRunning >= 0)
        {
            timerRunning -= Time.deltaTime;
            if (timerRunning < 0)
            {
                isRunning = false;
            }
        }

        // Timing of the Urn Consumable Display
        if (timerUrnDisplay >= 0)
        {
            timerUrnDisplay -= Time.deltaTime;
            if (timerUrnDisplay < 0)
            {
                urnDialogBox.SetActive(false);
            }
        }

        // Timing of the Spiritbox Consumable Display
        if (timerSpiritboxDisplay >= 0)
        {
            timerSpiritboxDisplay -= Time.deltaTime;
            if (timerSpiritboxDisplay < 0)
            {
                spiritboxDialogBox.SetActive(false);
            }
        }

        // Timing of the Spiritbox Consumable UI Display
        if (timerSpiritboxUIDisplay >= 0)
        {
            timerSpiritboxUIDisplay -= Time.deltaTime;
            if (timerSpiritboxUIDisplay < 0)
            {
                spiritboxUI.SetActive(false);
            }
        }

        // Timing of the Spiritbox Buff
        if (isSpiritboxBuffed)
        {
            spiritboxTimer -= Time.deltaTime;
            if (spiritboxTimer < 0)
            {
                SpiritboxRemoveBuff();
            }
        }

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
        if (Input.GetMouseButtonDown(1) && k_timeSinceAttack > 0.5f && !k_rolling)
        {
            Launch();
        }

        //Attack
        else if (Input.GetMouseButtonDown(0) && k_timeSinceAttack > 0.25f && !k_rolling)
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

        //Damaging the knight, for test purposes
        else if (Input.GetKeyDown("f") && !k_rolling)
        {
            DamageKnightTest();
        }

        //Use Urn Collectible
        else if (Input.GetKeyDown("q") && !k_rolling)
        {
            UseUrn();
        }

        //Use Spiritbox Collectible
        else if (Input.GetKeyDown("e") && !k_rolling && !isSpiritboxBuffed)
        {
            UseSpiritbox();
        }

        //Run
        else if (Mathf.Abs(horizontal) > Mathf.Epsilon || Mathf.Abs(vertical) > Mathf.Epsilon)
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
        if (value < 0)
        {
            if (k_isInvincible)
            {
                return;
            }
            random = Random.value;
            if (random < 0.33f) { PlaySound(hit1Clip); }
            else if (random > 0.66f) { PlaySound(hit2Clip); }
            else { PlaySound(hit3Clip); }
            Hurt();

            k_isInvincible = true;
            k_InvincibleTimer = k_timeInvincible;
        }

        knight_currentHealth = Mathf.Clamp(knight_currentHealth + value, 0, knight_maxHealth);
        //Debug.Log(knight_currentHealth + "/" + knight_maxHealth);
        HealthbarUI.instance.SetValue(knight_currentHealth / (float)knight_maxHealth);
    }

    public void DamageTaken(int value)
    {
        if (value > 0)
        {
            if (k_isInvincible)
            {
                return;
            }
            knight_currentHealth -= value;
            Hurt();

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
        // AttackCollider object manager
        GameObject attackManagerObject = Instantiate(attackCollider, new Vector2(k_body2d.position.x + 30.0f, k_body2d.position.y + 30.0f), Quaternion.identity);

        MeleeAttackManager Melee = attackManagerObject.GetComponent<MeleeAttackManager>();
        Melee.dmg = k_meleeDamage;
        Melee.AttackBoxColl(k_facingDirection, k_body2d.position.x, k_body2d.position.y);

        // Loop back to one after third attack
        if (k_currentAttack > 3)
            k_currentAttack = 1;

        // Reset Attack combo if time since last attack is too large
        if (k_timeSinceAttack > 1.0f)
            k_currentAttack = 1;

        //Plays Attack1, Attack2 or Attack3 Clip, based on which is coming up
        if (k_currentAttack == 1) { PlaySound(attack1Clip); }
        if (k_currentAttack == 2) { PlaySound(attack2Clip); }
        if (k_currentAttack == 3) { PlaySound(attack3Clip); }

        // Call one of three attack animations "Attack1", "Attack2", "Attack3"
        k_animator.SetTrigger("Attack" + k_currentAttack);

        // Reset timer
        k_timeSinceAttack = 0.0f;
    }

    public void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, k_body2d.position + Vector2.up * 0.8f, Quaternion.identity);

        ProjectileControll projectile = projectileObject.GetComponent<ProjectileControll>();
        projectile.dmg = k_projectileDamage;
        projectile.Launch(k_facingDirection, 300);

        //Plays randomly either Fireball1, Fireball2 or Fireball3
        random = Random.value;
        if (random < 0.33f) { PlaySound(fireball1Clip); }
        else if (random > 0.66f) { PlaySound(fireball2Clip); }
        else { PlaySound(fireball3Clip); }

        k_timeSinceAttack = 0.0f;
    }

    public void Hurt()
    {
        k_body2d.velocity = new Vector2(0, 0);
        k_animator.SetTrigger("Hurt");
    }

    public void Death()
    {
        SceneManager.LoadScene("GameOverMenu 1");
    }

    public void Roll()
    {
        k_rolling = true;
        k_animator.SetTrigger("Roll");
        k_body2d.velocity = new Vector2(k_facingDirection * k_rollForce, vertical * k_speed);
        AE_ResetRoll();
        PlaySound(rollClip);
    }

    public void Run()
    {
        // Reset timer
        k_delayToIdle = 0.05f;
        k_animator.SetInteger("AnimState", 1);

        //Footsteps audio and timer
        if (isRunning) { return; }

        //Plays randomly either Footstep1, 2, 3 or 4
        random = Random.value;
        if (random < 0.25f) { PlaySound(footstep1Clip); }
        else if (random > 0.75f) { PlaySound(footstep2Clip); }
        else if (random < 0.5){ PlaySound(footstep3Clip); }
        else { PlaySound(footstep4Clip); }

        PlaySound(footstep1Clip);
        isRunning = true;
        //timeRunning = 0.4f / (k_speed / 4);
        if (isSpiritboxBuffed) { timeRunning = 0.4f / (k_speed / 4); }
        timerRunning = timeRunning;
    }

    public void Idle()
    {
        // Prevents flickering transitions to idle
        k_delayToIdle -= Time.deltaTime;
        if (k_delayToIdle < 0)
            k_animator.SetInteger("AnimState", 0);
    }

    //Urn Consumable, heals the player
    public void UseUrn()
    {
        if (Urns != 0)
        {
            Urns--;
            ChangeHealth(20);
            DisplayUrnDialog();
            PlaySound(urnUsedClip);
        }
    }

    //Spiritbox consumable, buffs the player's speed and damge
    public void UseSpiritbox()
    {
        if (Spiritboxes != 0)
        {
            //Set Timer
            isSpiritboxBuffed = true;
            spiritboxTimer = spiritboxTimeBuffed;
            Spiritboxes--;

            //Sets the buff to be between 1-100% randomly
            percentage = 1.0f + Random.value;

            //Saves the knights speed, melee damage, and projectile damage before adjusting it
            k_preSpiritBoxSpeed = k_speed;
            k_preSpiritBoxMeleeDamage = k_meleeDamage;
            k_preSpiritBoxProjectileDamage = k_projectileDamage;

            //Applies the Spiritbox buff
            k_speed *= percentage;
            k_meleeDamage = (int)(k_meleeDamage * percentage);
            k_projectileDamage = (int)(k_projectileDamage * percentage);

            //Dialog on screen
            DisplaySpiritboxDialog();

            //Audio
            SetFootstepFrequency();
            PlaySound(spiritboxUsedClip);
        }
    }
    public void SpiritboxRemoveBuff()
    {
        isSpiritboxBuffed = false;
        k_speed = k_preSpiritBoxSpeed;
        k_meleeDamage = k_preSpiritBoxMeleeDamage;
        k_projectileDamage = k_preSpiritBoxProjectileDamage;
        //Audio
        SetFootstepFrequency();
    }

    public void DisplayUrnDialog()
    {
        timerUrnDisplay = displayTime;
        urnDialogBox.SetActive(true);
    }

    public void DisplaySpiritboxDialog()
    {
        Countdown.instance.StartCountdown(spiritboxTimeBuffed);
        timerSpiritboxUIDisplay = spiritboxTimeBuffed;
        timerSpiritboxDisplay = displayTime;
        spiritboxDialogBox.SetActive(true);
        spiritboxUI.SetActive(true);
        BonusDamageUI.instance.StartBonusDamageUI(percentage);
    }
    
    //For test purposes
    public void DamageKnightTest()
    {
        ChangeHealth(-10);
    }

    //Audio
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    
    //When the knight's speed is changed, use this to sync the footsteps to it
    public void SetFootstepFrequency() 
    {
        timeRunning = 0.4f / (k_speed / 4);
    }

    //Mapgen
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

            enemySpawner.level += 1;
            enemySpawner.Start();
            lootSpawner.Start();
            playerSpawn.Start();
            pathfinder.GetComponent<Scan>().Start();
        
        }
    }
}
