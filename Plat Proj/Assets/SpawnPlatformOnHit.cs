using UnityEngine;

public class SpawnPlatformOnHit : MonoBehaviour
{
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("After hit behavior")]
    [SerializeField] private bool hideTargetOnHit = true;     // turn off SpriteRenderers
    [SerializeField] private bool disableTriggerOnHit = true; // stop future triggers
    [SerializeField] private bool destroyTargetOnHit = false; // alternative: remove object entirely

    private bool spawned = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (spawned) return;
        if (!other.CompareTag("Projectile")) return;

        if (platformPrefab && spawnPoint)
        {
            Instantiate(platformPrefab, spawnPoint.position, Quaternion.identity);
        }
        spawned = true;

        // Make projectile disappear on success
        Destroy(other.gameObject);

        // Post-hit behavior
        if (hideTargetOnHit) HideVisuals();
        if (disableTriggerOnHit)
        {
            var col = GetComponent<Collider2D>();
            if (col) col.enabled = false;
        }
        if (destroyTargetOnHit)
        {
            // If you use this, you can set hideTargetOnHit/disableTriggerOnHit to false.
            Destroy(gameObject);
        }
    }

    private void HideVisuals()
    {
        // Disable all SpriteRenderers on this object and its children
        var srs = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
        foreach (var sr in srs) sr.enabled = false;
    }
}
