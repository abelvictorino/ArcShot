using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NeonFlicker : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float minAlpha = 0.6f;
    [SerializeField] private float maxAlpha = 1f;
    [SerializeField] private float seed = 0.123f;

    private SpriteRenderer sr;
    private Color baseColor;

    void Awake(){ sr = GetComponent<SpriteRenderer>(); baseColor = sr.color; }

    void Update()
    {
        float n = Mathf.PerlinNoise(seed, Time.time * speed);
        float a = Mathf.Lerp(minAlpha, maxAlpha, n);
        sr.color = new Color(baseColor.r, baseColor.g, baseColor.b, a);
    }
}
