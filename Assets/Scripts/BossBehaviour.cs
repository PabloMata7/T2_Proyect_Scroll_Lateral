using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    AudioSource audioSource;
    AudioClip atackSound;
    AudioClip atackSound2;
    AudioClip atackSound3;
    AudioClip deathSound;
    [Header("Estadísticas")]
    public float moveSpeed = 3f;
    public float detectionRange = 6f;
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;
    public int maxHealth = 15; 
    [SerializeField] private int currentHealth;

    public LayerMask playerLayer;

    private enum State { Idle, Chasing, Attacking }
    private State currentState = State.Idle;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private Animator anim;

    private float lastAttackTime;
    private bool facingRight = true;
    private bool isProvoked = false;

    private static readonly int Attack1Hash = Animator.StringToHash("Attack1");  // Fase 1
    private static readonly int Attack2Hash = Animator.StringToHash("Attack2");  // Fase 2
    private static readonly int Attack3Hash = Animator.StringToHash("Attack3");   // Fase 3
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int HurtHash = Animator.StringToHash("Hurt");  
    private static readonly int DeathHash = Animator.StringToHash("Death");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        lastAttackTime = -attackCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform == null && Player.Instance != null)
            playerTransform = Player.Instance.transform;

        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        switch (currentState)
        {
            case State.Idle:
                if (distanceToPlayer < detectionRange)
                    currentState = State.Chasing;
                break;

            case State.Chasing:
                if (distanceToPlayer <= attackRange)
                {
                    currentState = State.Attacking;
                    rb.velocity = Vector2.zero;
                }
                else
                {
                    float chaseDistance = isProvoked ? detectionRange * 3f : detectionRange * 1.5f;
                    if (distanceToPlayer > chaseDistance)
                    {
                        currentState = State.Idle;
                        rb.velocity = Vector2.zero;
                        isProvoked = false;
                    }
                }
                LookAtPlayer();
                break;

            case State.Attacking:
                // Lógica de salida del estado de ataque (si el jugador huye)
                // Nota: La ejecución del ataque se hace en UpdateAnimations o aquí mismo.
                // Para mantener tu estructura, la dejamos abajo, pero controlamos el cambio de estado.

                // Solo permitimos volver a perseguir si NO estamos en mitad de la animación de ataque
                // (Esto depende de cómo tengas configurado el Exit Time, pero por seguridad lógica):
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    if (distanceToPlayer > attackRange)
                    {
                        currentState = State.Chasing;
                    }
                }
                break;
        }

        UpdateAnimations();

        if (currentState == State.Chasing)
            LookAtPlayer();
    }

    private void FixedUpdate()
    {
        if (currentState == State.Chasing && playerTransform != null)
            MoveToPlayer();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentState != State.Attacking)
        {
            currentState = State.Chasing;
            isProvoked = true;
            if (playerTransform == null && Player.Instance != null)
                playerTransform = Player.Instance.transform;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            //if (Random < 0.3f)
            //anim.SetTrigger(HurtHash);
        }
    }

    private void MoveToPlayer()
    {
        float direction = Mathf.Sign(playerTransform.position.x - transform.position.x);
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
    }

    private void LookAtPlayer()
    {
        if (playerTransform.position.x > transform.position.x && !facingRight) Flip();
        else if (playerTransform.position.x < transform.position.x && facingRight) Flip();
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void FaceTowardsPlayer()
    {
        // Si el jugador está a la derecha y el boss mira a la izquierda -> Flip
        if (playerTransform.position.x > transform.position.x && !facingRight)
        {
            Flip();
        }
        // Si el jugador está a la izquierda y el boss mira a la derecha -> Flip
        else if (playerTransform.position.x < transform.position.x && facingRight)
        {
            Flip();
        }
    }

    private void FaceAwayFromPlayer()
    {
        // Si el jugador está a la derecha, el boss quiere mirar a la IZQUIERDA -> Flip si estoy mirando a derecha
        if (playerTransform.position.x > transform.position.x && facingRight)
        {
            Flip();
        }
        // Si el jugador está a la izquierda, el boss quiere mirar a la DERECHA -> Flip si estoy mirando a izquierda
        else if (playerTransform.position.x < transform.position.x && !facingRight)
        {
            Flip();
        }
    }

    private void UpdateAnimations()
    {
        anim.SetFloat(SpeedHash, Mathf.Abs(rb.velocity.x));

        if (currentState == State.Attacking)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack(); // Decisión de Boss
            }
        }
    }


    private void Attack()
    {
        lastAttackTime = Time.time;

        // 1. Calculamos % de vida
        float healthPercent = (float)currentHealth / maxHealth;

        // 2. Factor Aleatorio (Weighted Random)
        int roll = Random.Range(0, 100);

        int hashSelected = 0;

        // 3. Decisión basada en fases
        if (healthPercent > 0.66f) // FASE 1 (Vida Alta)
        {
            // Solo ataque básico
            hashSelected = Attack2Hash;
            Debug.Log("Boss: Fase 1 (Swipe)");
        }
        else if (healthPercent > 0.33f) // FASE 2 (Vida Media)
        {
            // 30% Básico, 70% Fuerte (Attack2)
            if (roll < 30) hashSelected = Attack2Hash;
            else hashSelected = Attack1Hash;
            Debug.Log("Boss: Fase 2 (Mix)");
        }
        else // FASE 3 (Vida Baja)
        {
            // 20% Básico, 30% Fuerte, 50% ULTI (Attack3)
            if (roll < 20) hashSelected = Attack2Hash;
            else if (roll < 50) hashSelected = Attack1Hash;
            else hashSelected = Attack3Hash;
            Debug.Log("Boss: Fase 3 (Berserk)");
        }

        if(hashSelected == Attack2Hash)
        {
            FaceAwayFromPlayer();
        }
        else
        {
            FaceTowardsPlayer();
        }

        anim.SetTrigger(hashSelected);
        
        //PerformDamageCheck(); // <--- OJO: Idealmente mover esto a Animation Event
    }

    public void PerformDamageCheck(int attackType)
    {
        float damage = 1f;
        float range = attackRange;

        // Configuración según el tipo de ataque (Hardcoded o por variables)
        switch (attackType)
        {
            case 1:
                damage = 1f;
                range = attackRange;
                break;
            case 2:
                damage = 1f;
                range = attackRange * 1.5f; // Más alcance
                break;
            case 3:
                damage = 2f;
                range = attackRange * 2f; // Mucho alcance
                break;
        }

        Debug.Log($"EVENTO DISPARADO: Tipo {attackType}");

        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);

        if (hitPlayer != null)
        {
            // Asumiendo que Player tiene TakeDamage
            Player.Instance.TakePlayerDamage(1.0f);
            Debug.Log($"¡Golpe confirmado! Daño: {damage}");
        }
    }

    private void Die()
    {
        this.enabled = false; // Desactiva la IA
        anim.SetTrigger(DeathHash);
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        Destroy(gameObject, 5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange); // Rango visión

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Rango ataque

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange * 1.5f);
    }
    private void AttackSound1()
    {
        if (audioSource != null && atackSound != null)
        {
            audioSource.PlayOneShot(atackSound);
        }
    }
    private void AttackSound2()
    {
        if (audioSource != null && atackSound2 != null)
        {
            audioSource.PlayOneShot(atackSound2);
        }
    }
    private void AttackSound3()
    {
        if (audioSource != null && atackSound3 != null)
        {
            audioSource.PlayOneShot(atackSound3);
        }
    }
}
