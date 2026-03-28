using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Do not auto-destroy on triggers; the target script will destroy us on success.
        // If we hit solid (non-trigger) geometry, despawn.
        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
