using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public float moveSpeed = 8f;
    public float jumpForce = 14f;
    public float airControlDamping = 0.2f;

    public Transform groundCheck;     
    public float groundCheckRadius = 0.2f; 
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool isGrounded;
    private bool jumpRequested;
    public bool facingRight = true;

    public bool isShooting = false;

    //Dash
    [SerializeField] private float dashDistance = 5f;      // Velocidad explosiva
    [SerializeField] private float dashDuration = 0.2f;  // Tiempo que dura el impulso
    [SerializeField] private float dashCooldown = 1f;    // Tiempo de recuperación

    private float calculatedDashSpeed;

    // Estado interno del Dash
    public bool isDashing;
    private bool canDash = true;
    private float originalGravity; // Para restaurar la gravedad tras el dash

    [SerializeField] private GameObject ghostPrefab; // prefab GhostSprite
    [SerializeField] private float ghostSpawnInterval = 0.05f; // Cada cuánto tiempo sale un fantasma
    [SerializeField] private int poolSize = 10; // Cuántos fantasmas reciclables tendremos

    private Queue<GameObject> ghostPool = new Queue<GameObject>();

    private SpriteRenderer playerSR;

    private Animator animator;

    // Pre-cálculo de Hashes del Animator
    // Evitamos usar strings en el Update (ahorro de CPU y GC)
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int GroundedHash = Animator.StringToHash("IsGrounded"); 
    private static readonly int ShootHash = Animator.StringToHash("Shoot");
    private static readonly int DashHash = Animator.StringToHash("Dash");

    private void Awake()
    {
        // Si ya existe una instancia y no soy yo, me destruyo (Patrón Singleton básico)
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        // ASIGNACIÓN CRÍTICA: Aquí es donde "Instance" deja de ser null
        Instance = this;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        playerSR = GetComponent<SpriteRenderer>();

        InitializeGhostPool();

        calculatedDashSpeed = dashDistance / dashDuration;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpRequested = true;
        }
        // Input para Dash 
        if (Input.GetKeyDown(KeyCode.E) && canDash)
        {
            StartCoroutine(Dash());
        }
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        // SI ESTAMOS DASHEANDO NO SE EJECUTA
        if (isDashing) return;
        
        CheckGround();
        Move();

        if (jumpRequested)
        {
            Jump();
            jumpRequested = false; // Consumimos el flag
        }
    }


    private void UpdateAnimations()
    {
        if (animator == null) return;

        // "Speed": Usamos el valor absoluto de la velocidad horizontal real.
        // Mathf.Abs es necesario porque el Animator solo entiende magnitud (0 a infinito) para transiciones.
        animator.SetFloat(SpeedHash, Mathf.Abs(rb.velocity.x));

        // "IsGrounded": Sincronizamos el booleano físico con la máquina de estados.
        animator.SetBool(GroundedHash, isGrounded);
    }

    // 2. Método público para activar la animación
    // Lo hacemos público para que PlayerShooting.cs pueda llamarlo
    public void TriggerShootAnim()
    {
        if (animator != null)
        {
            animator.SetTrigger(ShootHash);
        }
    }

    private void Move()
    {
        // Calculamos la velocidad objetivo
        float targetVelocityX = horizontalInput * moveSpeed;

        // Si estamos en el aire, podríamos querer reducir el control (opcional)
        if (!isGrounded)
        {
            targetVelocityX = Mathf.Lerp(rb.velocity.x, targetVelocityX, 1f - airControlDamping);
        }

        // Modificamos la velocidad directamente para plataformas precisos.
        // Mantener _rb.velocity.y es crucial para que la gravedad haga su trabajo.
        rb.velocity = new Vector2(targetVelocityX, rb.velocity.y);

        // Gestión del Flip del sprite (Visual)
        if (horizontalInput > 0 && !facingRight && !isShooting)
        {
            Flip();
        }
        else if (horizontalInput < 0 && facingRight && !isShooting)
        {
            Flip();
        }
    }

    private void Jump()
    {
        // ForceMode2D.Impulse es ideal para saltos instantáneos (F = m * a aplicado en un instante)
        // Reseteamos la velocidad Y antes de aplicar fuerza para que el salto sea consistente
        // incluso si estamos cayendo ligeramente.
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void CheckGround()
    {
        // OverlapCircle es más eficiente y permisivo que un Raycast simple para plataformas.
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void OnValidate()
    {
        if (dashDuration > 0)
            calculatedDashSpeed = dashDistance / dashDuration;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        StartCoroutine(SpawnGhostRoutine());

        // 1. CONGELAR FÍSICA VERTICAL
        // Guardamos la gravedad actual y la ponemos a 0.
        // Esto evita que si haces dash en el aire, el personaje caiga en arco.
        // Queremos una línea recta horizontal perfecta.
        originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // 2. APLICAR VELOCIDAD
        // Calculamos la dirección basándonos en hacia dónde mira el sprite.
        // Usamos transform.localScale.x porque tu método Flip() invierte la escala.
        // Si facingRight es true, scale.x es 1. Si es false, es -1.
        float dashDirection = transform.localScale.x;

        // Asignamos velocidad directa. No usamos AddForce porque queremos velocidad constante instantánea.
        rb.velocity = new Vector2(dashDirection * calculatedDashSpeed, 0f);

        // 3. VISUALES
        if (animator != null) animator.SetTrigger(DashHash);

        // 4. DURACIÓN DEL DASH
        yield return new WaitForSeconds(dashDuration);

        // 5. RESTAURAR ESTADO
        rb.gravityScale = originalGravity;
        rb.velocity = Vector2.zero;
        isDashing = false;

        // 6. COOLDOWN
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void InitializeGhostPool()
    {
        for (int i = 0; i < poolSize; i++) 
        {
            GameObject ghostObj = Instantiate(ghostPrefab);
            ghostObj.SetActive(false);
            ghostPool.Enqueue(ghostObj);
        }
    }

    private IEnumerator SpawnGhostRoutine()
    {
        // Mientras estemos en estado de Dash...
        while (isDashing)
        {
            // 1. Sacamos un fantasma de la cola (el más antiguo)
            GameObject ghostObj = ghostPool.Dequeue();

            // 2. Lo colocamos en la posición actual del jugador
            ghostObj.transform.position = transform.position;
            ghostObj.transform.rotation = transform.rotation;
            // Importante: copiar la escala por si el flip se hace por escala
            ghostObj.transform.localScale = transform.localScale;

            // 3. Configuramos su sprite (copiamos el frame exacto del jugador)
            GhostTrailEffect ghostScript = ghostObj.GetComponent<GhostTrailEffect>();
            // Le pasamos el sprite actual y si está girado o no por SpriteRenderer
            ghostScript.SetUpGhost(playerSR.sprite, playerSR.flipX);

            // 4. Lo activamos (esto dispara su OnEnable y empieza a desvanecerse)
            ghostObj.SetActive(true);

            // 5. Lo volvemos a meter al final de la cola para reutilizarlo cuando le toque
            ghostPool.Enqueue(ghostObj);

            // Esperamos el intervalo antes del siguiente fantasma
            yield return new WaitForSeconds(ghostSpawnInterval);
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
