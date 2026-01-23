using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip atackSound;
    public AudioClip hitSound;
    public AudioClip deathSound;

    public float moveSpeed = 3f;
    public float detectionRange = 6f;   // Distancia para empezar a perseguir
    public float attackRange = 1.2f;    // Distancia para golpear
    public float attackCooldown = 1.5f; // Tiempo entre golpes

    public int enemyHealth = 3;
    [SerializeField] private int currentHealth;

    public LayerMask playerLayer;

    private enum State { Idle, Chasing, Attacking }
    private State currentState = State.Idle;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private Animator anim;

    private float lastAttackTime;
    private bool facingRight = true;
    private float facingThreshold = 0.5f;

    private bool isProvoked = false;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int DeathHash = Animator.StringToHash("Death");
    private static readonly int HurtHash = Animator.StringToHash("Hurt");

    private float soundDelay = 0.5f;
    private float soundTime = 0f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = enemyHealth;
    }

    // Update is called once per frame
    void Update()
    {
        soundTime += Time.deltaTime;

        // Buscamos al jugador dinámicamente si lo perdemos, o usamos Player.Instance.
        if (playerTransform == null && Player.Instance != null)
        {
            playerTransform = Player.Instance.transform;
        }

        if (playerTransform == null) return; // Si no hay player, no hacemos nada (Idle)

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        switch (currentState)
        {
            case State.Idle:
                if (distanceToPlayer < detectionRange)  // Si la distancia del enemigo al jugador es menor que el rango de detección
                {                                       // Pasamos al estado Chasing
                    currentState = State.Chasing;
                }
                break;
            case State.Chasing:
                if (distanceToPlayer <= attackRange)    // Si la distancia entre el enemigo y el jugador es menor al rango de ataque, ataca
                {
                    currentState = State.Attacking;
                    rb.velocity = Vector2.zero;         // Frenamos en seco para atacar
                }
                else
                {
                    // para evitar que el enemigo "vibre" en el borde de detección.
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
                Debug.Log("Atacando");
                // El estado de ataque se gestiona por cooldown. 
                // Si el jugador se aleja mientras atacamos, volveremos a perseguir tras el golpe.
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    // Si sigue cerca, atacamos de nuevo (la lógica de ataque está abajo)
                    // Si se alejó, volvemos a perseguir
                    if (distanceToPlayer > attackRange)
                    {
                        currentState = State.Chasing;
                    }
                }
                break;
        }

        UpdateAnimations();

        //Orientacion
        if (currentState == State.Chasing)
        {
            LookAtPlayer();
        }

    }

    private void FixedUpdate()
    {
        // 4. MOVIMIENTO FÍSICO
        // Solo nos movemos si estamos persiguiendo. En ataque e idle estamos quietos.
        if (currentState == State.Chasing && playerTransform != null)
        {
            MoveToPlayer();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentState != State.Attacking) //No interrumpir el ataque
        {
            currentState = State.Chasing;
            isProvoked = true;

            if (playerTransform == null && Player.Instance != null)
            {
                playerTransform = Player.Instance.transform;
            }
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            anim.SetTrigger(HurtHash);
        }
    }

    private void Die()
    {
        // 1. Desactivar físicas y scripts para que deje de atacar/moverse
        rb.velocity = Vector2.zero;
        //GetComponent<Collider2D>().enabled = false;
        this.enabled = false; // Desactiva este script

        // 2. Animación de muerte
        anim.SetTrigger(DeathHash);
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        // 3. Destruir el objeto (con delay para que se vea la animación)
        Destroy(gameObject, 2f);
    }

    private void MoveToPlayer()
    {
        float xDifference = playerTransform.position.x - transform.position.x;

        if (Mathf.Abs(xDifference) < facingThreshold)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            return;
        }

        // Dirección normalizada hacia el jugador (solo eje X para plataformas)
        float direction = Mathf.Sign(xDifference);
        
        // Aplicamos velocidad manteniendo la gravedad (velocity.y)
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
    }

    private void LookAtPlayer()
    {
        float xDifference = playerTransform.position.x - transform.position.x;

        if (Mathf.Abs(xDifference) < facingThreshold) return;

        if (playerTransform.position.x > transform.position.x && !facingRight)
        {
            Flip();
        }
        else if (playerTransform.position.x < transform.position.x && facingRight)
        {
            Flip();
        }
    }

    private void Flip() // Clase similar a la del jugador
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void UpdateAnimations()
    {
        // Sincronizamos la velocidad física con el parámetro "Speed"
        // Si estamos atacando, la velocidad es 0, así que el animator pasará a Idle o Attack base.
        anim.SetFloat(SpeedHash, Mathf.Abs(rb.velocity.x));

        // Lógica de Ataque
        if (currentState == State.Attacking)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
            }
        }
    }

    private void Attack()
    {
        lastAttackTime = Time.time;
        anim.SetTrigger(AttackHash); // Disparamos la animación

        //PerformDamageCheck();
    }

    public void PerformDamageCheck()
    {
        Debug.Log($"EVENTO DISPARADO: Ataque");

        // Detectamos si el jugador sigue en rango de golpe
        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);

        if (hitPlayer != null)
        {
            Debug.Log("¡Enemigo golpeó al jugador!");
            Player.Instance.TakePlayerDamage(1.0f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange); // Rango visión

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Rango ataque
    }
    private void AttackSound()
    {
        if(audioSource != null && atackSound != null)
        {
            audioSource.PlayOneShot(atackSound);
        }
    }

    private void HitSound()
    {
        if (audioSource != null && hitSound != null && soundTime>soundDelay)
        {
            audioSource.PlayOneShot(hitSound);
            Debug.Log("sonido");
            soundTime = 0;
        }
    }

}
