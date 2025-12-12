using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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
    private bool facingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
    }

    private void FixedUpdate()
    {
        // 2. PHYSICS CALCULATIONS (Se ejecuta en intervalos fijos, por defecto 0.02s)

        CheckGround();
        Move();

        if (jumpRequested)
        {
            Jump();
            jumpRequested = false; // Consumimos el flag
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
        if (horizontalInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (horizontalInput < 0 && facingRight)
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

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
