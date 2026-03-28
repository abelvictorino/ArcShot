using UnityEngine;

public class PulseGlow : MonoBehaviour
{
    [SerializeField] private float scaleAmp = 0.05f;   // 5% size pulse
    [SerializeField] private float alphaAmp = 0.25f;   // 25% alpha pulse
    [SerializeField] private float speed = 2.5f;       // pulse speed

    private SpriteRenderer sr;
    private Vector3 baseScale;
    private Color baseColor;
    private bool active = true;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseScale = transform.localScale;
        baseColor = sr ? sr.color : Color.white;
    }

    void Update()
    {
        if (!active || sr == null) return;
        float s = 0.5f + 0.5f * Mathf.Sin(Time.time * speed);
        transform.localScale = baseScale * (1f + scaleAmp * (s * 2f - 1f));
        sr.color = new Color(baseColor.r, baseColor.g, baseColor.b, Mathf.Clamp01(baseColor.a * (1f - alphaAmp + alphaAmp * (0.6f + 0.4f * s))));
    }

    // Call this when the target is hit so it stops pulsing (or hide/destroy separately)
    public void StopPulse()
    {
        active = false;
        transform.localScale = baseScale;
        if (sr) sr.color = baseColor;
    }
}
