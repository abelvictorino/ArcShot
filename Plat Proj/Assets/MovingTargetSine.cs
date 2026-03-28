using UnityEngine;

public class MovingTargetSine : MonoBehaviour
{
    [SerializeField] private Vector2 amplitude = new Vector2(0f, 1.5f);
    [SerializeField] private Vector2 speed = new Vector2(0f, 1f);

    private Vector3 start;

    void Awake() => start = transform.position;

    void Update()
    {
        float t = Time.time;
        float dx = amplitude.x * Mathf.Sin(t * speed.x * Mathf.PI * 2f);
        float dy = amplitude.y * Mathf.Sin(t * speed.y * Mathf.PI * 2f);
        transform.position = start + new Vector3(dx, dy, 0f);
    }
}
