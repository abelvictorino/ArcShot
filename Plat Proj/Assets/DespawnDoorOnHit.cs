using System.Collections;
using UnityEngine;

public class DespawnDoorOnHit : MonoBehaviour
{
    [Header("What to unlock/despawn")]
    [SerializeField] private GameObject doorRoot;          // assign the door/wall GameObject (has SpriteRenderer + Collider2D)

    [Header("How it behaves after a hit")]
    [SerializeField] private float fadeSeconds = 0f;        // 0 = no fade (instant)
    [SerializeField] private bool destroyDoorAfter = true;  // destroy doorRoot at the end (else just hide/disable)
    [SerializeField] private bool disableCollidersImmediately = true; // let player pass right away

    [Header("Target self behavior")]
    [SerializeField] private bool hideTargetOnHit = true;   // hide this target’s visuals
    [SerializeField] private bool disableTargetTrigger = true;

    private bool used = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used) return;
        if (!other.CompareTag("Projectile")) return;
        used = true;

        // Clean up the projectile that hit us
        Destroy(other.gameObject);

        // Open/remove the door
        if (doorRoot != null)
        {
            if (fadeSeconds > 0f)
                StartCoroutine(FadeAndDespawnDoor());
            else
                InstantDespawnDoor();
        }
        else
        {
            Debug.LogWarning($"{name}: No doorRoot assigned on DespawnDoorOnHit.");
        }

        // Target visuals/trigger cleanup
        if (hideTargetOnHit) HideTargetVisuals();
        if (disableTargetTrigger)
        {
            var col = GetComponent<Collider2D>();
            if (col) col.enabled = false;
        }
    }

    private void InstantDespawnDoor()
    {
        // Optionally allow passage immediately
        if (disableCollidersImmediately)
        {
            foreach (var c in doorRoot.GetComponentsInChildren<Collider2D>(true))
                c.enabled = false;
        }

        if (destroyDoorAfter)
        {
            Destroy(doorRoot);
        }
        else
        {
            // Hide visuals instead of destroying
            foreach (var sr in doorRoot.GetComponentsInChildren<SpriteRenderer>(true))
                sr.enabled = false;
            doorRoot.SetActive(false);
        }
    }

    private IEnumerator FadeAndDespawnDoor()
    {
        var srs = doorRoot.GetComponentsInChildren<SpriteRenderer>(true);

        // Disable colliders right away so the player can pass during the fade
        if (disableCollidersImmediately)
        {
            foreach (var c in doorRoot.GetComponentsInChildren<Collider2D>(true))
                c.enabled = false;
        }

        // Cache original colors
        var originals = new Color[srs.Length];
        for (int i = 0; i < srs.Length; i++) originals[i] = srs[i].color;

        float t = 0f;
        while (t < fadeSeconds)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, t / fadeSeconds);
            for (int i = 0; i < srs.Length; i++)
            {
                var c = originals[i];
                c.a = a;
                srs[i].color = c;
            }
            yield return null;
        }

        if (destroyDoorAfter)
            Destroy(doorRoot);
        else
        {
            foreach (var sr in srs) sr.enabled = false;
            doorRoot.SetActive(false);
        }
    }

    private void HideTargetVisuals()
    {
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>(true))
            sr.enabled = false;
    }
}
