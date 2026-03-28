using System.Collections;
using UnityEngine;

public class CameraShake2D : MonoBehaviour
{
    public static CameraShake2D I;
    void Awake(){ I = this; }

    public void Shake(float duration = 0.08f, float amplitude = 0.05f)
    {
        StopAllCoroutines();
        StartCoroutine(DoShake(duration, amplitude));
    }

    IEnumerator DoShake(float d, float a)
    {
        Vector3 basePos = transform.localPosition;
        float t = 0f;
        while (t < d)
        {
            t += Time.deltaTime;
            transform.localPosition = basePos + (Vector3)Random.insideUnitCircle * a;
            yield return null;
        }
        transform.localPosition = basePos;
    }
}
