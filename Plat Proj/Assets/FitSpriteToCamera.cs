using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class FitSpriteToCamera : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float padding = 0f; // world units added to width/height

    void Reset() { cam = Camera.main; }
    void LateUpdate()
    {
        if (cam == null) cam = Camera.main;
        var sr = GetComponent<SpriteRenderer>();
        if (!sr || !sr.sprite || !cam || !cam.orthographic) return;

        // Center on camera
        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f);

        // Compute camera size in world units
        float worldH = cam.orthographicSize * 2f;
        float worldW = worldH * cam.aspect;

        // Sprite size in world units
        Vector2 spriteSize = sr.sprite.bounds.size;

        // Scale uniformly to cover entire view
        float scaleX = (worldW + padding) / spriteSize.x;
        float scaleY = (worldH + padding) / spriteSize.y;
        float scale = Mathf.Max(scaleX, scaleY);

        transform.localScale = new Vector3(scale, scale, 1f);
    }
}
