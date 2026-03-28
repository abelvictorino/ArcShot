using System.Collections;
using UnityEngine;

public class SpawnPop : MonoBehaviour
{
    [SerializeField] private float popTime = 0.12f;
    [SerializeField] private float startScale = 0.6f;

    void OnEnable()
    {
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        var t0 = Time.time;
        Vector3 endScale = transform.localScale;
        transform.localScale = endScale * startScale;

        // quick white flash overlay (optional)
        var srs = GetComponentsInChildren<SpriteRenderer>(true);
        Color[] baseColors = new Color[srs.Length];
        for (int i = 0; i < srs.Length; i++) baseColors[i] = srs[i].color;

        while (Time.time - t0 < popTime)
        {
            float k = (Time.time - t0) / popTime;
            transform.localScale = Vector3.Lerp(endScale * startScale, endScale, k);

            // flash
            for (int i=0;i<srs.Length;i++)
            {
                var c = baseColors[i];
                float add = Mathf.Lerp(0.7f, 0f, k);
                srs[i].color = new Color(c.r + add, c.g + add, c.b + add, c.a);
            }
            yield return null;
        }
        // restore colors
        for (int i=0;i<srs.Length;i++) srs[i].color = baseColors[i];
        transform.localScale = endScale;
    }
}
