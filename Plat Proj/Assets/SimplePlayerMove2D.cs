using UnityEngine;
using UnityEngine.InputSystem; // new Input System

// reviewer comments: Enable jumping feature to add complexity for other levels
// and to allow the player to reach higher platforms.

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController_InputSystem : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    [Header("Scene Refs")]
    [SerializeField] private Transform firePoint;     // child on Player
    [SerializeField] private Transform groundCheck;   // child at feet
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float groundCheckRadius = 0.1f;

    [Header("Lob Throw")]
    [SerializeField] private float launchSpeed = 10f;
    [Range(5f,80f)]
    [SerializeField] private float launchAngleDegrees = 25f;

    [Header("Animation")]
    [SerializeField] private Animator animator;        // assign in Inspector or auto-get in Awake
    [SerializeField] private string speedParam = "Speed";
    [SerializeField] private bool normalizeSpeed = true; // true = 0..1 based on moveSpeed

    [Header("Attack Animation")]
    [SerializeField] private string attackTriggerParam = "Shoot"; // Animator Trigger
    [SerializeField] private bool fireViaAnimEvent = true;        // true: use Anim_Fire event in Shoot.anim

    [Header("Throw Timing")]
    [Tooltip("Extra delay before the projectile is spawned. If using an Animation Event, this delay is applied AFTER the event fires.")]
    [SerializeField] private float throwDelay = 0.0f;
    [Tooltip("Cooldown after a shot before another can be triggered.")]
    [SerializeField] private float throwCooldown = 0.35f;

    private float nextShootTime = 0f;
    private Coroutine pendingThrow;      // used when NOT using animation events
    private bool awaitingAnimFire = false;

    private Rigidbody2D rb;
    private InputSystem_Actions input;                     // generated class from your asset
    private InputSystem_Actions.PlayerActions player;      // the "Player" map
    private int facingSign = 1; // +1 right, -1 left
    public int FacingSign => facingSign;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        if (animator == null) animator = GetComponent<Animator>();

        input = new InputSystem_Actions();
        player = input.Player; // exposes Move/Attack/Jump actions
    }

    void OnEnable()
    {
        player.AddCallbacks(this); // register this MonoBehaviour for callbacks
        player.Enable();           // enable all actions in the map
    }

    void OnDisable()
    {
        player.RemoveCallbacks(this);
        player.Disable();

        if (pendingThrow != null) { StopCoroutine(pendingThrow); pendingThrow = null; }
        awaitingAnimFire = false;
    }

    void Update()
    {
        // Read Move.x every frame
        Vector2 move = player.Move.ReadValue<Vector2>();
        float x = Mathf.Clamp(move.x, -1f, 1f);

        // Horizontal movement
        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y); // replace 'linearVelocity' with 'velocity' if needed

        // Flip by input (keeps FacingSign correct for throws)
        if (x > 0.01f) { facingSign = 1;  transform.localScale = new Vector3(1, 1, 1); }
        else if (x < -0.01f) { facingSign = -1; transform.localScale = new Vector3(-1, 1, 1); }

        // ---- Drive Animator "Speed" for Blend Tree (Idle<->Run)
        if (animator != null)
        {
            float speedX = Mathf.Abs(rb.linearVelocity.x); // replace with rb.velocity.x if needed
            float value = normalizeSpeed ? Mathf.Clamp01(speedX / Mathf.Max(0.01f, moveSpeed)) : speedX;

            // small deadzone so Idle doesn't flicker
            if (value < 0.05f) value = 0f;

            animator.SetFloat(speedParam, value);
        }
    }

    // ---- Input Callbacks from IPlayerActions ----
    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        bool grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);
        if (grounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // replace with rb.velocity if needed
        }
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (throwDelay > 0f)
        {
            StartCoroutine(FireAfterDelay(throwDelay));
        }
        else
        {
            FireLob();
            nextShootTime = Time.time + throwCooldown;
        }

        if (!ctx.performed) return;

        nextShootTime = Time.time + throwCooldown;
        
        // Cooldown gate: ignore input until nextShootTime
        if (Time.time < nextShootTime) return;

        // Trigger the Shoot animation (if any)
        if (animator && !string.IsNullOrEmpty(attackTriggerParam))
            animator.SetTrigger(attackTriggerParam);

        if (fireViaAnimEvent)
        {
            // We'll fire when Anim_Fire gets called (plus optional throwDelay)
            awaitingAnimFire = true;
        }
        else
        {
            // No anim event: schedule a delayed fire, then start cooldown when it actually fires
            if (pendingThrow == null)
                pendingThrow = StartCoroutine(FireAfterDelay(throwDelay));
        }
    }

    // Unused but required by the interface:
    public void OnMove(InputAction.CallbackContext ctx) { /* handled in Update */ }
    public void OnLook(InputAction.CallbackContext ctx) { }
    public void OnInteract(InputAction.CallbackContext ctx) { }
    public void OnCrouch(InputAction.CallbackContext ctx) { }
    public void OnPrevious(InputAction.CallbackContext ctx) { }
    public void OnNext(InputAction.CallbackContext ctx) { }
    public void OnSprint(InputAction.CallbackContext ctx) { }

    private System.Collections.IEnumerator FireAfterDelay(float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);
        FireLob();
        nextShootTime = Time.time + throwCooldown; // cooldown starts when the shot actually goes out
        pendingThrow = null;
    }

    private void FireLob()
    {
        if (!projectilePrefab || !firePoint) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        var rbProj = proj.GetComponent<Rigidbody2D>();
        if (rbProj == null) return;

        if (rbProj.gravityScale <= 0f) rbProj.gravityScale = 1f;

        float angleRad = launchAngleDegrees * Mathf.Deg2Rad;
        Vector2 localVel = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * launchSpeed;
        localVel.x *= facingSign; // flip by facing

        rbProj.linearVelocity = localVel; // replace with rbProj.velocity if needed
    }

    // for visualizing ground check
    void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
