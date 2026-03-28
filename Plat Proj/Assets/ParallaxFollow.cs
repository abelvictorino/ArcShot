using UnityEngine;

public class ParallaxFollow : MonoBehaviour
{
    [SerializeField] private float parallax = 0.9f; // 0.9 = follows slightly slower than camera
    [SerializeField] private Vector2 offset;

    private Transform cam;
    void Start(){ cam = Camera.main.transform; }

    void LateUpdate()
    {
        if (!cam) return;
        transform.position = new Vector3(cam.position.x * parallax + offset.x,
                                         cam.position.y * parallax + offset.y,
                                         transform.position.z);
    }
}
