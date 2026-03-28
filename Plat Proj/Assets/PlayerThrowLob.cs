using UnityEngine;
using UnityEngine.InputSystem;


// reviewer comments: Add a cooldown to projectile throw to incentivize good timing

[RequireComponent(typeof(PlayerController_InputSystem))]
public class PlayerThrowLob : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float launchSpeed = 10f;
    [Range(5f, 80f)]
    [SerializeField] private float launchAngleDegrees = 25f;


    private PlayerController_InputSystem mover;

    private InputSystem_Actions input;
    private InputSystem_Actions.PlayerActions player;

    void Awake()
    {
        mover = GetComponent<PlayerController_InputSystem>();
        input = new InputSystem_Actions();
        player = input.Player;
    }


    void OnEnable()
    {
        player.Attack.performed += OnAttack;
        player.Enable();
    }

    void OnDisable()
    {
        player.Attack.performed -= OnAttack;
        player.Disable();
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        FireLob();
    }

    private void FireLob()
    {
        if (!projectilePrefab || !firePoint) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        var rb = proj.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        // Ensure projectile uses gravity for an arc
        if (rb.gravityScale <= 0f) rb.gravityScale = 1f;

        float angleRad = launchAngleDegrees * Mathf.Deg2Rad;
        Vector2 localVel = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * launchSpeed;

        // Flip horizontally based on facing
        localVel.x *= mover.FacingSign;

        rb.linearVelocity = localVel;
    }
}
